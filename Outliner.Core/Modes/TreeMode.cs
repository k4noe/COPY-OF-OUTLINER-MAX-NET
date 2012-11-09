﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;
using Outliner.Controls.Tree;
using Outliner.Scene;
using Outliner.Commands;
using Outliner.Controls;
using Outliner.Filters;
// Import System.Windows.Forms with alias to avoid ambiguity 
// between System.Windows.TreeNode and Outliner.Controls.TreeNode.
using WinForms = System.Windows.Forms;
using Outliner.MaxUtils;
using Outliner.NodeSorters;
using Outliner.LayerTools;
using System.Drawing;
using Outliner.Controls.ContextMenu;
using System.IO;

namespace Outliner.Modes
{
public abstract class TreeMode
{
   protected Boolean started;
   public TreeView Tree { get; private set; }
   private ICollection<Tuple<GlobalDelegates.Delegate5, SystemNotificationCode>> systemNotifications;
   private ICollection<Tuple<uint, TreeModeNodeEventCallbacks>> nodeEventCallbacks;
   protected Dictionary<Object, List<TreeNode>> treeNodes { get; private set; }
   
   protected FilterCombinator<IMaxNodeWrapper> filters;
   private const Int32 InvisibleNodesFilterIndex = 0;
   private const Int32 PermanentFiltersIndex = 1;
   private const Int32 OtherFiltersIndex = 2;

   protected TreeMode(TreeView tree)
   {
      ExceptionHelpers.ThrowIfArgumentIsNull(tree, "tree");

      proc_PausePreSystemEvent = new GlobalDelegates.Delegate5(this.PausePreSystemEvent);
      proc_ResumePostSystemEvent = new GlobalDelegates.Delegate5(this.ResumePostSystemEvent);
      proc_SelectionsetChanged = new GlobalDelegates.Delegate5(this.SelectionSetChanged);

      this.Tree = tree;
      this.treeNodes = new Dictionary<Object, List<TreeNode>>();

      this.filters = new FilterCombinator<IMaxNodeWrapper>(Functor.And);
      this.filters.Filters.Add(new InvisibleNodeFilter());
      this.filters.Filters.Add(new MaxNodeFilterCombinator() { Enabled = false });
      this.filters.Filters.Add(new MaxNodeFilterCombinator() { Enabled = false });
      this.filters.FilterChanged += filters_FilterChanged;

      this.started = false;
   }

   protected abstract void FillTree();


   public virtual void Start()
   {
      if (started)
         return;

      this.RegisterSystemNotification(proc_PausePreSystemEvent, SystemNotificationCode.SystemPreNew);
      this.RegisterSystemNotification(proc_PausePreSystemEvent, SystemNotificationCode.SystemPreReset);
      this.RegisterSystemNotification(proc_PausePreSystemEvent, SystemNotificationCode.FilePreOpen);
      this.RegisterSystemNotification(proc_PausePreSystemEvent, SystemNotificationCode.FilePreMerge);
      this.RegisterSystemNotification(proc_SelectionsetChanged, SystemNotificationCode.SelectionsetChanged);

      this.RegisterNodeEventCallbackObject(new DefaultNodeEventCallbacks(this));

      this.Tree.SelectionChanged += tree_SelectionChanged;
      this.Tree.BeforeNodeTextEdit += tree_BeforeNodeTextEdit;
      this.Tree.AfterNodeTextEdit += tree_AfterNodeTextEdit;
      this.Tree.MouseClick += tree_MouseClick;

      this.FillTree();

      this.started = true;
   }

   public virtual void Stop()
   {
      if (!started)
         return;

      this.UnregisterSystemNotifications();
      this.UnregisterNodeEventCallbacks();

      this.Tree.SelectionChanged -= tree_SelectionChanged;
      this.Tree.BeforeNodeTextEdit -= tree_BeforeNodeTextEdit;
      this.Tree.AfterNodeTextEdit -= tree_AfterNodeTextEdit;
      this.Tree.MouseClick -= tree_MouseClick;

      this.Clear();

      this.started = false;
   }


   #region Register SystemNotifications and NodeEventCallbacks

   /// <summary>
   /// Registers a SystemNotification proc, which will be automatically unregistered when <see cref="UnregisterSystemNotifications"/> is called.
   /// </summary>
   protected void RegisterSystemNotification(GlobalDelegates.Delegate5 proc, SystemNotificationCode code)
   {
      if (this.systemNotifications == null)
         this.systemNotifications = new List<Tuple<GlobalDelegates.Delegate5, SystemNotificationCode>>();

      int regResult = MaxInterfaces.Global.RegisterNotification(proc, null, code);
      if (regResult != 0)
         this.systemNotifications.Add(new Tuple<GlobalDelegates.Delegate5, SystemNotificationCode>(proc, code));
   }

   /// <summary>
   /// Unregisters a SystemNotification. 
   /// Be sure not to create new delegates when calling this method, but stored ones used when registering it.
   /// </summary>
   protected void UnregisterSystemNotification(GlobalDelegates.Delegate5 proc, SystemNotificationCode code)
   {
      int unregResult = MaxInterfaces.Global.UnRegisterNotification(proc, null, code);

      if (unregResult != 0 && this.systemNotifications != null)
      {
         this.systemNotifications.Remove(new Tuple<GlobalDelegates.Delegate5, SystemNotificationCode>(proc, code));
      }
   }

   /// <summary>
   /// Unregisters all SystemNotifications registered using <see cref="RegisterSystemNotification"/>.
   /// </summary>
   protected virtual void UnregisterSystemNotifications()
   {
      if (this.systemNotifications == null)
         return;

      this.systemNotifications.ForEach(n =>
         MaxInterfaces.Global.UnRegisterNotification(n.Item1, null, n.Item2));
      
      this.systemNotifications.Clear();
      this.systemNotifications = null;
   }


   /// <summary>
   /// Registers a NodeEventCallback object, which will be automatically unregistered when <see cref="UnregisterNodeEventCallbacks"/> is called.
   /// </summary>
   protected void RegisterNodeEventCallbackObject(TreeModeNodeEventCallbacks cb)
   {
      if (nodeEventCallbacks == null)
         this.nodeEventCallbacks = new List<Tuple<uint, TreeModeNodeEventCallbacks>>();
      
      IISceneEventManager sceneEventMgr = MaxInterfaces.Global.ISceneEventManager;
      uint cbKey = sceneEventMgr.RegisterCallback(cb, false, 100, true);

      this.nodeEventCallbacks.Add(new Tuple<uint, TreeModeNodeEventCallbacks>(cbKey, cb));
   }

   /// <summary>
   /// Unregisters all NodeEventCallbacks registered using <see cref="RegisterNodeEventCallbackObject"/>.
   /// </summary>
   protected virtual void UnregisterNodeEventCallbacks()
   {
      if (this.nodeEventCallbacks == null)
         return;

      IISceneEventManager sceneEventMgr = MaxInterfaces.Global.ISceneEventManager;
      foreach (Tuple<uint, TreeModeNodeEventCallbacks> cb in this.nodeEventCallbacks)
      {
         sceneEventMgr.UnRegisterCallback(cb.Item1);
         cb.Item2.Dispose();
      }
      this.nodeEventCallbacks.Clear();
      this.nodeEventCallbacks = null;
   }

   protected abstract class TreeModeNodeEventCallbacks : Autodesk.Max.Plugins.INodeEventCallback
   {
      protected TreeMode treeMode { get; private set; }
      protected TreeView tree { get { return this.treeMode.Tree; } }
      protected Dictionary<Object, List<TreeNode>> treeNodes 
      { 
         get { return this.treeMode.treeNodes; } 
      }

      protected TreeModeNodeEventCallbacks(TreeMode treeMode)
      {
         this.treeMode = treeMode;
      }

      public override void CallbackBegin()
      {
         this.tree.BeginUpdate(TreeViewUpdateFlags.Redraw);
      }

      public override void CallbackEnd()
      {
         this.tree.EndUpdate();
      }
   }

   #endregion


   #region Helper methods
   
   public virtual List<TreeNode> GetTreeNodes(IMaxNodeWrapper wrapper)
   {
      if (wrapper == null)
         return null;

      return this.GetTreeNodes(wrapper.WrappedNode);
   }

   public virtual List<TreeNode> GetTreeNodes(Object node)
   {
      List<TreeNode> tns = null;
      if (node != null)
         this.treeNodes.TryGetValue(node, out tns);
      return tns;
   }

   /// <summary>
   /// Returns the first TreeNode found in the TreeNodes dictionary.
   /// Use when it's certain that each node has only a single TreeNode.
   /// </summary>
   public virtual TreeNode GetFirstTreeNode(Object node)
   {
      List<TreeNode> tns = this.GetTreeNodes(node);
      if (tns != null && tns.Count > 0)
         return tns[0];
      else
         return null;
   }

   public virtual TreeNode GetFirstTreeNode(IMaxNodeWrapper wrapper)
   {
      if (wrapper == null)
         return null;

      return this.GetFirstTreeNode(wrapper.WrappedNode);
   }

   public virtual void RegisterNode(Object node, TreeNode tn)
   {
      List<TreeNode> tns;
      if (!this.treeNodes.TryGetValue(node, out tns))
      {
         tns = new List<TreeNode>();
         this.treeNodes.Add(node, tns);
      }
      tns.Add(tn);
   }

   public virtual void RegisterNode(IMaxNodeWrapper node, TreeNode tn)
   {
      if (node == null)
         return;

      this.RegisterNode(node.WrappedNode, tn);
   }

   public virtual void UnregisterNode(Object node, TreeNode tn)
   {
      List<TreeNode> tns;
      if (this.treeNodes.TryGetValue(node, out tns))
      {
         tns.Remove(tn);
         if (tns.Count == 0)
            this.treeNodes.Remove(node);
      }
   }

   public virtual void UnregisterNode(IMaxNodeWrapper wrapper, TreeNode tn)
   {
      if (wrapper == null)
         return;

      this.UnregisterNode(wrapper.WrappedNode, tn);
   }

   public virtual void UnregisterNode(Object node)
   {
      this.treeNodes.Remove(node);
   }

   public virtual void UnregisterNode(IMaxNodeWrapper wrapper)
   {
      if (wrapper == null)
         return;

      this.UnregisterNode(wrapper.WrappedNode);
   }

   public virtual TreeNode AddNode(IMaxNodeWrapper wrapper, TreeNodeCollection parentCol)
   {
      ExceptionHelpers.ThrowIfArgumentIsNull(wrapper, "wrapper");
      ExceptionHelpers.ThrowIfArgumentIsNull(parentCol, "parentCol");

      TreeNode tn = new MaxTreeNode(wrapper);
      this.RegisterNode(wrapper, tn);

      tn.ShowNode = this.filters.ShowNode(wrapper);
      tn.DragDropHandler = this.CreateDragDropHandler(wrapper);

      parentCol.Add(tn);

      if (wrapper.Selected)
         this.Tree.SelectNode(tn, true);

      return tn;
   }

   public virtual TreeNode AddNode(Object node, TreeNodeCollection parentCol)
   {
      ExceptionHelpers.ThrowIfArgumentIsNull(node, "node");
      ExceptionHelpers.ThrowIfArgumentIsNull(parentCol, "parentCol");

      return this.AddNode(IMaxNodeWrapper.Create(node), parentCol);
   }


   public virtual DragDropHandler CreateDragDropHandler(IMaxNodeWrapper node)
   {
      return null;
   }


   public virtual void RemoveNode(IMaxNodeWrapper wrapper)
   {
      this.RemoveNode(wrapper.WrappedNode);
   }

   public virtual void RemoveNode(Object node)
   {
      List<TreeNode> tns = this.GetTreeNodes(node);
      if (tns != null)
      {
         foreach (TreeNode  tn in tns)
         {
            this.Tree.SelectNode(tn, false);
            //this.Tree.SelectedNodes.Remove(tn);
            tn.Remove();
         }
         this.UnregisterNode(node);
      }
   }

   public virtual void RemoveTreeNode(TreeNode tn)
   {
      IMaxNodeWrapper node = HelperMethods.GetMaxNode(tn);
      if (node == null)
         return;

      tn.Remove();

      List<TreeNode> tns = this.GetTreeNodes(node);
      if (tns != null)
      {
         tns.Remove(tn);
      }
   }


   protected void Clear()
   {
      this.Tree.Nodes.Clear();
      this.treeNodes.Clear();
   }



   public virtual void InvalidateObject(Object obj, Boolean recursive, Boolean sort)
   {
      if (obj != null)
      {
         List<TreeNode> tns = this.GetTreeNodes(obj);
         if (tns != null)
         {
            tns.ForEach(tn => tn.Invalidate(recursive));
            if (sort)
               this.Tree.StartTimedSort(tns);
         }
      }
   }

   public virtual void InvalidateTreeNodes(ITab<UIntPtr> nodes, Boolean invalidateBounds, Boolean sort)
   {
      foreach (IINode node in nodes.NodeKeysToINodeList())
      {
         List<TreeNode> tns = this.GetTreeNodes(node);
         if (tns != null)
         {
            if (invalidateBounds)
               tns.ForEach(tn => tn.InvalidateBounds(false, false));

            tns.ForEach(tn => tn.Invalidate());

            if (sort)
               this.Tree.AddToSortQueue(tns);
         }
      }

      if (sort)
         this.Tree.StartTimedSort(true);
   }

   public virtual void UpdateFilter(Object obj)
   {
      if (obj != null)
      {
         List<TreeNode> tns = this.GetTreeNodes(obj);
         if (tns != null)
         {
            foreach (TreeNode tn in tns)
            {
               IMaxNodeWrapper wrapper = HelperMethods.GetMaxNode(tn);
               tn.ShowNode = this.filters.ShowNode(wrapper);
            }
         }
      }
   }


   #endregion


   #region System notifications

   protected GlobalDelegates.Delegate5 proc_PausePreSystemEvent;
   protected virtual void PausePreSystemEvent(IntPtr param, IntPtr info)
   {
      this.Stop();

      this.RegisterSystemNotification(this.proc_ResumePostSystemEvent, SystemNotificationCode.SystemPostNew);
      this.RegisterSystemNotification(this.proc_ResumePostSystemEvent, SystemNotificationCode.SystemPostReset);
      this.RegisterSystemNotification(this.proc_ResumePostSystemEvent, SystemNotificationCode.FilePostOpen);
      this.RegisterSystemNotification(this.proc_ResumePostSystemEvent, SystemNotificationCode.FilePostMerge);
   }

   protected GlobalDelegates.Delegate5 proc_ResumePostSystemEvent;
   protected virtual void ResumePostSystemEvent(IntPtr param, IntPtr info)
   {
      this.UnregisterSystemNotifications();
      
      this.Start();
   }

   protected GlobalDelegates.Delegate5 proc_SelectionsetChanged;
   protected virtual void SelectionSetChanged(IntPtr param, IntPtr info)
   {
      this.Tree.SelectAllNodes(false);

      Int32 selNodeCount = MaxInterfaces.COREInterface.SelNodeCount;
      if (selNodeCount > 0)
      {
         for (Int32 i = 0; i < selNodeCount; i++)
         {
            List<TreeNode> tns = this.GetTreeNodes(MaxInterfaces.COREInterface.GetSelNode(i));
            if (tns != null)
               tns.ForEach(tn => this.Tree.SelectNode(tn, true));
         }
      }
   }

   #endregion


   #region NodeEventCallbacks

   protected class DefaultNodeEventCallbacks : TreeModeNodeEventCallbacks
   {
      public DefaultNodeEventCallbacks(TreeMode treeMode) : base(treeMode) { }

      public override void Deleted(ITab<UIntPtr> nodes)
      {
         foreach (IINode node in nodes.NodeKeysToINodeList())
            this.treeMode.RemoveNode(node);
      }

      public override void NameChanged(ITab<UIntPtr> nodes)
      {
         Boolean sort = this.tree.NodeSorter is AlphabeticalSorter;
         this.treeMode.InvalidateTreeNodes(nodes, true, sort);
      }

      public override void WireColorChanged(ITab<UIntPtr> nodes)
      {
         NodePropertySorter sorter = this.tree.NodeSorter as NodePropertySorter;
         Boolean sort = sorter != null && sorter.Property == NodeProperty.WireColor;
         this.treeMode.InvalidateTreeNodes(nodes, false, sort);
      }


      public override void DisplayPropertiesChanged(ITab<UIntPtr> nodes)
      {
         NodePropertySorter sorter = this.tree.NodeSorter as NodePropertySorter;
         Boolean sort = sorter != null && NodePropertyHelpers.IsDisplayProperty(sorter.Property);
         this.treeMode.InvalidateTreeNodes(nodes, false, sort);
      }

      public override void RenderPropertiesChanged(ITab<UIntPtr> nodes)
      {
         NodePropertySorter sorter = this.tree.NodeSorter as NodePropertySorter;
         Boolean sort = sorter != null && NodePropertyHelpers.IsRenderProperty(sorter.Property);
         this.treeMode.InvalidateTreeNodes(nodes, false, false);
      }
   }

   #endregion


   #region Tree events

   protected virtual void tree_SelectionChanged(object sender, SelectionChangedEventArgs e)
   {
      this.UnregisterSystemNotification(proc_SelectionsetChanged, SystemNotificationCode.SelectionsetChanged);

      IEnumerable<IMaxNodeWrapper> selNodes = HelperMethods.GetMaxNodes(e.Nodes);
      SelectCommand cmd = new SelectCommand(selNodes);
      cmd.Execute(true);

      this.RegisterSystemNotification(proc_SelectionsetChanged, SystemNotificationCode.SelectionsetChanged);
   }

   protected virtual void tree_BeforeNodeTextEdit(object sender, BeforeNodeTextEditEventArgs e)
   {
      IMaxNodeWrapper node = HelperMethods.GetMaxNode(e.TreeNode);
      if (node == null)
      {
         e.Cancel = true;
         return;
      }

      if (!node.CanEditName)
      {
         WinForms::MessageBox.Show( Tree
                                  , OutlinerResources.Warning_CannotEditName
                                  , OutlinerResources.Warning_CannotEditNameTitle
                                  , WinForms::MessageBoxButtons.OK
                                  , WinForms::MessageBoxIcon.Warning);
         e.Cancel = true;
         return;
      }

      e.EditText = node.Name;

      MaxInterfaces.Global.DisableAccelerators();
   }

   protected virtual void tree_AfterNodeTextEdit(object sender, AfterNodeTextEditEventArgs e)
   {
      IMaxNodeWrapper node = HelperMethods.GetMaxNode(e.TreeNode);
      if (node == null)
         return;

      if (e.NewText != e.OldText)
      {
         RenameCommand cmd = new RenameCommand(new List<IMaxNodeWrapper>(1) { node }, e.NewText);
         cmd.Execute(false);
      }

      //Note: setting treenode text to displayname and sorting are
      //      handled by nodenamechanged callback.

      MaxInterfaces.Global.EnableAccelerators();
   }



   protected virtual WinForms::ContextMenuStrip CreateContextMenu(TreeNode clickedTn)
   {
      IEnumerable<IMaxNodeWrapper> selectedNodes = HelperMethods.GetMaxNodes(this.Tree.SelectedNodes);
      
      String contextMenuFile = Path.Combine(OutlinerPaths.ContextMenuDir, "ContextMenu.xml");
      ContextMenuData data = XmlSerializationHelpers<ContextMenuData>.FromXml(contextMenuFile);

      return data.ToContextMenuStrip(clickedTn, selectedNodes);
   }


   void tree_MouseClick(object sender, WinForms.MouseEventArgs e)
   {
      if ((e.Button & WinForms.MouseButtons.Right) != WinForms.MouseButtons.Right)
         return;

      TreeNode clickedNode = this.Tree.GetNodeAt(e.Location);
      OutlinerSplitContainer container = this.Tree.Parent.Parent as OutlinerSplitContainer;

      WinForms::ToolStripDropDown strip = StandardContextMenu.Create(this.CreateContextMenu(clickedNode), container, this.Tree, this);
      strip.Show(this.Tree, e.Location);
   }

   #endregion


   #region Filters

   public MaxNodeFilterCombinator Filters
   {
      get { return this.filters.Filters[OtherFiltersIndex] as MaxNodeFilterCombinator; }
      set
      {
         ExceptionHelpers.ThrowIfArgumentIsNull(value, "value");

         this.filters.Filters[OtherFiltersIndex] = value;

         if (this.started)
            this.EvaluateFilters();
      }
   }

   public MaxNodeFilterCombinator PermanentFilter
   {
      get { return this.filters.Filters[PermanentFiltersIndex] as MaxNodeFilterCombinator; }
      set
      {
         ExceptionHelpers.ThrowIfArgumentIsNull(value, "value");

         this.filters.Filters[PermanentFiltersIndex] = value;

         if (this.started)
            this.EvaluateFilters();
      }
   }

   void filters_FilterChanged(object sender, EventArgs e)
   {
      if (this.started)
         this.EvaluateFilters();
   }

   /// <summary>
   /// Evaluates the filters and adds/removes treenodes based on it.
   /// </summary>
   public void EvaluateFilters()
   {
      foreach (KeyValuePair<Object, List<TreeNode>> item in this.treeNodes)
      {
         foreach (TreeNode tn in item.Value)
         {
            IMaxNodeWrapper node = HelperMethods.GetMaxNode(tn);
            tn.ShowNode = this.filters.ShowNode(node);
         }
      }
      this.Tree.Sort();
   }

   #endregion
}
}

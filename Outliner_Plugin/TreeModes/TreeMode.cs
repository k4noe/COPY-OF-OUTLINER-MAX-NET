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

namespace Outliner.TreeModes
{
public abstract class TreeMode
{
   protected TreeView tree { get; private set; }
   protected Autodesk.Max.IInterface ip { get; private set; }
   protected List<KeyValuePair<GlobalDelegates.Delegate5, SystemNotificationCode>> systemNotifications;
   protected List<KeyValuePair<uint, TreeModeNodeEventCallbacks>> nodeEventCallbacks;
   protected Dictionary<Object, TreeNode> treeNodes { get; private set; }

   protected TreeMode(TreeView tree, Autodesk.Max.IInterface ip)
   {
      this.tree = tree;
      this.ip = ip;
      this.treeNodes = new Dictionary<Object, TreeNode>();
      this.Filters = new FilterCollection<IMaxNodeWrapper>();
      this.Filters.Add(new InvisibleNodeFilter());

      this.tree.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(tree_SelectionChanged);
      this.tree.BeforeNodeTextEdit += new EventHandler<BeforeNodeTextEditEventArgs>(tree_BeforeNodeTextEdit);
      this.tree.AfterNodeTextEdit += new EventHandler<AfterNodeTextEditEventArgs>(tree_AfterNodeTextEdit);

      this.RegisterSystemNotifications();
      this.RegisterNodeEventCallbacks();
   }

   public abstract void FillTree();


   #region Register SystemNotifications and NodeEventCallbacks

   /// <summary>
   /// Registers a SystemNotification proc, which will be automatically unregistered when <see cref="UnregisterSystemNotifications"/> is called.
   /// </summary>
   protected virtual void RegisterSystemNotification(GlobalDelegates.Delegate5 proc, SystemNotificationCode code)
   {
      if (this.systemNotifications == null)
         this.systemNotifications = new List<KeyValuePair<GlobalDelegates.Delegate5, SystemNotificationCode>>();

      MaxInterfaces.Global.RegisterNotification(proc, null, code);
      this.systemNotifications.Add(new KeyValuePair<GlobalDelegates.Delegate5, SystemNotificationCode>(proc, code));
   }

   /// <summary>
   /// Registers the default SystemNotifications.
   /// </summary>
   public virtual void RegisterSystemNotifications()
   {
      this.RegisterSystemNotification(this.SystemPreNew, SystemNotificationCode.SystemPreNew);
      this.RegisterSystemNotification(this.SystemPostNew, SystemNotificationCode.SystemPostNew);
      this.RegisterSystemNotification(this.SystemPreReset, SystemNotificationCode.SystemPreReset);
      this.RegisterSystemNotification(this.SystemPostReset, SystemNotificationCode.SystemPostReset);
      this.RegisterSystemNotification(this.FilePreOpen, SystemNotificationCode.FilePreOpen);
      this.RegisterSystemNotification(this.FilePostOpen, SystemNotificationCode.FilePostOpen);
      this.RegisterSystemNotification(this.SelectionsetChanged, SystemNotificationCode.SelectionsetChanged);
   }

   /// <summary>
   /// Unregisters all SystemNotifications registered using <see cref="RegisterSystemNotification"/>.
   /// </summary>
   public virtual void UnregisterSystemNotifications()
   {
      if (this.systemNotifications == null)
         return;

      foreach (KeyValuePair<GlobalDelegates.Delegate5, SystemNotificationCode> notif in this.systemNotifications)
         MaxInterfaces.Global.UnRegisterNotification(notif.Key, null, notif.Value);

      this.systemNotifications.Clear();
      this.systemNotifications = null;
   }


   /// <summary>
   /// Registers a NodeEventCallback object, which will be automatically unregistered when <see cref="UnregisterNodeEventCallbacks"/> is called.
   /// </summary>
   protected virtual void RegisterNodeEventCallbackObject(TreeModeNodeEventCallbacks cb)
   {
      if (nodeEventCallbacks == null)
         this.nodeEventCallbacks = new List<KeyValuePair<uint, TreeModeNodeEventCallbacks>>();
      
      IISceneEventManager sceneEventMgr = MaxInterfaces.Global.ISceneEventManager;
      uint cbKey = sceneEventMgr.RegisterCallback(cb, false, 100, true);

      this.nodeEventCallbacks.Add(new KeyValuePair<uint, TreeModeNodeEventCallbacks>(cbKey, cb));
   }

   /// <summary>
   /// Registers the default NodeEventCallbacks.
   /// </summary>
   public virtual void RegisterNodeEventCallbacks()
   {
      this.RegisterNodeEventCallbackObject(new DefaultNodeEventCallbacks(this));
   }

   /// <summary>
   /// Unregisters all NodeEventCallbacks registered using <see cref="RegisterNodeEventCallbackObject"/>.
   /// </summary>
   public virtual void UnregisterNodeEventCallbacks()
   {
      if (this.nodeEventCallbacks == null)
         return;

      IISceneEventManager sceneEventMgr = MaxInterfaces.Global.ISceneEventManager;
      foreach (KeyValuePair<uint, TreeModeNodeEventCallbacks> cb in this.nodeEventCallbacks)
      {
         sceneEventMgr.UnRegisterCallback(cb.Key);
         cb.Value.Dispose();
      }
      this.nodeEventCallbacks.Clear();
      this.nodeEventCallbacks = null;
   }

   protected abstract class TreeModeNodeEventCallbacks : Autodesk.Max.Plugins.INodeEventCallback
   {
      protected TreeMode treeMode;
      protected TreeView tree { get { return this.treeMode.tree; } }
      protected Dictionary<Object, TreeNode> treeNodes { get { return this.treeMode.treeNodes; } }

      public TreeModeNodeEventCallbacks(TreeMode treeMode)
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
   
   public virtual TreeNode CreateTreeNode(IMaxNodeWrapper node)
   {
      if (node == null)
         return null;

      MaxTreeNode tn = new MaxTreeNode(node);
      return tn;
   }

   public virtual TreeNode GetTreeNode(Object node)
   {
      TreeNode tn = null;
      if (node != null)
         this.treeNodes.TryGetValue(node, out tn);
      return tn;
   }

   public virtual TreeNode AddNode(Object node, TreeNodeCollection parentCol)
   {
      if (node == null || parentCol == null)
         return null;

      TreeNode tn = null;
      IMaxNodeWrapper wrapper = null;
      if (this.treeNodes.TryGetValue(node, out tn))
         wrapper = HelperMethods.GetMaxNode(tn);
      else
      {
         wrapper = IMaxNodeWrapper.Create(node);
         tn = this.CreateTreeNode(wrapper);
         this.treeNodes.Add(node, tn);
      }

      tn.FilterResult = this.Filters.ShowNode(wrapper);
      if (tn.FilterResult != FilterResults.Hide)
         parentCol.Add(tn);

      if (wrapper.Selected)
         this.tree.SelectNode(tn, true);

      return tn;
   }


   public virtual void RemoveTreeNode(Object node)
   {
      TreeNode tn = this.GetTreeNode(node);
      if (tn != null)
      {
         this.tree.SelectedNodes.Remove(tn);
         tn.Remove();
         this.treeNodes.Remove(node);
      }
   }


   public void ClearTreeNodes()
   {
      this.tree.Nodes.Clear();
      this.treeNodes.Clear();
   }


   public virtual void InvalidateTreeNodes(ITab<UIntPtr> nodes, Boolean invalidateBounds, Boolean sort)
   {
      foreach (IINode node in nodes.NodeKeysToINodeList())
      {
         TreeNode tn = this.GetTreeNode(node);
         if (tn != null)
         {
            if (invalidateBounds)
               tn.InvalidateBounds(false, false);

            tn.Invalidate();

            if (sort)
               this.tree.AddToSortQueue(tn);
         }
      }

      if (sort)
         this.tree.StartTimedSort(true);
   }


   #endregion


   #region System notifications

   public virtual void SelectionsetChanged(IntPtr param, IntPtr info)
   {
      this.tree.SelectAllNodes(false);

      Int32 selNodeCount = this.ip.SelNodeCount;
      if (selNodeCount > 0)
      {
         for (Int32 i = 0; i < selNodeCount; i++)
         {
            TreeNode tn;
            if (this.treeNodes.TryGetValue(ip.GetSelNode(i), out tn))
               this.tree.SelectNode(tn, true);
         }
      }
   }

   public virtual void SystemPreNew(IntPtr param, IntPtr info)
   {
      this.UnregisterNodeEventCallbacks();
      this.ClearTreeNodes();
   }

   public virtual void SystemPostNew(IntPtr param, IntPtr info)
   {
      this.RegisterNodeEventCallbacks();
      this.FillTree();
   }

   public virtual void SystemPreReset(IntPtr param, IntPtr info)
   {
      this.UnregisterNodeEventCallbacks();
      this.ClearTreeNodes();
   }

   public virtual void SystemPostReset(IntPtr param, IntPtr info)
   {
      this.RegisterNodeEventCallbacks();
      this.FillTree();
   }

   public virtual void FilePreOpen(IntPtr param, IntPtr info)
   {
      this.UnregisterNodeEventCallbacks();
      this.ClearTreeNodes();
   }

   public virtual void FilePostOpen(IntPtr param, IntPtr info)
   {
      this.RegisterNodeEventCallbacks();
      this.FillTree();
   }

   #endregion


   #region NodeEventCallbacks

   protected class DefaultNodeEventCallbacks : TreeModeNodeEventCallbacks
   {
      public DefaultNodeEventCallbacks(TreeMode treeMode) : base(treeMode) { }

      public override void Deleted(ITab<UIntPtr> nodes)
      {
         foreach (IINode node in nodes.NodeKeysToINodeList())
            this.treeMode.RemoveTreeNode(node);
      }

      public override void NameChanged(ITab<UIntPtr> nodes)
      {
         this.treeMode.InvalidateTreeNodes(nodes, true, true);
      }

      public override void DisplayPropertiesChanged(ITab<UIntPtr> nodes)
      {
         Boolean sort = this.tree.NodeSorter is NodeSorters.FrozenSorter
                      || this.tree.NodeSorter is NodeSorters.HiddenSorter;
         this.treeMode.InvalidateTreeNodes(nodes, false, sort);
      }

      public override void RenderPropertiesChanged(ITab<UIntPtr> nodes)
      {
         this.treeMode.InvalidateTreeNodes(nodes, false, false);
      }
   }

   #endregion


   #region Tree events

   void tree_SelectionChanged(object sender, SelectionChangedEventArgs e)
   {
      IEnumerable<IMaxNodeWrapper> selNodes = HelperMethods.GetMaxNodes(e.Nodes);
      OutlinerDescriptor.Instance.OpenSelectedGroupHeads(selNodes);
      SelectCommand cmd = new SelectCommand(selNodes);
      cmd.Execute(true);
   }

   void tree_BeforeNodeTextEdit(object sender, BeforeNodeTextEditEventArgs e)
   {
      IMaxNodeWrapper node = HelperMethods.GetMaxNode(e.TreeNode);
      if (node == null)
      {
         e.Cancel = true;
         return;
      }

      if (!node.CanEditName)
      {
         WinForms::MessageBox.Show(tree, 
                                   OutlinerResources.Warning_CannotEditName, 
                                   OutlinerResources.Warning_CannotEditNameTitle, 
                                   WinForms::MessageBoxButtons.OK, 
                                   WinForms::MessageBoxIcon.Warning);
         e.Cancel = true;
         return;
      }

      e.EditText = node.Name;
   }

   void tree_AfterNodeTextEdit(object sender, AfterNodeTextEditEventArgs e)
   {
      IMaxNodeWrapper node = HelperMethods.GetMaxNode(e.TreeNode);
      if (node == null)
         return;

      RenameCommand cmd = new RenameCommand(new List<IMaxNodeWrapper>(1) { node }, e.NewText);
      cmd.Execute(false);

      //Note: setting treenode text to displayname and sorting are
      //      handled by nodenamechanged callback.
   }

   #endregion


   #region Filters

   protected IEnumerable<IMaxNodeWrapper> GetChildNodes(IMaxNodeWrapper node)
   {
      if (node == null)
         return null;

      return node.WrappedChildNodes;
   }

   private FilterCollection<IMaxNodeWrapper> _filters;
   public FilterCollection<IMaxNodeWrapper> Filters
   {
      get { return _filters; }
      set
      {
         if (value == null)
            throw new ArgumentNullException("value");

         if (_filters != null)
         {
            _filters.FiltersEnabled -= this.filtersEnabled;
            _filters.FiltersCleared -= this.filtersCleared;
            _filters.FilterAdded -= this.filterAdded;
            _filters.FilterRemoved -= this.filterRemoved;
            _filters.FilterChanged -= this.filterChanged;
         }

         _filters = value;
         _filters.GetChildNodesFn = this.GetChildNodes;

         _filters.FiltersEnabled += this.filtersEnabled;
         _filters.FiltersCleared += this.filtersCleared;
         _filters.FilterAdded += this.filterAdded;
         _filters.FilterRemoved += this.filterRemoved;
         _filters.FilterChanged += this.filterChanged;
      }
   }

   private void filtersEnabled(object sender, EventArgs e)
   {

   }
   private void filtersCleared(object sender, EventArgs e)
   {

   }
   private void filterAdded(object sender, FilterChangedEventArgs<IMaxNodeWrapper> e)
   {

   }
   private void filterRemoved(object sender, FilterChangedEventArgs<IMaxNodeWrapper> e)
   {

   }
   private void filterChanged(object sender, FilterChangedEventArgs<IMaxNodeWrapper> e)
   {

   }

   #endregion
}
}

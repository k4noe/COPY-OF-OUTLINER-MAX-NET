﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;
using Outliner.Controls.FiltersBase;
using Outliner.Scene;
using Outliner.Commands;
using Outliner.Controls;
// Import System.Windows.Forms with alias to avoid ambiguity 
// between System.Windows.TreeNode and Outliner.Controls.TreeNode.
using WinForms = System.Windows.Forms;

namespace Outliner.TreeModes
{
public abstract class TreeMode : Autodesk.Max.Plugins.INodeEventCallback
{
   protected Outliner.Controls.TreeView tree;
   protected Autodesk.Max.IInterface ip;
   private uint sceneEventCbKey;

   protected Dictionary<Object, TreeNode> nodes;

   public TreeMode(Outliner.Controls.TreeView tree, Autodesk.Max.IInterface ip)
   {
      this.tree = tree;
      this.ip = ip;
      this.nodes = new Dictionary<Object, TreeNode>();
      this.Filters = new FilterCollection<IMaxNodeWrapper>();

      this.tree.SelectionChanged += new SelectionChangedEventHandler(tree_SelectionChanged);
      this.tree.AfterNodeTextEdit += new AfterNodeTextEditEventHandler(tree_AfterNodeTextEdit);
      //this.tree.AfterLabelEdit += new NodeLabelEditEventHandler(tree_AfterLabelEdit);

      IGlobal iGlobal = GlobalInterface.Instance;
      iGlobal.RegisterNotification(PostReset, null, SystemNotificationCode.SystemPostReset);
      iGlobal.RegisterNotification(SelectionsetChanged, null, SystemNotificationCode.SelectionsetChanged);

      this.sceneEventCbKey = iGlobal.ISceneEventManager.RegisterCallback(this, false, 100, true);
   }


   /// <summary>
   /// Cleanup of event notifications and callbacks.
   /// </summary>
   public void Unregister()
   {
      IGlobal iGlobal = GlobalInterface.Instance;
      iGlobal.UnRegisterNotification(PostReset, null);
      iGlobal.UnRegisterNotification(SelectionsetChanged, null);

      iGlobal.ISceneEventManager.UnRegisterCallback(this.sceneEventCbKey);

      base.Dispose();
   }

   public abstract void FillTree();


   #region Helper methods
      
   public virtual TreeNode GetTreeNode(Object node)
   {
      TreeNode tn = null;
      if (node != null)
         this.nodes.TryGetValue(node, out tn);
      return tn;
   }

   public virtual void RemoveTreeNode(Object node)
   {
      TreeNode tn = this.GetTreeNode(node);
      if (tn != null)
      {
         this.tree.SelectedNodes.Remove(tn);
         tn.Remove();
         this.nodes.Remove(node);
      }
   }

   #endregion


   #region NodeEvent Callbacks

   public override void CallbackBegin()
   {
      this.tree.BeginUpdate();
   }

   public override void CallbackEnd()
   {
      this.tree.EndUpdate();
   }

   public override void Deleted(ITab<UIntPtr> nodes)
   {
      foreach (IINode node in nodes.NodeKeysToINodeList())
         this.RemoveTreeNode(node);
   }

   public override void NameChanged(ITab<UIntPtr> nodes)
   {
      foreach (IINode node in nodes.NodeKeysToINodeList())
      {
         TreeNode tn = this.GetTreeNode(node);
         if (tn != null)
         {
            tn.Text = node.Name;
            this.tree.AddToSortQueue(tn);
         }
      }
      this.tree.TimedSort(true);
   }

   public override void DisplayPropertiesChanged(ITab<UIntPtr> nodes)
   {
      foreach (IINode node in nodes.NodeKeysToINodeList())
         this.tree.InvalidateTreeNode(this.GetTreeNode(node));
   }

   #endregion


   #region System notifications

   public virtual void SelectionsetChanged(IntPtr paramPtr, IntPtr infoPtr)
   {
      this.tree.SelectAllNodes(false);

      Int32 selNodeCount = this.ip.SelNodeCount;
      if (selNodeCount > 0)
      {
         for (Int32 i = 0; i < selNodeCount; i++)
         {
            TreeNode tn;
            if (this.nodes.TryGetValue(ip.GetSelNode(i), out tn))
               this.tree.SelectNode(tn, true);
         }
      }
   }

   public virtual void PostReset(IntPtr paramPtr, IntPtr infoPtr)
   {
      this.tree.Nodes.Clear();
      this.nodes.Clear();
   }

   #endregion


   #region Tree events

   void tree_SelectionChanged(object sender, Controls.SelectionChangedEventArgs e)
   {
      SelectCommand cmd = new SelectCommand(HelperMethods.GetMaxNodes(e.Nodes));
      cmd.Execute(true);
   }

   void tree_AfterNodeTextEdit(object sender, AfterNodeTextEditEventArgs e)
   {
      IMaxNodeWrapper node = HelperMethods.GetMaxNode(e.TreeNode);
      if (node == null)
         return;

      RenameCommand cmd = new RenameCommand(new List<IMaxNodeWrapper>(1) { node }, e.NewText);
      cmd.Execute(false);

      //Note: sort is handled by nodenamechanged callback.
   }

   #endregion


   #region Filters

   protected IEnumerable<IMaxNodeWrapper> GetChildNodes(IMaxNodeWrapper n)
   {
      return n.ChildNodes;
   }

   private FilterCollection<IMaxNodeWrapper> _filters;
   public FilterCollection<IMaxNodeWrapper> Filters
   {
      get { return _filters; }
      set
      {
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
   private void filterAdded(object sender, NodeFilterChangedEventArgs<IMaxNodeWrapper> e)
   {

   }
   private void filterRemoved(object sender, NodeFilterChangedEventArgs<IMaxNodeWrapper> e)
   {

   }
   private void filterChanged(object sender, NodeFilterChangedEventArgs<IMaxNodeWrapper> e)
   {

   }

   #endregion
}
}

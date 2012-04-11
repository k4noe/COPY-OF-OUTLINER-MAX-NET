﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outliner.Controls.Tree;

namespace Outliner.NodeSorters
{
   public abstract class NodeSorter : IComparer<TreeNode>
   {
      public NodeSorter() : this(false) { }
      public NodeSorter(Boolean invert)
      {
         this.invert = invert;
      }

      private Boolean invert;
      public Boolean Ascending { get { return !this.invert; } }
      public Boolean Descending { get { return this.invert; } }

      public int Compare(TreeNode x, TreeNode y)
      {
         if (this.invert)
         {
            int c = this.InternalCompare(x, y);
            if (c == 0)
               return 0;
            else if (c > 0)
               return -1;
            else
               return 1;
         }
         else
            return this.InternalCompare(x, y);
      }

      protected abstract int InternalCompare(TreeNode x, TreeNode y);
   }
}
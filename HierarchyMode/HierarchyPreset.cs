﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outliner.Controls.Tree;
using Outliner.NodeSorters;
using System.Drawing;
using Outliner.Controls.Tree.Layout;
using Outliner.Filters;
using Outliner.Scene;
using Outliner.Plugins;
using Outliner.Presets;
using Outliner.TreeNodeButtons;

namespace Outliner.Modes.Hierarchy
{
[OutlinerPlugin(OutlinerPluginType.DefaultPreset)]
public class HierarchyPreset : OutlinerPreset
{
   public HierarchyPreset()
   {
      this.TextRes = Resources.Preset_DisplayName;
      this.ResourceTypeName = typeof(Resources).FullName;
      this.Image16Res = "hierarchy_mode_16";
      this.Image24Res = "hierarchy_mode_24";

      this.TreeModeTypeName = typeof(HierarchyMode).FullName;
      this.NodeSorter = new AlphabeticalSorter();
      this.Filters = new MaxNodeFilterCombinator();

      this.TreeNodeLayout = new TreeNodeLayout();
      this.TreeNodeLayout.ItemHeight = 20;
      this.TreeNodeLayout.FullRowSelect = true;
      this.TreeNodeLayout.LayoutItems.Add(new ExpandButton() { UseVisualStyles = false });
      this.TreeNodeLayout.LayoutItems.Add(new NodeIcon(IconSet.Maya, false));
      this.TreeNodeLayout.LayoutItems.Add(new MayaStyleIndent());
      this.TreeNodeLayout.LayoutItems.Add(new TreeNodeText());
      this.TreeNodeLayout.LayoutItems.Add(new EmptySpace());
      this.TreeNodeLayout.LayoutItems.Add(new HideButton());
      this.TreeNodeLayout.LayoutItems.Add(new FreezeButton());
   }
}
}

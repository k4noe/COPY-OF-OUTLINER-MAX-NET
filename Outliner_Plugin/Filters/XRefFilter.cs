﻿using Autodesk.Max;
using Outliner.Controls.FiltersBase;
using Outliner.Scene;
using Outliner.TreeModes;

namespace Outliner.Filters
{
   public class XRefFilter : NodeFilter<IMaxNodeWrapper>
   {
      override public FilterResult ShowNode(IMaxNodeWrapper data)
      {
         //TODO: move to HelperMethods.IsXref
         if (HelperMethods.ClassIDEquals(data.ClassID, BuiltInClassIDA.XREFATMOS_CLASS_ID, BuiltInClassIDB.XREFATMOS_CLASS_ID)
            || HelperMethods.ClassIDEquals(data.ClassID, BuiltInClassIDA.XREFCTRL_CLASS_ID, BuiltInClassIDB.XREFCTRL_CLASS_ID)
            || HelperMethods.ClassIDEquals(data.ClassID, BuiltInClassIDA.XREFMATERIAL_CLASS_ID, BuiltInClassIDB.XREFMATERIAL_CLASS_ID)
            || HelperMethods.ClassIDEquals(data.ClassID, BuiltInClassIDA.XREFOBJ_CLASS_ID))
            return FilterResult.Hide;
         else
            return FilterResult.Show;
      }
   }
}

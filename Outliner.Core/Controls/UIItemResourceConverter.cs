﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Outliner.Controls
{
   public abstract class UIItemResourceConverter : TypeConverter
   {
      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
         return false;
      }

      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
         return true;
      }

      public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
         List<String> standardValues = new List<string>();

         UIItemModel itemModel = context.Instance as UIItemModel;
         if (itemModel != null)
         {
            Type resourceType = itemModel.ResourceType;
            if (resourceType != null)
            {
               standardValues.AddRange(resourceType.GetProperties(BindingFlags.Static | BindingFlags.NonPublic)
                                                   .Where(p => IsValidResource(p))
                                                   .Select(p => p.Name));
            }
         }

         return new StandardValuesCollection(standardValues);
      }

      protected abstract Boolean IsValidResource(PropertyInfo property);
   }
}
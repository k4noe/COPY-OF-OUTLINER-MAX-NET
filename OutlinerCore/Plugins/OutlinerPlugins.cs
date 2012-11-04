﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Outliner.Filters;
using Outliner.Scene;
using Outliner.NodeSorters;
using Outliner.Modes;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Autodesk.Max;
using Outliner.MaxUtils;

namespace Outliner.Plugins
{
public static class OutlinerPlugins
{
   internal const OutlinerPluginType PluginTypeAll = (OutlinerPluginType)0xFF;
   internal const OutlinerPluginType PluginTypeNone = (OutlinerPluginType)0x00;
   private const String PluginExtension = "dll";
   private const String PluginSearchPattern = "*." + PluginExtension;

   private static IEnumerable<Assembly> pluginAssemblies;
   private static List<OutlinerPluginData> plugins;


   /// <summary>
   /// The collection of assemblies from which plugins have been loaded.
   /// </summary>
   public static IEnumerable<Assembly> PluginAssemblies
   {
      get
      {
         if (pluginAssemblies == null)
         {
            List<Assembly> assemblies = new List<Assembly>();

            //Add own assembly.
            assemblies.Add(Assembly.GetAssembly(typeof(OutlinerPlugins)));

            String pluginDir = OutlinerPaths.PluginsDir;

            AppDomain domain = AppDomain.CurrentDomain;


            //Add plugin assemblies.
            if (Directory.Exists(pluginDir))
            {
               String[] pluginFiles = Directory.GetFiles(pluginDir
                                                        , OutlinerPlugins.PluginSearchPattern
                                                        , SearchOption.AllDirectories);
               foreach (String pluginFile in pluginFiles)
               {
                  assemblies.Add(Assembly.LoadFile(pluginFile));
               }
            }

            pluginAssemblies = assemblies;
         }

         return pluginAssemblies;
      }
   }
   
   /// <summary>
   /// (Re)loads all plugins from the <see cref="PluginDirectory"/>.
   /// </summary>
   /// <returns></returns>
   public static List<OutlinerPluginData> LoadPlugins()
   {
      AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(CurrentDomain_AssemblyResolve);
      AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

      List<OutlinerPluginData> plugins = new List<OutlinerPluginData>();

      foreach (Assembly assembly in OutlinerPlugins.PluginAssemblies)
      {
         IEnumerable<Type> pluginClasses = assembly.GetExportedTypes().Where(t => t.IsPublic && (!t.IsAbstract || t.IsSealed));

         foreach (Type pluginClass in pluginClasses)
         {
            OutlinerPluginType pluginType = PluginTypeNone;
            OutlinerPluginAttribute pluginAttr = TypeHelpers.GetAttribute<OutlinerPluginAttribute>(pluginClass);
            if (pluginAttr != null)
            {
               pluginType = pluginAttr.PluginType;
            }

            if ((pluginType & PluginTypeAll) == 0)
               continue;

            StartPlugin(pluginClass);

            String name = pluginClass.Name;
            DisplayNameAttribute dispAtrr = TypeHelpers.GetAttribute<DisplayNameAttribute>(pluginClass);
            if (dispAtrr != null)
               name = dispAtrr.DisplayName;

            Image imgSmall = null;
            Image imgLarge = null;
            LocalizedDisplayImageAttribute imgAttr = TypeHelpers.GetAttribute<LocalizedDisplayImageAttribute>(pluginClass);
            if (imgAttr != null)
            {
               imgSmall = imgAttr.DisplayImageSmall;
               imgLarge = imgAttr.DisplayImageLarge;
            }

            plugins.Add(new OutlinerPluginData(pluginType, pluginClass, name, imgSmall, imgLarge));
         }
      }

      plugins.Sort((pX, pY) => pX.DisplayName.CompareTo(pY.DisplayName));

      return plugins;
   }


   private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
   {
      return OutlinerPlugins.PluginAssemblies.FirstOrDefault(a => a.FullName == args.Name);
   }


   private static void StartPlugin(Type pluginClass)
   {
      MethodInfo[] methods = pluginClass.GetMethods(BindingFlags.Static | BindingFlags.Public);
      foreach (MethodInfo method in methods)
      {
         if(TypeHelpers.HasAttribute<OutlinerPluginStartAttribute>(method))
            method.Invoke(null, null);
      }
   }

   /// <summary>
   /// A collection of all loaded plugin metadata.
   /// </summary>
   public static IEnumerable<OutlinerPluginData> Plugins
   {
      get
      {
         if (plugins == null)
            plugins = OutlinerPlugins.LoadPlugins();

         return plugins;
      }
   }


   public static void RegisterPlugin(OutlinerPluginData pluginData)
   {
      if (!OutlinerPlugins.Plugins.Any(p => p.Type.Equals(pluginData.Type)))
         OutlinerPlugins.plugins.Add(pluginData);
   }


   /// <summary>
   /// Gets a collection of plugin metadata for plugins of the given type.
   /// </summary>
   /// <param name="type">The type of plugins to select.</param>
   public static IEnumerable<OutlinerPluginData> GetPluginsByType(OutlinerPluginType type)
   {
      return Plugins.Where(p => (p.PluginType & type) != 0);
   }


   public static OutlinerPluginData GetPluginDataByType(Type pluginType)
   {
      return Plugins.FirstOrDefault(p => p.Type == pluginType);
   }

   /// <summary>
   /// Gets the Type of the supplied plugin and typename.
   /// </summary>
   public static Type GetPluginType(OutlinerPluginType pluginType, String typeName)
   {
      IEnumerable<OutlinerPluginData> plugins = OutlinerPlugins.GetPluginsByType(pluginType);
      OutlinerPluginData plugin = plugins.FirstOrDefault(p => p.Type.Name == typeName || p.Type.FullName == typeName);
      return (plugin != null) ? plugin.Type : null;
   }

   /// <summary>
   /// Gets an array of Types that can be serialized.
   /// </summary>
   public static Type[] GetSerializableTypes()
   {
      return OutlinerPlugins.GetPluginsByType( OutlinerPluginType.Filter
                                             | OutlinerPluginType.NodeSorter
                                             | OutlinerPluginType.TreeNodeButton
                                             | OutlinerPluginType.DefaultPreset)
                            .Select(p => p.Type)
                            .ToArray();
   }

   /// <summary>
   /// Gets a collection of Filter plugins metadata for filters of the given category.
   /// </summary>
   /// <param name="category">The category of filter to look for.</param>
   internal static IEnumerable<OutlinerPluginData> GetFilterPlugins(FilterCategories category)
   {
      IEnumerable<OutlinerPluginData> pluginTypes = GetPluginsByType(OutlinerPluginType.Filter);
      
      List<OutlinerPluginData> filters = new List<OutlinerPluginData>();
      foreach (OutlinerPluginData plugin in pluginTypes)
      {
         FilterCategoryAttribute categoryAttr = TypeHelpers.GetAttribute<FilterCategoryAttribute>(plugin.Type);
         if (categoryAttr != null && (categoryAttr.Category & category) == category)
            filters.Add(plugin);
      }

      return filters;
   }
}
}

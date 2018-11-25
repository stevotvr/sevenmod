﻿// <copyright file="PluginManager.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using SevenMod.Console;
    using SevenMod.ConVar;

    /// <summary>
    /// Manages plugins.
    /// </summary>
    public class PluginManager
    {
        /// <summary>
        /// The currently active plugins.
        /// </summary>
        private static readonly Dictionary<string, PluginAbstract> Plugins = new Dictionary<string, PluginAbstract>();

        /// <summary>
        /// Gets a list of the currently active plugins.
        /// </summary>
        public static List<PluginAbstract> ActivePlugins { get => new List<PluginAbstract>(Plugins.Values); }

        /// <summary>
        /// Gets the metadata for a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <returns>The <see cref="PluginAbstract.PluginInfo"/> object containing the metadata for the plugin, or <c>null</c> if the plugin is not loaded.</returns>
        public static PluginAbstract.PluginInfo? GetPluginInfo(string name)
        {
            name = name.ToLower();
            if (Plugins.ContainsKey(name))
            {
                return Plugins[name].Info;
            }

            return null;
        }

        /// <summary>
        /// Loads a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        public static void Load(string name)
        {
            name = name.ToLower();
            if (Plugins.ContainsKey(name))
            {
                return;
            }

            var parentType = Type.GetType("SevenMod.Core.PluginAbstract");
            var dll = Assembly.LoadFile($"{SMPath.Plugins}{name}.dll");
            try
            {
                var type = dll.GetType($"SevenMod.Plugin.{name}.{name}", true, true);
                if (type.IsSubclassOf(parentType))
                {
                    var plugin = Activator.CreateInstance(type) as PluginAbstract;
                    plugin.LoadPlugin();
                    plugin.GameAwake();
                    plugin.ReloadAdmins();
                    if (ConVarManager.ConfigsLoaded)
                    {
                        ConVarManager.ExecuteConfigs(plugin);
                        plugin.ConfigsExecuted();
                    }

                    Plugins.Add(name, plugin);
                    Log.Out("Added {0}", type.Name);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        /// <summary>
        /// Loads the enabled plugins.
        /// </summary>
        public static void Refresh()
        {
            var files = Directory.GetFiles(SMPath.Plugins, "*.dll");
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                Load(name);
            }

            if (!ConVarManager.ConfigsLoaded)
            {
                ConVarManager.ExecuteConfigs();
                foreach (var plugin in Plugins.Values)
                {
                    plugin.ConfigsExecuted();
                }
            }
        }

        /// <summary>
        /// Reloads a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        public static void Reload(string name)
        {
            Unload(name);
            Load(name);
        }

        /// <summary>
        /// Unloads a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        public static void Unload(string name)
        {
            name = name.ToLower();
            if (Plugins.ContainsKey(name))
            {
                Plugins[name].UnloadPlugin();
                AdminCommandManager.UnloadPlugin(Plugins[name]);
                ConVarManager.UnloadPlugin(Plugins[name]);
                Plugins.Remove(name);
            }
        }

        /// <summary>
        /// Unloads all plugins.
        /// </summary>
        public static void UnloadAll()
        {
            foreach (var plugin in Plugins.Values)
            {
                plugin.UnloadPlugin();
                AdminCommandManager.UnloadPlugin(plugin);
                ConVarManager.UnloadPlugin(plugin);
            }

            Plugins.Clear();
        }
    }
}

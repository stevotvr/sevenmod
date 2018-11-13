﻿// <copyright file="ConfigManager.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;

    /// <summary>
    /// Manages the core configuration and provides utilities for plugins to manage their own
    /// configuration files.
    /// </summary>
    public class ConfigManager
    {
        /// <summary>
        /// Initializes the <see cref="ConfigManager"/> class.
        /// </summary>
        public static void Init()
        {
            ParseConfig(CoreConfig.Instance, "Core");
        }

        /// <summary>
        /// Parses a configuration file.
        /// </summary>
        /// <param name="container">An object with properties corresponding to the configuration
        /// directives defined in the XML.</param>
        /// <param name="configName">The base name of the configuration XML file.</param>
        public static void ParseConfig(object container, string configName)
        {
            var xml = new XmlDocument();
            try
            {
                xml.Load($"{SMPath.Config}{configName}.xml");
            }
            catch
            {
                Log.Error($"[SevenMod] Failed to load {configName}.xml");
            }

            foreach (XmlElement element in xml.GetElementsByTagName("Option"))
            {
                if (!element.HasAttribute("Name") || !element.HasAttribute("Value"))
                {
                    continue;
                }

                var name = element.GetAttribute("Name");
                var prop = container.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if ((prop == null) || !prop.CanWrite)
                {
                    continue;
                }

                var value = Convert.ChangeType(element.GetAttribute("Value"), prop.PropertyType);
                prop.SetValue(container, value, null);
            }

            foreach (XmlElement element in xml.GetElementsByTagName("List"))
            {
                if (!element.HasAttribute("Name"))
                {
                    continue;
                }

                string name = element.GetAttribute("Name");
                var prop = container.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if ((prop == null) || !prop.CanRead || (prop.PropertyType != typeof(List<string>)))
                {
                    continue;
                }

                var list = (List<string>)prop.GetValue(container, null);
                foreach (XmlElement item in element.GetElementsByTagName("Item"))
                {
                    list.Add(item.InnerText);
                }
            }
        }
    }
}

﻿// <copyright file="BaseBans.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseBans
{
    using SevenMod.Admin;
    using SevenMod.Console;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that adds the ban, addban, and unban admin commands.
    /// </summary>
    public class BaseBans : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Basic Ban Commands",
            Author = "SevenMod",
            Description = "Adds basic banning commands.",
            Version = "0.1.0.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.LoadTranslations("BaseBans.Plugin");

            this.RegAdminCmd("addban", AdminFlags.Ban, "Addban Description").Executed += this.OnAddbanCommandExecuted;
            this.RegAdminCmd("ban", AdminFlags.Ban, "Ban Description").Executed += this.OnBanCommandExecuted;
            this.RegAdminCmd("unban", AdminFlags.Unban, "Unban Description").Executed += this.OnUnbanCommandExecuted;
        }

        /// <summary>
        /// Called when the addban admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnAddbanCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                this.ReplyToCommand(e.Client, "Not enough parameters");
                return;
            }

            if (!ConsoleHelper.ParseParamSteamIdValid(e.Arguments[0]))
            {
                this.ReplyToCommand(e.Client, "Invalid player ID");
                return;
            }

            SdtdConsole.Instance.ExecuteSync($"ban add {e.Arguments[0]}", null);
        }

        /// <summary>
        /// Called when the ban admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnBanCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 2)
            {
                this.ReplyToCommand(e.Client, "Not enough parameters");
                return;
            }

            if (!uint.TryParse(e.Arguments[1], out var duration))
            {
                this.ReplyToCommand(e.Client, "Invalid ban duration");
                return;
            }

            if (this.ParseSingleTargetString(e.Client, e.Arguments[0], out var target))
            {
                var unit = "minutes";
                if (duration == 0)
                {
                    unit = "years";
                    duration = 999999;
                }

                SdtdConsole.Instance.ExecuteSync($"ban add {target.PlayerId} {duration} {unit}", null);
            }
        }

        /// <summary>
        /// Called when the unban admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnUnbanCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                this.ReplyToCommand(e.Client, "Not enough parameters");
                return;
            }

            if (!ConsoleHelper.ParseParamSteamIdValid(e.Arguments[0]))
            {
                this.ReplyToCommand(e.Client, "Invalid player ID");
                return;
            }

            SdtdConsole.Instance.ExecuteSync($"ban remove {e.Arguments[0]}", null);
        }
    }
}

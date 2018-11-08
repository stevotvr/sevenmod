﻿// <copyright file="AdminCmdKick.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseCommands
{
    using System.Collections.Generic;
    using SevenMod.Core;

    /// <summary>
    /// <para>Admin Command: sm_kick</para>
    /// <para>Kicks a player from the server.</para>
    /// </summary>
    public class AdminCmdKick : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string[] GetCommands()
        {
            return new string[] { "sm_kick" };
        }

        /// <inheritdoc/>
        public override string GetDescription()
        {
            return "kicks a player from the server";
        }

        /// <inheritdoc/>
        public override void Exec(List<string> args, CommandSenderInfo senderInfo)
        {
            if (args.Count < 1)
            {
                this.ReplyToCommand(senderInfo, "Not enough parameters");
                return;
            }

            string playerId;
            ClientInfo target;
            if (ConsoleHelper.ParseParamPartialNameOrId(args[0], out playerId, out target) != 1)
            {
                this.ReplyToCommand(senderInfo, "No valid targets found");
                return;
            }

            if (!AdminManager.CanTarget(senderInfo.RemoteClientInfo, target))
            {
                this.ReplyToCommand(senderInfo, $"Cannot target {target.playerName}");
                return;
            }

            SdtdConsole.Instance.ExecuteSync($"kick {playerId}", null);
        }
    }
}

﻿// <copyright file="ChatHook.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Chat
{
    using System;
    using System.Collections.Generic;
    using SevenMod.ConVar;
    using SevenMod.Core;

    /// <summary>
    /// Hooks into the game chat and allows plugins to register their own chat hooks.
    /// </summary>
    public class ChatHook
    {
        /// <summary>
        /// Client entity IDs for which commands should reply to via game chat.
        /// </summary>
        private static List<int> replyToChat = new List<int>();

        /// <summary>
        /// The value of the PublicChatTrigger <see cref="ConVar"/>.
        /// </summary>
        private static ConVarValue publicChatTrigger;

        /// <summary>
        /// The value of the SilentChatTrigger <see cref="ConVar"/>.
        /// </summary>
        private static ConVarValue silentChatTrigger;

        /// <summary>
        /// Initializes the chat hook system.
        /// </summary>
        internal static void Init()
        {
            publicChatTrigger = ConVarManager.CreateConVar(null, "PublicChatTrigger", "!", "Chat prefix for public commands.").Value;
            silentChatTrigger = ConVarManager.CreateConVar(null, "SilentChatTrigger", "/", "Chat prefix for silent commands.").Value;
        }

        /// <summary>
        /// Processes a chat message.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client that sent the message.</param>
        /// <param name="message">The message text.</param>
        /// <returns><c>true</c> to allow the message to continue propagating; <c>false</c> to consume the message.</returns>
        internal static bool HookChatMessage(ClientInfo client, string message)
        {
            if (client == null)
            {
                return true;
            }

            foreach (var plugin in PluginManager.Plugins.Values)
            {
                if (plugin.LoadStatus == PluginContainer.Status.Loaded)
                {
                    try
                    {
                        if (!plugin.Plugin.ChatMessage(client, message))
                        {
                            return false;
                        }
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        plugin.SetFailState(e.Message);
                    }
                }
            }

            if (message.StartsWith(publicChatTrigger.AsString) || message.StartsWith(silentChatTrigger.AsString))
            {
                replyToChat.Add(client.entityId);
                ConnectionManager.Instance.ServerConsoleCommand(client, $"sm {message.Substring(1)}");
                replyToChat.Remove(client.entityId);

                return message.StartsWith(publicChatTrigger.AsString);
            }

            return true;
        }

        /// <summary>
        /// Checks whether commands should reply to a user via chat.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client for which to reply.</param>
        /// <returns><c>true</c> to reply via chat; <c>false</c> to reply via console.</returns>
        internal static bool ShouldReplyToChat(ClientInfo client)
        {
            return replyToChat.Contains(client.entityId);
        }
    }
}

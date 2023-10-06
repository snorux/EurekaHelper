﻿global using Dalamud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using XivCommon;

// From: https://github.com/UnknownX7

namespace Dalamud
{
    public class DalamudApi
    {
        [PluginService]
        //[RequiredVersion("1.0")]
        public static DalamudPluginInterface PluginInterface { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IBuddyList BuddyList { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IChatGui ChatGui { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IClientState ClientState { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static ICommandManager CommandManager { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static ICondition Condition { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IDataManager DataManager { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IFateTable FateTable { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IFlyTextGui FlyTextGui { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IFramework Framework { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IGameGui GameGui { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IGameNetwork GameNetwork { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IJobGauges JobGauges { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IKeyState KeyState { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static ILibcFunction LibcFunction { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IObjectTable ObjectTable { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IPartyFinderGui PartyFinderGui { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IPartyList PartyList { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static ISigScanner SigScanner { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static ITargetManager TargetManager { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IToastGui ToastGui { get; private set; }

        [PluginService]
        //[RequiredVersion("1.0")]
        public static IDtrBar DtrBar { get; private set; }

        [PluginService]
        public static IGameInteropProvider GameInteropProvider { get; private set; }
        
        public static XivCommonBase XivCommonBase { get; private set; }

        private static PluginCommandManager<IDalamudPlugin> pluginCommandManager;

        public DalamudApi() { }

        public DalamudApi(IDalamudPlugin plugin) => pluginCommandManager ??= new(plugin);

        public DalamudApi(IDalamudPlugin plugin, DalamudPluginInterface pluginInterface)
        {
            if (!pluginInterface.Inject(this))
            {
                PluginLog.LogError("Failed loading DalamudApi!");
                return;
            }

            pluginCommandManager ??= new(plugin);
            XivCommonBase = new XivCommonBase(pluginInterface);
        }

        public static DalamudApi operator +(DalamudApi container, object o)
        {
            foreach (var f in typeof(DalamudApi).GetProperties())
            {
                if (f.PropertyType != o.GetType()) continue;
                if (f.GetValue(container) != null) break;
                f.SetValue(container, o);
                return container;
            }
            throw new InvalidOperationException();
        }

        public static void Initialize(IDalamudPlugin plugin, DalamudPluginInterface pluginInterface) => _ = new DalamudApi(plugin, pluginInterface);

        public static void Dispose() => pluginCommandManager?.Dispose();
    }

    #region PluginCommandManager
    public class PluginCommandManager<T> : IDisposable where T : IDalamudPlugin
    {
        private readonly T plugin;
        private readonly (string, CommandInfo)[] pluginCommands;

        public PluginCommandManager(T p)
        {
            plugin = p;
            pluginCommands = plugin.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Where(method => method.GetCustomAttribute<CommandAttribute>() != null)
                .SelectMany(GetCommandInfoTuple)
                .ToArray();

            AddCommandHandlers();
        }

        private void AddCommandHandlers()
        {
            foreach (var (command, commandInfo) in pluginCommands)
                DalamudApi.CommandManager.AddHandler(command, commandInfo);
        }

        private void RemoveCommandHandlers()
        {
            foreach (var (command, _) in pluginCommands)
                DalamudApi.CommandManager.RemoveHandler(command);
        }

        private IEnumerable<(string, CommandInfo)> GetCommandInfoTuple(MethodInfo method)
        {
            var handlerDelegate = (CommandInfo.HandlerDelegate)Delegate.CreateDelegate(typeof(CommandInfo.HandlerDelegate), plugin, method);

            var command = handlerDelegate.Method.GetCustomAttribute<CommandAttribute>();
            var aliases = handlerDelegate.Method.GetCustomAttribute<AliasesAttribute>();
            var helpMessage = handlerDelegate.Method.GetCustomAttribute<HelpMessageAttribute>();
            var doNotShowInHelp = handlerDelegate.Method.GetCustomAttribute<DoNotShowInHelpAttribute>();

            var commandInfo = new CommandInfo(handlerDelegate)
            {
                HelpMessage = helpMessage?.HelpMessage ?? string.Empty,
                ShowInHelp = doNotShowInHelp == null,
            };

            // Create list of tuples that will be filled with one tuple per alias, in addition to the base command tuple.
            var commandInfoTuples = new List<(string, CommandInfo)> { (command?.Command, commandInfo) };
            if (aliases != null)
                commandInfoTuples.AddRange(aliases.Aliases.Select(alias => (alias, commandInfo)));

            return commandInfoTuples;
        }

        public void Dispose()
        {
            RemoveCommandHandlers();
            DalamudApi.XivCommonBase.Dispose();
            GC.SuppressFinalize(this);
        }
    }
    #endregion

    #region Attributes
    [AttributeUsage(AttributeTargets.Method)]
    public class AliasesAttribute : Attribute
    {
        public string[] Aliases { get; }

        public AliasesAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string Command { get; }

        public CommandAttribute(string command)
        {
            Command = command;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DoNotShowInHelpAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HelpMessageAttribute : Attribute
    {
        public string HelpMessage { get; }

        public HelpMessageAttribute(string helpMessage)
        {
            HelpMessage = helpMessage;
        }
    }
    #endregion
}
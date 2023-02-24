using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Game;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using EurekaHelper.XIV;

namespace EurekaHelper.System
{
    public class FatesManager : IDisposable
    {
        private List<Fate> lastFates = new();
        private IEurekaTracker EurekaTracker;

        public FatesManager()
        {
            DalamudApi.ClientState.TerritoryChanged += OnTerritoryChanged;

            if (Utils.IsPlayerInEurekaZone(DalamudApi.ClientState.TerritoryType))
            {
                DalamudApi.Framework.Update += OnUpdate;
                EurekaTracker = Utils.GetEurekaTracker(DalamudApi.ClientState.TerritoryType);
            }
        }

        private void OnTerritoryChanged(object? sender, ushort territoryId)
        {
            if (Utils.IsPlayerInEurekaZone(territoryId))
            {
                DalamudApi.Framework.Update += OnUpdate;
                EurekaTracker = Utils.GetEurekaTracker(territoryId);
            }
            else
            {
                DalamudApi.Framework.Update -= OnUpdate;
            }
        }

        private void OnUpdate(Framework framework)
        {
            if (EurekaHelper.Config.DisplayFateProgress)
            {
                List<Fate> instanceFates = DalamudApi.FateTable.Where(x => !Utils.IsBunnyFate(x.FateId)).ToList();
                foreach (var fate in instanceFates)
                {
                    EurekaFate eurekaFate = EurekaTracker.GetFates().SingleOrDefault(i => fate.FateId == i.FateId);
                    if (eurekaFate is null || eurekaFate.FateProgress == fate.Progress)
                        continue;

                    if (fate.Progress % 25 == 0)
                    {
                        eurekaFate.FateProgress = fate.Progress;
                        var sb = new SeStringBuilder()
                            .AddText($"{eurekaFate.BossName}: ")
                            .Append(Utils.MapLink(eurekaFate.TerritoryId, eurekaFate.MapId, eurekaFate.FatePosition))
                            .AddText(" is at ")
                            .AddUiForeground(58)
                            .AddText($"{eurekaFate.FateProgress}%")
                            .AddUiForegroundOff();

                        EurekaHelper.PrintMessage(sb.BuiltString);
                    }
                }
            }

            if (DalamudApi.FateTable.SequenceEqual(lastFates))
                return;

            List<Fate> currFates = DalamudApi.FateTable.Except(lastFates).ToList();
            List<EurekaFate> newFates = EurekaTracker.GetFates().Where(i => currFates.Select(i => i.FateId).Contains(i.FateId)).ToList();

            foreach (var fate in newFates)
                DisplayFatePop(fate);

            lastFates = DalamudApi.FateTable.ToList();
        }

        private static void DisplayFatePop(EurekaFate fate)
        {
            var sb = new SeStringBuilder()
                .AddText($"{fate.BossName}: ")
                .Append(Utils.MapLink(fate.TerritoryId, fate.MapId, fate.FatePosition));

            if (!fate.IsBunnyFate)
            {
                if (EurekaHelper.Config.DisplayToastPop)
                    DalamudApi.ToastGui.ShowQuest(sb.BuiltString);

                if (EurekaHelper.Config.PlayPopSound)
                    SoundManager.PlaySoundEffect(EurekaHelper.Config.NMSoundEffect);

                if (EurekaHelper.Config.DisplayFatePop)
                {
                    DalamudApi.PluginInterface.RemoveChatLinkHandler(fate.FateId);
                    DalamudLinkPayload payload = DalamudApi.PluginInterface.AddChatLinkHandler(fate.FateId, (i, m) =>
                    {
                        Utils.SetFlagMarker(fate);

                        switch (EurekaHelper.Config.PayloadOptions)
                        {
                            case PayloadOptions.CopyToClipboard:
                                Utils.CopyToClipboard(Utils.RandomFormattedText(fate));
                                break;

                            default:
                            case PayloadOptions.ShoutToChat:
                                Utils.SendMessage(Utils.RandomFormattedText(fate));
                                break;
                        }
                    });

                    var text = EurekaHelper.Config.PayloadOptions switch
                    {
                        PayloadOptions.ShoutToChat => "shout",
                        PayloadOptions.CopyToClipboard => "copy",
                        _ => "shout"
                    };

                    sb.AddText(" ");
                    sb.AddUiForeground(32);
                    sb.Add(payload);
                    sb.AddText($"[Click to {text}]");
                    sb.Add(RawPayload.LinkTerminator);
                    sb.AddUiForegroundOff();

                    EurekaHelper.PrintMessage(sb.BuiltString);
                }

                if (EurekaHelper.Config.AutoPopFate)
                {
                    if (PluginWindow.GetConnection().IsConnected() && PluginWindow.GetConnection().CanModify())
                    {
                        var trackerFate = PluginWindow.GetConnection().GetTracker().GetFates().Find(x => x.IncludeInTracker && x.FateId == fate.FateId);

                        if (trackerFate is null)
                            return;

                        if (!trackerFate.IsPopped())
                        {
                            _ = Task.Run(async () =>
                            {
                                await PluginWindow.GetConnection().SetPopTime((ushort)fate.TrackerId, DateTimeOffset.Now.ToUnixTimeMilliseconds());
                            });
                        }
                    }
                }
            }
            else
            {
                if (EurekaHelper.Config.DisplayBunnyFates)
                {
                    EurekaHelper.PrintMessage(sb.BuiltString);
                    SoundManager.PlaySoundEffect(EurekaHelper.Config.BunnySoundEffect);
                }
            }
        }

        public void Dispose()
        {
            DalamudApi.ClientState.TerritoryChanged -= OnTerritoryChanged;
            DalamudApi.Framework.Update -= OnUpdate;
        }
    }
}

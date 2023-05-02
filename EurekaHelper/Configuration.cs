using Dalamud.Configuration;
using Dalamud.Game.Text;
using EurekaHelper.System;
using System;
using System.Collections.Generic;

namespace EurekaHelper
{
    public enum PayloadOptions
    {
        ShoutToChat,
        CopyToClipboard,
        Nothing
    }

    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public void Initialize() 
        {
            if (CustomMessages.Count == 0)
            {
                CustomMessages.Add("/shout %bossName% POP. %flag%");
                Save();
            }
        }

        /*
         * General Configurations
         */
        public XivChatType ChatChannel { get; set; } = XivChatType.Echo;

        /*
         * Tracker Configurations
         */
        public bool DisplayFateProgress = false;

        public bool DisplayBunnyFates = false;

        public bool DisplayFatePop = true;

        public bool PlayPopSound = true;

        public bool DisplayToastPop = false;

        public bool AutoPopFate = true;

        public bool RandomizeMapCoords = true;

        public bool AutoCreateTracker = false;

        public List<string> CustomMessages { get; set; } = new();

        public SoundManager.SoundEffect NMSoundEffect { get; set; } = SoundManager.SoundEffect.SoundEffect36;

        public SoundManager.SoundEffect BunnySoundEffect { get; set; } = SoundManager.SoundEffect.SoundEffect41;

        public PayloadOptions PayloadOptions { get; set; } = PayloadOptions.ShoutToChat;

        /*
         * Server ID Configurations
         */
        public bool DisplayServerId = false;

        public bool DisplayServerIdInServerInfo = false;

        /*
         * Elemental Configurations
         */

        public bool DisplayElemental = true;

        public bool DisplayElementalToast = false;

        public bool ElementalCrowdsource = true;

        public PayloadOptions ElementalPayloadOptions { get; set;} = PayloadOptions.CopyToClipboard;

        public void Save() => DalamudApi.PluginInterface.SavePluginConfig(this);
    }
}

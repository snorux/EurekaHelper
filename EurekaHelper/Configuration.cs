using Dalamud.Configuration;
using EurekaHelper.System;
using System;

namespace EurekaHelper
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public void Initialize() { }

        public bool DisplayFateProgress = false;

        public bool DisplayBunnyFates = false;

        public bool PlayPopSound = false;

        public bool CopyNMToClipboard = false;

        public bool AutoPopFate = false;

        public SoundManager.SoundEffect NMSoundEffect { get; set; } = SoundManager.SoundEffect.SoundEffect36;
        public SoundManager.SoundEffect BunnySoundEffect { get; set; } = SoundManager.SoundEffect.SoundEffect41;

        public void Save() => DalamudApi.PluginInterface.SavePluginConfig(this);
    }
}

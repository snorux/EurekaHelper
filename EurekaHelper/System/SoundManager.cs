using Dalamud.Utility.Signatures;
using System;

namespace EurekaHelper.System
{
    public enum SoundEffect
    {
        SoundEffect36 = 0x24,
        SoundEffect37 = 0x25,
        SoundEffect38 = 0x26,
        SoundEffect39 = 0x27,
        SoundEffect40 = 0x28,
        SoundEffect41 = 0x29,
        SoundEffect42 = 0x2A,
        SoundEffect43 = 0x2B,
        SoundEffect44 = 0x2C,
        SoundEffect45 = 0x2D,
        SoundEffect46 = 0x2E,
        SoundEffect47 = 0x2F,
        SoundEffect48 = 0x30,
        SoundEffect49 = 0x31,
        SoundEffect50 = 0x32,
        SoundEffect51 = 0x33,
        SoundEffect52 = 0x34
    }

    internal unsafe class GameSound
    {
        [Signature("E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??")]
        public readonly delegate* unmanaged<uint, IntPtr, IntPtr, byte, void> PlaySoundEffect = null;

        public GameSound()
        {
            DalamudApi.GameInteropProvider.InitializeFromAttributes(this);
        }
    }

    public static class SoundManager
    {
        private static readonly GameSound GameSound = new();

        public static void PlaySoundEffect(SoundEffect soundEffect)
        {
            unsafe
            {
                GameSound.PlaySoundEffect((uint)soundEffect, IntPtr.Zero, IntPtr.Zero, 0);
            }
        }
    }
}

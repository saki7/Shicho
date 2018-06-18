using UnityEngine;
using System;
using System.Collections.Generic;


namespace Shicho
{
    using KeyModMap = Dictionary<SInput.KeyMod, string>;

    public static class SInput
    {
        [Flags]
        [Serializable]
        public enum KeyMod : UInt32
        {
            None  = 0,
            Ctrl  = 0b00000001,
            Shift = 0b00000010,
            Alt   = 0b00000100,
        }

        public static readonly KeyModMap ModMap = new KeyModMap() {
            {KeyMod.Ctrl, "Ctrl"}, {KeyMod.Shift, "Shift"}, {KeyMod.Alt, "Alt"},
            {KeyMod.Ctrl | KeyMod.Shift, "Ctrl-Shift"},
            {KeyMod.Ctrl | KeyMod.Alt, "Ctrl-Alt"},
            {KeyMod.Ctrl | KeyMod.Alt | KeyMod.Shift, "Ctrl-Alt-Shift"},
            {KeyMod.Alt | KeyMod.Shift, "Alt-Shift"},
        };

        [Serializable]
        public struct BoundKey
        {
            public KeyMod Mod { get; set; }
            public KeyCode Code { get; set; }

            public override string ToString()
            {
                return $"{ModMap[Mod]}-{Code}";
            }
        }

        public static KeyMod GetMod()
        {
            var mod = KeyMod.None;

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                mod |= KeyMod.Ctrl;
            }
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
                mod |= KeyMod.Alt;
            }
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                mod |= KeyMod.Shift;
            }
            return mod;
        }

        public static bool HasOnlyKeyDown(KeyMod mod, BoundKey key)
        {
            return Input.GetKeyDown(key.Code) && ((key.Mod & mod) == key.Mod);
        }
    }
}

using System;

namespace Shicho.Input
{
    [Flags]
    public enum KeyMod : UInt32
    {
        Ctrl  = 0b00000001,
        Shift = 0b00000010,
        Alt   = 0b00000100,
    }
}

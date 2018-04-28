namespace Shicho.Core
{
    internal static class Log
    {
        public delegate void DebugFunc(object msg);
        public static DebugFunc Debug;

        public delegate void InfoFunc(object msg);
        public static InfoFunc Info;

        public delegate void WarnFunc(object msg);
        public static WarnFunc Warn;

        public delegate void ErrorFunc(object msg);
        public static ErrorFunc Error;

        internal static string Format(object msg)
        {
            return $"[{Mod.ModInfo.ID}] {msg}";
        }
    }
}

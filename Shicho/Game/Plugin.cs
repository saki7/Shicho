using Shicho.Core;

using ColossalFramework.Plugins;

using System;
using System.Reflection;
using System.Linq;


namespace Shicho.Game
{
    public static class Plugin
    {
        public static void FetchAll()
        {
            plugins_ = PluginManager.instance.GetPluginsInfo()
                .Where(p => p.isEnabled && !p.isBuiltin && !p.ContainsAssembly(Assembly.GetExecutingAssembly()))
                .ToArray()
            ;
        }

        public static bool HasWorkshop(UInt64 id)
        {
            return plugins_.Any(p => p.publishedFileID.AsUInt64 == id);
        }

        public static PluginManager.PluginInfo[] All { get => plugins_; }
        private static PluginManager.PluginInfo[] plugins_;
    }
}

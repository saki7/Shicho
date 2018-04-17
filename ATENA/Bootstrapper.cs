using ColossalFramework;
using ColossalFramework.Plugins;
using UnityEngine;
using ICities;

using System;
using System.Linq;
using System.Threading;


namespace ATENA
{
    using LogCh = CODebugBase<LogChannel>;

    internal class Bootstrapper
    {
        public delegate void Log2(string msg);

        public static void Bootstrap()
        {
            if (!bootstrapped_) {
                LogCh.verbose = true;
                LogCh.EnableChannels(LogChannel.All);
                bootstrapped_ = true;
            }

            if (initialized_) return;

            // Switch logger based on (mostly) `ModTools` presence
            var useUnityLogger = true; // or IsModToolsActive()

            if (useUnityLogger) {
                Log.Info  = Debug.Log;
                Log.Warn  = Debug.LogWarning;
                Log.Error = Debug.LogError;

                Log.Info("using Unity logger");

            } else {
                void emit(PluginManager.MessageType type, object msg) =>
                    DebugOutputPanel.AddMessage(type, Log.Format(msg))
                ;
                Log.Info  = (msg) => emit(PluginManager.MessageType.Message, msg);
                Log.Warn  = (msg) => emit(PluginManager.MessageType.Warning, msg);
                Log.Error = (msg) => emit(PluginManager.MessageType.Error, msg);

                Log.Warn("using Colossal logger");
            }

            Log.Info("bootstrapping...");
            DestroyOld(ModInfo.ID);

            var gobj = new GameObject(ModInfo.ID);
            Log.Info($"new instance: 0x{gobj.GetInstanceID():X}");

            var app = gobj.AddComponent<Atena>();
            app.Initialize();
            initialized_ = true;

            Log.Info("loaded.");
        }

        private static void DestroyOld(string name)
        {
            while (true) {
                try {
                    var gobj = GameObject.Find(name)?.gameObject;
                    if (!gobj) break;

                    var oldInstanceID = gobj.GetInstanceID();
                    GameObject.DestroyImmediate(gobj);
                    Log.Info($"destroyed old instance: 0x{oldInstanceID:X}");

                    // Thread.Yield();
                    Thread.Sleep(0);

                } catch (Exception e) {
                    Log.Info($"failed to destroy: {e}");
                    break;
                }
            }
        }

        private static bool IsModToolsActive()
        {
            var pmng = PluginManager.instance;
            return (
                from p in pmng.GetPluginsInfo()

                // https://steamcommunity.com/sharedfiles/filedetails/?id=450877484
                where p.name is "450877484"

                select p.isEnabled
            ).FirstOrDefault();
        }

        public static bool bootstrapped_ = false, initialized_ = false;
    }
}

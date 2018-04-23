extern alias CitiesL;
using ColossalFramework;
using ColossalFramework.Plugins;
using UnityEngine;

using Harmony;

using System;
using System.Reflection;
using System.Linq;
using System.Threading;


namespace ATENA
{
    using LogCh = CODebugBase<CitiesL.LogChannel>;

    internal class Bootstrapper
    {
        static Bootstrapper instance_ = new Bootstrapper();
        public static Bootstrapper Instance {
            get => instance_;
        }
        private Bootstrapper() {}

        public void Bootstrap()
        {
            if (!bootstrapped_) {
                harmony_ = HarmonyInstance.Create("com.github.setnahq.atena");
                harmony_.PatchAll(Assembly.GetExecutingAssembly());

                LogCh.verbose = true;
                LogCh.EnableChannels(CitiesL.LogChannel.All);
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
                    CitiesL.DebugOutputPanel.AddMessage(type, Log.Format(msg))
                ;
                Log.Info  = (msg) => emit(PluginManager.MessageType.Message, msg);
                Log.Warn  = (msg) => emit(PluginManager.MessageType.Warning, msg);
                Log.Error = (msg) => emit(PluginManager.MessageType.Error, msg);

                Log.Warn("using Colossal logger");
            }

            Log.Info("bootstrapping...");
            {
                var oldGameObject = gobj ?? GameObject.Find(ModInfo.ID);

                while (oldGameObject) {
                    try {
                        Log.Warn($"old GameObject found: (\"{ModInfo.ID}\", 0x{oldGameObject.GetInstanceID():X})");
                        GameObject.DestroyImmediate(oldGameObject);
                        oldGameObject = GameObject.Find(ModInfo.ID);

                    } catch (Exception e) {
                        Log.Error(e);
                        break;
                    }
                }
            }

            gobj = new GameObject(ModInfo.ID);
            Log.Info($"new instance: 0x{gobj.GetInstanceID():X}");

            Atena.SetInstance(gobj.AddComponent<Atena>());
            initialized_ = true;

            Log.Info("loaded.");
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

        internal void Cleanup()
        {
            try {
                GameObject.DestroyImmediate(gobj);

                while (true) {
                    try {
                        var o = GameObject.Find(ModInfo.ID);
                        if (!o) break;
                        GameObject.DestroyImmediate(o);

                    } finally {
                        Thread.Sleep(0);
                    }
                }

            } finally {
                gobj = null; // deref
                initialized_ = false;
            }
        }

        private bool bootstrapped_ = false, initialized_ = false;
        private GameObject gobj = null;
        private HarmonyInstance harmony_;

        private Atena atena = null;
        public Atena Atena {
            get => atena;
        }
    }
}

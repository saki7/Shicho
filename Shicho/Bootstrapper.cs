extern alias Cities;

using Shicho.Core;

using ColossalFramework;
using ColossalFramework.Plugins;
using UnityEngine;

using Harmony;

using System;
using System.Reflection;
using System.Linq;
using System.Threading;


namespace Shicho
{
    using LogCh = CODebugBase<Cities::LogChannel>;

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
                harmony_ = HarmonyInstance.Create(Mod.ModInfo.COMIdentifier);
                harmony_.PatchAll(Assembly.GetExecutingAssembly());

                LogCh.verbose = true;
                LogCh.EnableChannels(Cities::LogChannel.All);
                bootstrapped_ = true;
            }

            if (IsInitialized()) return;

            // Switch logger based on (mostly) `ModTools` presence
            var useUnityLogger = true; // or IsModToolsActive()

            if (useUnityLogger) {
                Log.Debug = Debug.LogWarning;
                Log.Info  = Debug.Log;
                Log.Warn  = Debug.LogWarning;
                Log.Error = Debug.LogError;

                // Log.Info("using Unity logger");

            } else {
                void emit(PluginManager.MessageType type, object msg) =>
                    Cities::DebugOutputPanel.AddMessage(type, Log.Format(msg))
                ;
                Log.Info  = (msg) => emit(PluginManager.MessageType.Message, msg);
                Log.Warn  = (msg) => emit(PluginManager.MessageType.Warning, msg);
                Log.Error = (msg) => emit(PluginManager.MessageType.Error, msg);

                // Log.Warn("using Colossal logger");
            }

            #if !DEBUG
                Log.Debug = (_) => {};
            #endif

            Log.Info("bootstrapping...");
            try {
                DestroyOldInstance();

            } catch (Exception e) {
                Log.Error(e);
            }

            gobj_ = new GameObject(Mod.ModInfo.ID);
            // Log.Debug($"new instance: 0x{gobj_.GetInstanceID():X}");

            app_ = gobj_.AddComponent<App>();
            gobj_.SetActive(true);
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

        private void DestroyOldInstance()
        {
            if (gobj_ == null) return;
            //Log.Warn($"destroying: (\"{gobj_.name}\", 0x{gobj_.GetInstanceID():X})");
            GameObject.DestroyImmediate(gobj_);
        }

        internal void Cleanup()
        {
            if (!IsInitialized()) return;

            try {
                DestroyOldInstance();

            } finally {
                gobj_ = null;
            }
        }

        private bool IsInitialized() => gobj_ != null;

        private bool bootstrapped_ = false;
        private GameObject gobj_ = null;

        private App app_ = null;
        public static App AppInstance { get => Instance.app_; }

        private HarmonyInstance harmony_;
    }
}

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
using System.Collections.Generic;

namespace Shicho
{
    using LogCh = CODebugBase<Cities::LogChannel>;
    using PatchPair = KeyValuePair<MethodBase, MethodInfo>;

    internal class Bootstrapper
    {
        static Bootstrapper instance_ = new Bootstrapper();
        public static Bootstrapper Instance {
            get => instance_;
        }
        private Bootstrapper() {}

        public void BootstrapConfig()
        {
            if (cfg_ == null) {
                #region Log setup
                {
                    LogCh.verbose = true;
                    LogCh.EnableChannels(Cities::LogChannel.All);

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
                }
                #endregion Log setup

                cfg_ = Mod.Config.LoadFile(App.ConfigPath);
            }
        }

        public void Bootstrap()
        {
            if (IsInitialized) return;
            BootstrapConfig();

            #region Harmony initialization
            harmony_ = HarmonyInstance.Create(Mod.ModInfo.COMIdentifier);

            // unpatch other mods
            Tool.Watcher.UnpatchHostiles();

            Log.Debug($"applying Harmony...");
            harmony_.PatchAll(Assembly.GetExecutingAssembly());
            #endregion Harmony initialization

            Log.Info("loading mod instance...");
            try {
                DestroyOldInstance();

            } catch (Exception e) {
                Log.Error(e);
            }

            gobj_ = new GameObject(Mod.ModInfo.ID);
            // Log.Debug($"new instance: 0x{gobj_.GetInstanceID():X}");

            app_ = gobj_.AddComponent<App>();
            watcher_ = gobj_.AddComponent<Tool.Watcher>();

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

            GameObject.DestroyImmediate(watcher_);
            GameObject.DestroyImmediate(app_);
            GameObject.DestroyImmediate(gobj_);
        }

        private List<PatchPair> QueryAllPatchesByOwner(
            MethodBase target, IEnumerable<Harmony.Patch> patches, string owner
        ) {
            return patches
                .Where(p => p.owner == owner)
                .Select(p => new PatchPair(target, p.GetMethod(target)))
                .ToList()
            ;
        }

        private void RemoveAllPatchesByOwner(string owner)
        {
            var patches = new List<PatchPair>();

            foreach (var target in harmony_.GetPatchedMethods()) {
                var info = harmony_.GetPatchInfo(target);

                patches.AddRange(QueryAllPatchesByOwner(target, info.Prefixes, owner));
                patches.AddRange(QueryAllPatchesByOwner(target, info.Transpilers, owner));
                patches.AddRange(QueryAllPatchesByOwner(target, info.Postfixes, owner));
            }

            foreach (var pp in patches) {
                Log.Debug($"removing patch: {pp.Value} for {pp.Key}");
                harmony_.RemovePatch(pp.Key, pp.Value);
            }
        }

        internal void Cleanup()
        {
            if (!IsInitialized) return;

            try {
                RemoveAllPatchesByOwner(Mod.ModInfo.COMIdentifier);
                DestroyOldInstance();

            } finally {
                gobj_ = null;
            }
        }

        private GameObject gobj_ = null;
        private bool IsInitialized { get => gobj_ != null; }

        private Mod.Config cfg_;
        internal Mod.Config Config { get => cfg_; }

        private App app_ = null;
        public static App AppInstance { get => Instance.app_; }

        private Tool.Watcher watcher_;

        private HarmonyInstance harmony_;
        internal HarmonyInstance Harmony {
            get => harmony_;
        }
    }
}

﻿extern alias Cities;

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

            if (initialized_) return;

            // Switch logger based on (mostly) `ModTools` presence
            var useUnityLogger = true; // or IsModToolsActive()

            if (useUnityLogger) {
                Log.Debug = Debug.LogWarning;
                Log.Info  = Debug.Log;
                Log.Warn  = Debug.LogWarning;
                Log.Error = Debug.LogError;

                Log.Info("using Unity logger");

            } else {
                void emit(PluginManager.MessageType type, object msg) =>
                    Cities::DebugOutputPanel.AddMessage(type, Log.Format(msg))
                ;
                Log.Info  = (msg) => emit(PluginManager.MessageType.Message, msg);
                Log.Warn  = (msg) => emit(PluginManager.MessageType.Warning, msg);
                Log.Error = (msg) => emit(PluginManager.MessageType.Error, msg);

                Log.Warn("using Colossal logger");
            }

            Log.Info("bootstrapping...");
            {
                var oldGameObject = gobj ?? GameObject.Find(Mod.ModInfo.ID);

                while (oldGameObject) {
                    try {
                        Log.Warn($"old GameObject found: (\"{Mod.ModInfo.ID}\", 0x{oldGameObject.GetInstanceID():X})");
                        GameObject.DestroyImmediate(oldGameObject);
                        oldGameObject = GameObject.Find(Mod.ModInfo.ID);

                    } catch (Exception e) {
                        Log.Error(e);
                        break;
                    }
                }
            }

            gobj = new GameObject(Mod.ModInfo.ID);
            Log.Info($"new instance: 0x{gobj.GetInstanceID():X}");

            App.SetInstance(gobj.AddComponent<App>());
            initialized_ = true;

            Log.Info("loaded.");

            // TODO: move this to proper place
            App.Instance.OnLevelLoad();
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
                        var o = GameObject.Find(Mod.ModInfo.ID);
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
    }
}
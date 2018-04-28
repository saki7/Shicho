extern alias Cities;

using Shicho.Core;

using ICities;
using UnityEngine;

using System;
using System.IO;
using System.Collections.Generic;


namespace Shicho
{
    class App
        : MonoBehaviour
        , IDisposable
    {
        public void Awake()
        {
            cfg_ = Mod.Config.LoadFile(ConfigPath);

            try {
                // Log.Info("initializing...");

                cfgTool_ = gameObject.AddComponent<Tool.ConfigTool>();
                R = new ColossalFramework.Math.Randomizer(GetDeviceSeed());

                // stir up
                for (var i = 0; i < 123; ++i) {
                    R.ULong64();
                }

                // Log.Info("initialized");

            } catch (Exception e) {
                Log.Error($"failed to initialize: '{e}'");
            }
        }

        private void InitGUI()
        {

        }

        private void InitPhysics()
        {

        }

        public void LoadLevelData()
        {
            Log.Debug("loading prefabs...");
            pmgr_ = new PrefabManager();
            pmgr_.FetchAll();

            Log.Debug("loading props...");
            pcon_ = new PropManager();
            pcon_.Fetch();

            Log.Debug("loading traffic...");
            tcon_ = new TrafficController();
            tcon_.Fetch();

            Log.Debug("initializing flow generator...");
            fgen_ = new FlowGenerator(ref pmgr_, ref tcon_);
            // fgen_.AddFactory(typeof(Cities::Citizen));
        }

        public void UnloadLevelData()
        {
            if (pmgr_ != null) pmgr_.Dispose();
            if (tcon_ != null) tcon_.Dispose();
            if (fgen_ != null) fgen_.Dispose();
        }

        private void UnloadAllData()
        {
            UnloadLevelData();
            citizens_.Clear();
        }

        public void SetFlow(
            Cities::ItemClass.Service service,
            uint flow
        ) {
            if (service != Cities::ItemClass.Service.Citizen) {
                throw new NotImplementedException("service != Cities::ItemClass.Service.Citizen");
            }

            for (uint i = 0; i < flow; ++i) {
                var c = new Game.Citizen(20 + i, 3);
                //Log.Info($"created: {c}");

                citizens_.Add(c.ID, c);
            }
        }

        public void ChangeKeyBinding(Input.KeyMod? mod, KeyCode? key = null)
        {
            if (mod.HasValue) {
                cfg_.boundKeyMod = mod.Value;
            }

            if (key.HasValue) {
                cfg_.boundKey = key.Value;
            }
        }

        // NB: this method will be called by the env, regardless of
        //     the actual existence of the App instance :(
        public static void OnSettingsUI(UIHelperBase helper)
        {
            Bootstrapper.Instance.Bootstrap();
            Instance.cfgTool_.Populate(helper, Instance.cfg_);
        }

        public static ulong GetDeviceSeed()
        {
            return 1145144545191912345;
        }

        public void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            // Log.Debug("Dispose()");
            UnloadAllData();
        }

        public void SaveConfig() => cfg_.Save(ConfigPath);

        public static App Instance { get => Bootstrapper.AppInstance; }

        private const string ConfigPath = Mod.ModInfo.ID + ".xml";
        private Mod.Config cfg_ = null;
        public static Mod.Config Config { get => Instance.cfg_; }

        private Tool.ConfigTool cfgTool_ = null;
        internal ColossalFramework.Math.Randomizer R;

        private PrefabManager pmgr_;

        private PropManager pcon_;
        private TrafficController tcon_;

        private FlowGenerator fgen_;
        private Dictionary<Game.CitizenID, Game.Citizen> citizens_ = new Dictionary<Game.CitizenID, Game.Citizen>();
    }
}

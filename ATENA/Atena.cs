extern alias CitiesL;

using ATENA.Core;

using ICities;
using UnityEngine;

using System;
using System.Collections.Generic;


namespace ATENA
{
    internal class Atena
        : MonoBehaviour
    {
        private static Atena instance_;
        public static Atena Instance { get => instance_; }
        internal static void SetInstance(Atena a) { instance_ = a; }

        public Atena()
        {
            try {
                Log.Info("initializing...");

                cfg_ = GameObject.Find(Mod.ModInfo.ID).AddComponent<Mod.ConfigTool>();
                R = new ColossalFramework.Math.Randomizer(GetDeviceSeed());

                Log.Debug("loading prefabs...");
                pmgr_ = new PrefabManager();
                pmgr_.FetchAll();
                Log.Debug("prefabs loaded");

                Log.Debug("loading traffic...");
                tcon_ = new TrafficController();
                tcon_.Fetch();

                Log.Debug(tcon_);
                Log.Debug("traffic loaded");

                Log.Info("initialized!");

            } catch (Exception e) {
                Log.Error($"failed to initialize: '{e}'");
            }
        }

        public void PrintStats()
        {
            Log.Debug($@"
                === Atena status ===
                {tcon_}
                === end Atena status ===
            ");
        }

        public void Reset()
        {
            Log.Warn("resetting...");
            citizens_.Clear();
            Log.Warn("reset done");
        }

        public void SetFlow(
            CitiesL.ItemClass.Service service,
            uint flow
        ) {
            if (service != CitiesL.ItemClass.Service.Citizen) {
                throw new NotImplementedException("service != CitiesL.ItemClass.Service.Citizen");
            }

            for (uint i = 0; i < flow; ++i) {
                var c = new Game.Citizen(20 + i, 3);
                //Log.Info($"created: {c}");

                citizens_.Add(c.ID, c);
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            cfg_.Populate(helper);
        }

        public static ulong GetDeviceSeed()
        {
            return 114514;
        }

        private Mod.ConfigTool cfg_;
        internal ColossalFramework.Math.Randomizer R;

        private PrefabManager pmgr_;
        private TrafficController tcon_;

        private FlowGenerator fgen_ = new FlowGenerator();
        private Dictionary<Game.CitizenID, Game.Citizen> citizens_ = new Dictionary<Game.CitizenID, Game.Citizen>();
    }
}

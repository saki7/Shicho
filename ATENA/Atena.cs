extern alias CitiesL;
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

                Manager.ConfigManager.Instance.Load();
                cfg_ = GameObject.Find(ModInfo.ID).AddComponent<AtenaConfig>();

                R = new ColossalFramework.Math.Randomizer(GetDeviceSeed());

                Log.Info("initialized!");

            } catch (Exception e) {
                Log.Error($"failed to initialize: '{e}'");
            }
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

        public void Fetch()
        {
            foreach (var collection in CitiesL.NetCollection.FindObjectsOfType<CitiesL.NetCollection>()) {
                Log.Warn("collection: " + collection.name);

                foreach (var prefab in collection.m_prefabs) {
                    bool isRoadPrefab = prefab.GetComponent<CitiesL.NetInfo>() != null && prefab.GetComponent<CitiesL.RoadBaseAI>() != null;

                    if (isRoadPrefab) {
                        roadPrefabNames[prefab.GetInstanceID()] = prefab.name;
                        roadPrefabs[prefab.name] = prefab;
                        //ModDebug.Log("Road Prefab: " + prefab.name + "(" + isRoadPrefab + ")");
                    }
                }
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

        private AtenaConfig cfg_;
        internal ColossalFramework.Math.Randomizer R;

        private System.Collections.Generic.mu

        private FlowGenerator fgen_ = new FlowGenerator();
        private Dictionary<Game.CitizenID, Game.Citizen> citizens_ = new Dictionary<Game.CitizenID, Game.Citizen>();
    }
}

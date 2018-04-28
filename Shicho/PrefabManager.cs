extern alias Cities;

using Shicho.Core;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Shicho
{
    enum PrefabCategory
    {
        Building,
        Road,
    }

    class PrefabSet
    {
        public override string ToString()
        {
            return $"PrefabSet(count: {Infos.Count})";
        }

        public Dictionary<int, string> Names { get => names_; }
        public Dictionary<string, Cities::PrefabInfo> Infos { get => infos_; }

        public Dictionary<int, string> names_ = new Dictionary<int, string>();
        public Dictionary<string, Cities::PrefabInfo> infos_ = new Dictionary<string, Cities::PrefabInfo>();
    }

    class PrefabManager : IDisposable
    {
        public PrefabManager()
        {
            foreach (var v in System.Enum.GetValues(typeof(PrefabCategory))) {
                prefabs_.Add((PrefabCategory)v, new PrefabSet());
            }
        }

        public void FetchAll()
        {
            FetchBuildings();
            FetchRoads();
        }

        public void FetchBuildings()
        {
            Log.Debug("fetching buildings...");
            var pset = prefabs_[PrefabCategory.Building];

            foreach (var collection in Cities::BuildingCollection.FindObjectsOfType<Cities::BuildingCollection>()) {
                // Log.Info("got collection: " + collection.name);

                foreach (var prefab in collection.m_prefabs) {
                    if (
                        prefab.GetComponent<Cities::BuildingInfo>() != null &&
                        prefab.GetComponent<Cities::BuildingAI>() != null
                    ) {
                        pset.Names.Add(prefab.GetInstanceID(), prefab.name);
                        pset.Infos.Add(prefab.name, prefab);
                        // Log.Info($"building prefab: {prefab.name}");
                    }
                }
            }
        }

        public void FetchRoads()
        {
            Log.Debug("fetching roads...");
            var pset = prefabs_[PrefabCategory.Road];

            foreach (var collection in Cities::NetCollection.FindObjectsOfType<Cities::NetCollection>()) {
                // Log.Info("got collection: " + collection.name);

                foreach (var prefab in collection.m_prefabs) {
                    if (
                        prefab.GetComponent<Cities::NetInfo>() != null &&
                        prefab.GetComponent<Cities::RoadBaseAI>() != null)
                    {
                        pset.Names.Add(prefab.GetInstanceID(), prefab.name);
                        pset.Infos.Add(prefab.name, prefab);
                        // Log.Info($"road prefab: {prefab.name}");
                    }
                }
            }
        }

        private Dictionary<PrefabCategory, PrefabSet> prefabs_ = new Dictionary<PrefabCategory, PrefabSet>();
        public PrefabSet GetPrefabSet(PrefabCategory cat) => prefabs_[cat];

        public Cities::PrefabInfo[] GetDefaultPrefabs(PrefabCategory cat)
        {
            var pset = GetPrefabSet(cat);
            return pset.Infos.Values.Where(p => !p.m_isCustomContent).ToArray();
        }

        public void Dispose()
        {
            prefabs_.Clear();
        }
    }
}

extern alias CitiesL;

using ATENA.Core;

using System.Collections.Generic;
using System.Linq;


namespace ATENA
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
        public Dictionary<string, CitiesL.PrefabInfo> Infos { get => infos_; }

        public Dictionary<int, string> names_ = new Dictionary<int, string>();
        public Dictionary<string, CitiesL.PrefabInfo> infos_ = new Dictionary<string, CitiesL.PrefabInfo>();
    }

    class PrefabManager
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

            foreach (var collection in CitiesL.BuildingCollection.FindObjectsOfType<CitiesL.BuildingCollection>()) {
                // Log.Info("got collection: " + collection.name);

                foreach (var prefab in collection.m_prefabs) {
                    if (
                        prefab.GetComponent<CitiesL.BuildingInfo>() != null &&
                        prefab.GetComponent<CitiesL.BuildingAI>() != null
                    ) {
                        pset.Names.Add(prefab.GetInstanceID(), prefab.name);
                        pset.Infos.Add(prefab.name, prefab);
                        // Log.Info($"building prefab: {prefab.name}");
                    }
                }
            }

            Log.Info($"prefabs: {pset}");
        }

        public void FetchRoads()
        {
            Log.Debug("fetching roads...");
            var pset = prefabs_[PrefabCategory.Road];

            foreach (var collection in CitiesL.NetCollection.FindObjectsOfType<CitiesL.NetCollection>()) {
                // Log.Info("got collection: " + collection.name);

                foreach (var prefab in collection.m_prefabs) {
                    if (
                        prefab.GetComponent<CitiesL.NetInfo>() != null &&
                        prefab.GetComponent<CitiesL.RoadBaseAI>() != null)
                    {
                        pset.Names.Add(prefab.GetInstanceID(), prefab.name);
                        pset.Infos.Add(prefab.name, prefab);
                        // Log.Info($"road prefab: {prefab.name}");
                    }
                }
            }
            Log.Info($"prefabs: {pset}");
        }

        private Dictionary<PrefabCategory, PrefabSet> prefabs_ = new Dictionary<PrefabCategory, PrefabSet>();
        public PrefabSet GetPrefabSet(PrefabCategory cat) => prefabs_[cat];

        public CitiesL.PrefabInfo[] GetDefaultPrefabs(PrefabCategory cat)
        {
            var pset = GetPrefabSet(cat);
            return pset.Infos.Values.Where(p => !p.m_isCustomContent).ToArray();
        }
    }
}

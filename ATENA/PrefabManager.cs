extern alias CitiesL;

using ATENA.Core;

using System.Collections.Generic;


namespace ATENA
{
    class PrefabManager
    {
        public PrefabManager()
        {
            // FetchAll();
        }

        public void FetchAll()
        {
            FetchRoads();
        }

        public void FetchRoads()
        {
            Log.Debug("fetching roads...");

            foreach (var collection in CitiesL.NetCollection.FindObjectsOfType<CitiesL.NetCollection>()) {
                // Log.Info("got collection: " + collection.name);

                foreach (var prefab in collection.m_prefabs) {
                    bool isRoadPrefab = prefab.GetComponent<CitiesL.NetInfo>() != null && prefab.GetComponent<CitiesL.RoadBaseAI>() != null;

                    if (isRoadPrefab) {
                        roadPrefabNames_[prefab.GetInstanceID()] = prefab.name;
                        roadPrefabs_[prefab.name] = prefab;
                        // Log.Info($"road prefab: {prefab.name} ({isRoadPrefab})");
                    }
                }
            }

            Log.Debug("roads fetched");
        }

        private Dictionary<int, string> roadPrefabNames_ = new Dictionary<int, string>();
        private Dictionary<string, CitiesL.NetInfo> roadPrefabs_ = new Dictionary<string, CitiesL.NetInfo>();
    }
}

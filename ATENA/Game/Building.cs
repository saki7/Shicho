extern alias CitiesL;

using ATENA.Core;

using ColossalFramework;
using UnityEngine;


namespace ATENA.Game
{
    class BuildingID
    {
        public ushort value;
    }

    class Building
    {
        public Building(Vector3 pos, CitiesL.BuildingInfo info)
        {
            Log.Debug($"aaa!! {mgr_}, {pos}, {info}, {Atena.Instance.R}");

            var buildIndex = Singleton<CitiesL.SimulationManager>.instance.m_currentBuildIndex;

            mgr_.CreateBuilding(
                building: out id_.value,
                randomizer: ref Atena.Instance.R,
                info: info,
                position: pos,
                angle: 1,
                length: 1,
                buildIndex: buildIndex
            );
        }

        private static CitiesL.BuildingManager mgr_ = Singleton<CitiesL.BuildingManager>.instance;
        private BuildingID id_ = new BuildingID();
        public BuildingID ID { get => id_; }
    }
}

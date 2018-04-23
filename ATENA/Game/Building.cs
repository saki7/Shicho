extern alias CitiesL;
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
        public Building()
        {
            var info = new CitiesL.BuildingInfo();
            var pos = new Vector3();

            //mgr_.CreateBuilding(
            //    building: out id_.value,
            //    randomizer: ref Atena.Instance.R,
            //    info: info,
            //    position: pos,
            //    angle: 180,
            //    length: 20,
            //    buildIndex: 0
            //);
        }

        private static CitiesL.BuildingManager mgr_ = Singleton<CitiesL.BuildingManager>.instance;
        private BuildingID id_ = new BuildingID();
        public BuildingID ID { get => id_; }
    }
}

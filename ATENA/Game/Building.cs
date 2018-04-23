extern alias CitiesL;
using ColossalFramework;

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

            mgr_.CreateBuilding(
                out id_.value,
                ref Atena.Instance.R,
                info, pos, angle, length,
                buildIndex
            );
        }

        private static CitiesL.BuildingManager mgr_ = Singleton<CitiesL.BuildingManager>.instance;
        private BuildingID id_ = new BuildingID();
        public BuildingID ID { get => id_; }
    }
}

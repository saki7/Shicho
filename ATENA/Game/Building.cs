extern alias CitiesL;

using ATENA.Core;

using ColossalFramework;
using UnityEngine;

using System;
using System.Linq;


namespace ATENA.Game
{
    using CIC = CitiesL.ItemClass;
    using Flags = CitiesL.Building.Flags;

    class BuildingID
    {
        public ushort value;
    }

    class Building : IGameObject<CitiesL.Building, CitiesL.Building.Flags>
    {
        public Building(CitiesL.BuildingInfo info, Vector3 pos)
        {
            // Log.Debug($"aaa!! {mgr_}, {pos}, {info}, {Atena.Instance.R}");

            var buildIndex = Singleton<CitiesL.SimulationManager>.instance.m_currentBuildIndex;

            var success = mgr_.CreateBuilding(
                building: out id_.value,
                randomizer: ref Atena.Instance.R,
                info: info,
                position: pos,
                angle: 1,
                length: 4,
                buildIndex: buildIndex
            );

            var building = new CitiesL.Building();
            obj_ = mgr_.m_buildings.m_buffer[id_.value];
            Log.Debug($"building '{obj_.Info.name}': {obj_.m_position}");


            if (!success) {
                throw new GameError(typeof(Building), "failed: CreateBuilding()");
            }
            PrintAllInfo();
        }

        public static bool HasFlags(CitiesL.Building obj, Flags flags)
        {
            return (obj.m_flags & flags) != Flags.None;
        }

        bool IGameObject<CitiesL.Building, CitiesL.Building.Flags>.HasFlags(CitiesL.Building obj, Flags flags)
        {
            return Building.HasFlags(obj, flags);
        }

        private void PrintAllInfo()
        {
            foreach (var b in Core.DataQuery.Buildings().Where(b => b.Info.GetService() == CIC.Service.Residential)) {
                Log.Debug($"building '{b.Info.name}': {b.Info.GetService()}, [{b.m_flags}], Angle: {b.m_angle}, Length: {b.Length}, Position: {b.m_position}");
            }
        }

        public void Dispose()
        {
            mgr_.ReleaseBuilding(id_.value);
        }


        private static CitiesL.BuildingManager mgr_ = Singleton<CitiesL.BuildingManager>.instance;
        private BuildingID id_ = new BuildingID();
        public BuildingID ID { get => id_; }

        private CitiesL.Building obj_;
    }
}

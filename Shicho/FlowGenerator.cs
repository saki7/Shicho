extern alias Cities;

using Shicho.Core;

using Citizen = Cities::Citizen;
using BuildingInfo = Cities::BuildingInfo;

using CIC = Cities::ItemClass;
using ColossalFramework;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Shicho
{
    using FlowSourceMap = Dictionary<Type, FlowSource>;

    class FlowSource
    {
        public FlowSource(Game.Building building)
        {
            bldg_ = building;
        }
        private Game.Building bldg_;
        public Game.Building Building { get => bldg_; }
    }

    class FlowGenerator : IDisposable
    {
        public FlowGenerator(ref PrefabManager pmgr, ref TrafficController tcon)
        {
            pmgr_ = pmgr;
            tcon_ = tcon;
        }

        public void AddFactory(Type targetType)
        {
            Log.Debug($"adding factory for target '{targetType}'");

            if (targetType == typeof(Cities::Citizen)) {
                var refNode = tcon_.RoadNodes.First();
                // Log.Debug($"reference node: {refNode} [{refNode.m_flags}] ({refNode.m_position})");

                //foreach (var p in pmgr_.GetDefaultPrefabs(PrefabCategory.Building)) {
                //    Log.Debug($"{p.name}: {p.GetService()}, {p.GetSubService()}");
                //}

                var refInfo = pmgr_.GetDefaultPrefabs(PrefabCategory.Building)
                    .First(p => p.GetService() == CIC.Service.Residential)
                ;

                // Log.Debug($"reference info: {refInfo}");

                sources_.Add(targetType, new FlowSource(
                    building: new Game.Building(
                        info: (Cities::BuildingInfo)refInfo,
                        pos: DataQuery.Buildings().First().m_position
                    )
                ));

            } else {
                throw new NotImplementedException($"targetType '{targetType}'");
            }
        }

        public void Dispose()
        {
            sources_.Clear();
        }

        private PrefabManager pmgr_;
        private TrafficController tcon_;
        private FlowSourceMap sources_ = new FlowSourceMap();
    }
}

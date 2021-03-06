﻿extern alias Cities;

using ColossalFramework;

using System.Collections.Generic;
using System.Linq;


namespace Shicho.Core
{
    using Citizen = Cities::Citizen;
    using NetLane = Cities::NetLane;
    using Building = Cities::Building;
    using NetSegment = Cities::NetSegment;
    using NetNode = Cities::NetNode;

    static class DataQuery
    {
        public delegate bool QueryHandler<T, IdT>(ref T obj, IdT id);

        public static void Citizens(
            QueryHandler<Citizen, uint> h,
            Citizen.Flags flags = Citizen.Flags.Created
        ) {
            var mgr = Singleton<Cities::CitizenManager>.instance;

            for (int i = 0; i < mgr.m_citizens.m_size; ++i) {
                if ((mgr.m_citizens.m_buffer[i].m_flags & flags) == Citizen.Flags.None) continue;

                if (!h.Invoke(ref mgr.m_citizens.m_buffer[i], (uint)i)) return;
            }
        }

        public static IEnumerable<NetLane>
        NetLanes(NetLane.Flags flags = NetLane.Flags.Created)
        {
            var mgr = Singleton<Cities::NetManager>.instance;
            return mgr.m_lanes.m_buffer.Where(e =>
                ((NetLane.Flags)e.m_flags & flags) != NetLane.Flags.None
            );
        }

        public static IEnumerable<Building>
        Buildings(Building.Flags flags = Building.Flags.Created)
        {
            var mgr = Singleton<Cities::BuildingManager>.instance;
            return mgr.m_buildings.m_buffer.Where(e =>
                (e.m_flags & flags) != Cities::Building.Flags.None
            );
        }

        public static IEnumerable<NetSegment>
        Segments(NetSegment.Flags flags = NetSegment.Flags.Created)
        {
            var mgr = Singleton<Cities::NetManager>.instance;
            return mgr.m_segments.m_buffer.Where(e =>
                (e.m_flags & flags) != Cities::NetSegment.Flags.None
            );
        }

        public static IEnumerable<NetNode>
        Nodes(NetNode.Flags flags = NetNode.Flags.Created)
        {
            var mgr = Singleton<Cities::NetManager>.instance;
            return mgr.m_nodes.m_buffer.Where(e =>
                (e.m_flags & flags) != Cities::NetNode.Flags.None
            );
        }
    }
}

extern alias CitiesL;

using ColossalFramework;

using System.Collections.Generic;
using System.Linq;


namespace ATENA.Core
{
    using Building = CitiesL.Building;
    using NetSegment = CitiesL.NetSegment;
    using NetNode = CitiesL.NetNode;

    static class DataQuery
    {
        public static IEnumerable<Building>
        Buildings(Building.Flags flags = Building.Flags.Created)
        {
            var mgr = Singleton<CitiesL.BuildingManager>.instance;
            return mgr.m_buildings.m_buffer.Where(e =>
                (e.m_flags & flags) != CitiesL.Building.Flags.None
            );
        }

        public static IEnumerable<NetSegment>
        Segments(NetSegment.Flags flags = NetSegment.Flags.Created)
        {
            var mgr = Singleton<CitiesL.NetManager>.instance;
            return mgr.m_segments.m_buffer.Where(e =>
                (e.m_flags & flags) != CitiesL.NetSegment.Flags.None
            );
        }

        public static IEnumerable<NetNode>
        Nodes(NetNode.Flags flags = NetNode.Flags.Created)
        {
            var mgr = Singleton<CitiesL.NetManager>.instance;
            return mgr.m_nodes.m_buffer.Where(e =>
                (e.m_flags & flags) != CitiesL.NetNode.Flags.None
            );
        }

    }
}

extern alias CitiesL;
using CIC = CitiesL.ItemClass;
using NetNode = CitiesL.NetNode;
using NetSegment = CitiesL.NetSegment;

using ATENA.Core;

using ColossalFramework;

using System.Linq;
using System.Collections.Generic;


namespace ATENA
{
    using NetNodeMap = Dictionary<CIC.Service, List<NetNode>>;
    using NetSegmentMap = List<NetSegment>;

    class TrafficController
    {
        public void Fetch()
        {
            #region Fetch nodes
            {
                nodes_.Clear();
                uint fetchedCount = 0;

                foreach (var node in Buffer.Nodes(mgr_)) {
                    bool hasFlag(NetNode.Flags flag) => (node.m_flags & flag) != NetNode.Flags.None;
                    if (!hasFlag(NetNode.Flags.Created)) continue;

                    var service = node.Info.GetService();

                    if (!nodes_.ContainsKey(service)) {
                        nodes_.Add(service, new List<NetNode>());
                    }
                    nodes_[service].Add(node);
                    ++fetchedCount;
                }
                Log.Debug($"nodeCount: {mgr_.m_nodeCount} (fetched: {fetchedCount})");
            }
            #endregion Fetch nodes

            #region Fetch segments
            {
                segments_.Clear();
                uint fetchedCount = 0;

                foreach (var seg in Buffer.Segments(mgr_)) {
                    bool hasFlag(NetSegment.Flags flag) => (seg.m_flags & flag) != NetSegment.Flags.None;
                    if (!hasFlag(NetSegment.Flags.Created)) continue;

                    segments_.Add(seg);
                    ++fetchedCount;
                }
                Log.Debug($"segmentCount: {mgr_.m_segmentCount} (fetched: {fetchedCount})");
            }
            #endregion Fetch segments
        }

        public override string ToString()
        {
            return $@"Traffic nodes: ({string.Join(", ", nodes_.Select(kv => $"{kv.Key}: {kv.Value.Count}").ToArray())})";
        }

        private static CitiesL.NetManager mgr_ = Singleton<CitiesL.NetManager>.instance;
        private NetNodeMap nodes_ = new NetNodeMap();
        private NetSegmentMap segments_ = new NetSegmentMap();
    }
}

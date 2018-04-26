extern alias CitiesL;

using ATENA.Core;

using ColossalFramework;

using System.Linq;
using System.Collections.Generic;


namespace ATENA
{
    using NetNodeList = List<NetNode>;
    using NetNodeMap = Dictionary<CitiesL.ItemClass.Service, List<NetNode>>;
    using NetSegmentMap = List<NetSegment>;

    class TrafficController
    {
        public void Fetch()
        {
            #region Fetch nodes
            {
                nodes_.Clear();
                foreach (var node in DataQuery.Nodes()) {
                    var service = node.Info.GetService();

                    if (!nodes_.ContainsKey(service)) {
                        nodes_.Add(service, new NetNodeList());
                    }
                    nodes_[service].Add(node);
                    //Log.Debug($"pos: {node.m_position}");
                }
                Log.Debug($"nodeCount: {mgr_.m_nodeCount} (fetched: {nodes_.Count()})");
            }
            #endregion Fetch nodes

            #region Fetch segments
            {
                segments_.Clear();
                foreach (var seg in DataQuery.Segments()) {
                    segments_.Add(seg);
                }
                Log.Debug($"segmentCount: {mgr_.m_segmentCount} (fetched: {segments_.Count()})");
            }
            #endregion Fetch segments
        }

        public override string ToString()
        {
            return $@"Traffic nodes: ({string.Join(", ", nodes_.Select(kv => $"{kv.Key}: {kv.Value.Count}").ToArray())})";
        }

        private static CitiesL.NetManager mgr_ = Singleton<CitiesL.NetManager>.instance;
        private NetNodeMap nodes_ = new NetNodeMap();
        public NetNodeMap Nodes { get => nodes_; }
        public NetNodeList RoadNodes { get => nodes_[ItemClass.Service.Road]; }

        private NetSegmentMap segments_ = new NetSegmentMap();
        public NetSegmentMap Segments { get => segments_; }
    }
}

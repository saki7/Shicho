extern alias Cities;

using ATENA.Core;

using ColossalFramework;

using System;
using System.Linq;
using System.Collections.Generic;


namespace ATENA
{
    using NetNodeList = List<Cities::NetNode>;
    using NetNodeMap = Dictionary<Cities::ItemClass.Service, List<Cities::NetNode>>;
    using NetSegmentMap = List<Cities::NetSegment>;
    using DistrictList = List<Cities::District>;

    class TrafficController : IDisposable
    {
        public void Fetch()
        {
            var mgr = Cities::RenderManager.instance;
            //mgr.m_overlayBuffer.SetGlobalDepthBias(
            //    4.12f, 4.12f
            //);
            //mgr.lightSystem.m_lightBuffer.SetGlobalDepthBias(
            //    4.12f, 4.12f
            //);
            //UnityEngine.Shader.SetGlobalVector(
            //    "_ShadowForward",
            //    mgr.MainLight.transform.forward * 1200f
            //);
            // mgr.ShadowDistance = 200f;
            // Log.Debug($"shadowDistance: {mgr.ShadowDistance}");

            // Log.Debug($"[shadow bias]: shadowBias: {mgr.MainLight.shadowBias}, normalBias: {mgr.MainLight.shadowNormalBias}");

            // mgr.MainLight.shadowBias = 0.6f; // 0.162075f;
            // mgr.MainLight.shadowNormalBias = 0.6f; // 0.4

            // Log.Debug($"[shadow bias]: shadowBias: {mgr.MainLight.shadowBias}, normalBias: {mgr.MainLight.shadowNormalBias}");

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

            //#region Fetch districts
            //{
            //    districts_.Clear();
            //    foreach (var dist in DataQuery.Districts()) {
            //        districts_.Add(dist);
            //    }
            //}
            //#endregion Fetch districts
        }

        public void Dispose()
        {
            nodes_.Clear();
            segments_.Clear();
            districts_.Clear();
        }

        public override string ToString()
        {
            return $@"Traffic nodes: ({string.Join(", ", nodes_.Select(kv => $"{kv.Key}: {kv.Value.Count}").ToArray())})";
        }

        private static Cities::NetManager mgr_ = Singleton<Cities::NetManager>.instance;
        private NetNodeMap nodes_ = new NetNodeMap();
        public NetNodeMap Nodes { get => nodes_; }
        public NetNodeList RoadNodes { get => nodes_[Cities::ItemClass.Service.Road]; }

        private NetSegmentMap segments_ = new NetSegmentMap();
        public NetSegmentMap Segments { get => segments_; }

        private DistrictList districts_ = new DistrictList();
        public DistrictList Districts { get => districts_; }
    }
}

extern alias CitiesL;

using ATENA.Core;

using ColossalFramework;


namespace ATENA.Game
{
    class Road
    {
        public Road()
        {
            Log.Debug($"nodeCount: {mgr_.m_nodeCount}");
            foreach (var node in Buffer.Nodes(mgr_)) {
                Log.Debug($"node: {node} ({node.Info.GetService()})");
            }

            Log.Debug($"segmentCount: {mgr_.m_segmentCount}");
            foreach (var seg in Buffer.Segments(mgr_)) {
                Log.Debug($"seg: {seg}");
            }
        }

        private static CitiesL.NetManager mgr_ = Singleton<CitiesL.NetManager>.instance;
    }
}

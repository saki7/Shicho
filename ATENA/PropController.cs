extern alias CitiesL;

using ATENA.Core;

using System.Linq;


namespace ATENA
{
    using PropManager = CitiesL.PropManager;

    class PropController
    {
        public PropController()
        {}

        public void Fetch()
        {
            var mgr = NetManager.instance;
            var segments = mgr.m_segments.m_buffer.Where(e =>
                (e.m_flags & NetSegment.Flags.Created) != NetSegment.Flags.None
            );

            foreach (var seg in segments) {
                foreach (var lane in seg.Info.m_lanes) {
                    if (seg.Info.m_lanes == null || lane.m_laneProps == null) continue;

                    foreach (var prop in lane.m_laneProps.m_props) {
                        if (prop == null || prop.m_prop == null) continue;

                        var info = prop.m_prop;
                        if (info == null || !info.m_isDecal) continue;

                        //info.m_alwaysActive = true;
                        //info.m_minScale = 1f;
                        //info.m_illuminationOffRange.x = 10000f;
                        //info.m_illuminationOffRange.y = 10000f;
                        //info.m_lodMin = info.m_lodMax;
                        //info.m_lodRenderDistance = 2f;

                        //info.m_material.SetOverrideTag("RenderType", "Decal");

                        // info.m_material.renderQueue = 3000;
                        Log.Debug($"ffff: {info.m_material.GetFloat("_Mode")}: {System.String.Join(", ", info.m_material.shaderKeywords)}");
                        //info.m_material.DisableKeyword("_ALPHATEST_ON");
                        //info.m_material.DisableKeyword("_ALPHABLEND_ON");

                        //var material = info.m_material;
                        //material.SetOverrideTag("RenderType", "");
                        //material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        //material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        //material.SetInt("_ZWrite", 1);
                        //material.DisableKeyword("_ALPHATEST_ON");
                        //material.DisableKeyword("_ALPHABLEND_ON");
                        //material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        //material.renderQueue = -1;
                        //material.shader = UnityEngine.Shader.Find("Toon/Basic");

                        // Log.Debug($"RenderType: {info.m_material.GetTag("RenderType", false)}");
                        Log.Debug($"[{(info.m_isDecal ? "decal" : "other")}] maxRenderDistance: {info.m_maxRenderDistance}, lodRenderDistance: {info.m_lodRenderDistance}, m_illuminationOffRange: {info.m_illuminationOffRange}, m_illuminationBlinkType: {info.m_illuminationBlinkType}, minScale: {info.m_minScale}, maxScale: {info.m_maxScale}, lodMin: {info.m_lodMin}, lodMax: {info.m_lodMax}, info.m_material.renderQueue: {info.m_material.renderQueue}");

                        if (info.m_isDecal) {
                            info.m_maxRenderDistance = 10000f;
                            info.m_lodRenderDistance = info.m_maxRenderDistance;
                            info.m_effects = null;
                        }
                    }
                }
            }
        }

        private static PropManager mgr_ = PropManager.instance;
    }
}

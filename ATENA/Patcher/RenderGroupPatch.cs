extern alias CitiesL;

using ATENA.Core;

using Harmony;

namespace ATENA.Patcher.DayNightPropertiesPatch
{
    [HarmonyPatch(typeof(CitiesL.DayNightProperties))]
    [HarmonyPatch("UpdateLighting")]
    class UpdateLighting
    {
        static void Postfix()
        {
            var mgr = CitiesL.RenderManager.instance;
            if (mgr == null) return;
            mgr.MainLight.shadowBias = 0.6f;
        }
    }
}

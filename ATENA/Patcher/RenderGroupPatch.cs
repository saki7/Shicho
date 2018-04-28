extern alias Cities;

using ATENA.Core;

using Harmony;

namespace ATENA.Patcher.DayNightPropertiesPatch
{
    [HarmonyPatch(typeof(Cities::DayNightProperties))]
    [HarmonyPatch("UpdateLighting")]
    [HarmonyAfter(new string[] { "com.ronyx.relight", "com.ronyx.relight.biaspatch" })]
    class UpdateLighting
    {
        static void Postfix()
        {
            var mgr = Cities::RenderManager.instance;
            if (mgr == null) return;
            mgr.MainLight.shadowBias = 0.6f;
        }
    }
}

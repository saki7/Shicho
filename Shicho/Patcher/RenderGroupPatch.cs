extern alias Cities;

using Shicho.Core;

using Harmony;

namespace Shicho.Patcher.DayNightPropertiesPatch
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

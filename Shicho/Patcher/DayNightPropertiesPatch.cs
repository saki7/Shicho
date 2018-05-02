extern alias Cities;

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

            lock (App.Config.GraphicsLock) {
                mgr.MainLight.shadowBias = App.Config.Graphics.shadowBias;
            }
        }
    }
}

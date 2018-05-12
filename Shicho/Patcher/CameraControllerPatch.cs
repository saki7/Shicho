extern alias Cities;

using UnityEngine;
using Harmony;

namespace Shicho.Patcher.CameraControllerPatch
{
    [HarmonyPatch(typeof(Cities::CameraController))]
    [HarmonyPatch("LateUpdate")]
    class LateUpdate
    {
        static void Postfix()
        {
            if (!Cities::InfoManager.instance || Cities::InfoManager.instance.CurrentMode != Cities::InfoManager.InfoMode.None) {
                return;
            }
            var light = App.Instance.MainLight;
            if (!light) return;

            lock (App.Config.GraphicsLock) {
                light.shadows = LightShadows.Soft;

                if (App.Config.Graphics.shadowStrength) {
                    light.shadowStrength = App.Config.Graphics.shadowStrength.Value;
                }
                if (App.Config.Graphics.lightIntensity) {
                    light.intensity = App.Config.Graphics.lightIntensity.Value;
                }
            }
        }
    }
}

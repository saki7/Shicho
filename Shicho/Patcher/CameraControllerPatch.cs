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
            var light = App.Instance.MainLight;

            lock (App.Config.GraphicsLock) {
                light.shadows = LightShadows.Soft;
                light.shadowStrength = App.Config.Graphics.shadowStrength;
                light.intensity = App.Config.Graphics.lightIntensity;
            }
        }
    }
}

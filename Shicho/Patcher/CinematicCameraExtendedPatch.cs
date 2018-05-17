extern alias Cities;

using CinematicCameraExtended;

using ICities;

using ColossalFramework;
using ColossalFramework.Plugins;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

using System.Linq;

using Harmony;


namespace Shicho.Patcher
{
    [HarmonyPatch(typeof(CameraPath))]
    [HarmonyPatch("MoveCamera")]
    public class CinematicCameraExtendedPatch
    {
        public static bool IsModEnabled(string modName)
        {
            var plugins = PluginManager.instance.GetPluginsInfo();
            return (from plugin in plugins.Where(p => p.isEnabled)
                    select plugin.GetInstances<IUserMod>() into instances
                    where instances.Any()
                    select instances[0].Name into name
                    where name == modName
                    select name).Any();
        }

        static bool Prepare()
        {
            return Game.Plugin.HasWorkshop(785528371);
        }

        static bool Prefix(ref bool __result, float time, float speed, Camera camera, FastList<object> knots, out float timeOut)
        {
            bool result = true;
            int index = 0;
            float duration = 0f;

            for (int i = 0; i < knots.m_size; i++) {
                var knot = (Knot)knots.m_buffer[i];
                float knotDuration = (knot.duration + knot.delay) / speed;
                if (time < duration + knotDuration) {
                    index = i;
                    break;
                }
                duration += knotDuration;
            }

            bool ended = false;
            if (index + 1 >= knots.m_size) {
                index = knots.m_size - 2;
                ended = true;
                if (time > duration + ((Knot)knots.m_buffer[knots.m_size - 1]).delay) {
                    ended = false;
                    result = false;
                    index = 0;
                }
            }

            var currentKnot = (Knot)knots.m_buffer[index];
            var nextKnot = (Knot)knots.m_buffer[index + 1];

            float t = time - duration;
            if (t >= currentKnot.delay && result && !ended) {
                t -= currentKnot.delay;
                switch (currentKnot.mode) {
                case EasingMode.None:
                    t /= currentKnot.duration / speed;
                    break;
                case EasingMode.EaseIn:
                    t = CameraPath.EaseInQuad(t, 0f, 1f, currentKnot.duration / speed);
                    break;
                case EasingMode.EaseOut:
                    t = CameraPath.EaseOutQuad(t, 0f, 1f, currentKnot.duration / speed);
                    break;
                case EasingMode.EaseInOut:
                    t = CameraPath.EaseInOutQuad(t, 0f, 1f, currentKnot.duration / speed);
                    break;
                case EasingMode.Auto:
                    t = Spline.CalculateSplineT(knots.m_buffer, knots.m_size, index, t / (currentKnot.duration / speed));
                    break;
                }
            } else {
                t = ended ? 1f : 0f;
            }

            float fov = Mathf.Lerp(currentKnot.fov, nextKnot.fov, t);

            float distance1 = currentKnot.size * (1f - currentKnot.height / CameraDirector.cameraController.m_maxDistance) / Mathf.Tan(0.0174532924f * fov);
            float distance2 = nextKnot.size * (1f - nextKnot.height / CameraDirector.cameraController.m_maxDistance) / Mathf.Tan(0.0174532924f * fov);
            float distance = Mathf.Lerp(distance1, distance2, t);

            var rotation = Spline.CalculateSplineRotationEuler(knots.m_buffer, knots.m_size, index, t);
            var position = Spline.CalculateSplinePosition(knots.m_buffer, knots.m_size, index, t) + rotation * new Vector3(0f, 0f, -distance);

            camera.transform.position = position;
            camera.transform.rotation = rotation;
            camera.fieldOfView = fov;

            time += Time.deltaTime;
            timeOut = time;
            __result = result;
            return false;
        }
    }
}

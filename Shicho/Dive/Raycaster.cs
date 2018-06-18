extern alias Cities;

using ColossalFramework;
using UnityEngine;

using System;


namespace Shicho.Dive
{
    public class Raycaster : ToolBase
    {
        public static Vector3? Cast(Camera cam, float angle, Vector3 transform)
        {
            var direction = Quaternion.AngleAxis(angle, transform) * cam.transform.forward;
            //Vector3 direction = cam.transform.forward;
            var input = new RaycastInput(new Ray(cam.transform.position, direction), cam.farClipPlane) {
                m_ignoreBuildingFlags = Building.Flags.None,
                m_ignoreNodeFlags = NetNode.Flags.None,
                m_ignoreSegmentFlags = NetSegment.Flags.None,
                m_ignorePropFlags = PropInstance.Flags.None,
                m_buildingService = new RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default),
                m_netService = new RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default),
                m_netService2 = new RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default),
                m_propService = new RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default)
            };

            if (RayCast(input, out var rayOutput)) {
                return rayOutput.m_hitPos;
            }
            return null;
        }
    }
}

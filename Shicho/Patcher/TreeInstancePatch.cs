extern alias Cities;

using ColossalFramework;
using UnityEngine;
using Harmony;
using System;

namespace Shicho.Patcher.TreeInstancePatch
{
    using Shicho.Core;

    using TreeManager = Cities::TreeManager;
    using RenderManager = Cities::RenderManager;
    using WeatherManager = Cities::WeatherManager;
    using TreeInfo = Cities::TreeInfo;
    using TreeInstance = Cities::TreeInstance;


    [HarmonyPatch(typeof(TreeInstance))]
    [HarmonyPatch("RenderInstance")]
    [HarmonyPatch(new Type[] {
        typeof(RenderManager.CameraInfo),
        typeof(TreeInfo),
        typeof(Vector3),
        typeof(float),
        typeof(float),
    })]
    class RenderInstance
    {
        static bool Prefix(RenderManager.CameraInfo cameraInfo, TreeInfo info, Vector3 position, float scale, float brightness)
        {
            if (info.m_prefabInitialized) {
                if (cameraInfo == null || (UnityEngine.Object)info.m_lodMesh1 == (UnityEngine.Object)null || cameraInfo.CheckRenderDistance(position, info.m_lodRenderDistance)) {
                    var instance = Singleton<TreeManager>.instance;
                    if (instance == null) return false;
                    var materialBlock = instance.m_materialBlock;
                    var matrix = default(Matrix4x4);
                    matrix.SetTRS(position, Quaternion.identity, new Vector3(scale, scale, scale));

                    var color = info.m_defaultColor * brightness;
                    color.a = Singleton<WeatherManager>.instance.GetWindSpeed(position);
                    lock (App.Config.GraphicsLock) {
                        color.a *= App.Config.Graphics.treeMoveFactor;
                    }

                    materialBlock.Clear();
                    materialBlock.SetColor(instance.ID_Color, color);
                    instance.m_drawCallData.m_defaultCalls++;

                    Graphics.DrawMesh(info.m_mesh, matrix, info.m_material, info.m_prefabDataLayer, null, 0, materialBlock);

                } else {
                    position.y += info.m_generatedInfo.m_center.y * (scale - 1f);

                    var color = info.m_defaultColor * brightness;
                    color.a = Singleton<WeatherManager>.instance.GetWindSpeed(position);
                    lock (App.Config.GraphicsLock) {
                        color.a *= App.Config.Graphics.treeMoveFactor;
                    }

                    info.m_lodLocations[info.m_lodCount] = new Vector4(position.x, position.y, position.z, scale);
                    info.m_lodColors[info.m_lodCount] = color.linear;
                    info.m_lodMin = Vector3.Min(info.m_lodMin, position);
                    info.m_lodMax = Vector3.Max(info.m_lodMax, position);
                    if (++info.m_lodCount == info.m_lodLocations.Length) {
                        TreeInstance.RenderLod(cameraInfo, info);
                    }
                }
            }
            return false;
        }
    }
}

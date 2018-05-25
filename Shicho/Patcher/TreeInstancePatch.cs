extern alias Cities;

using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;

using Harmony;

using System;
using System.Reflection;


namespace Shicho.Patcher.TreeInstancePatch
{
    using Shicho.Core;

    using TreeManager = Cities::TreeManager;
    using RenderManager = Cities::RenderManager;
    using WeatherManager = Cities::WeatherManager;
    using TreeInfo = Cities::TreeInfo;
    using TreeInstance = Cities::TreeInstance;
    using System.Collections.Generic;


    [HarmonyPatch(typeof(TreeInstance))]
    [HarmonyPatch("RenderInstance")]
    [HarmonyPatch(new Type[] {
        typeof(RenderManager.CameraInfo),
        typeof(TreeInfo),
        typeof(Vector3),
        typeof(float),
        typeof(float),
        typeof(Vector4)
    })]
    class RenderInstance
    {
        static bool Prefix(ref RenderManager.CameraInfo cameraInfo, ref TreeInfo info, Vector3 position, float scale, float brightness, Vector4 objectIndex)
        {
            if (info.m_prefabInitialized) {
                if (cameraInfo == null || (UnityEngine.Object)info.m_lodMesh1 == (UnityEngine.Object)null || cameraInfo.CheckRenderDistance(position, info.m_lodRenderDistance)) {
                    var instance = Singleton<TreeManager>.instance;
                    if (instance == null) return false;
                    var materialBlock = instance.m_materialBlock;
                    var matrix = default(Matrix4x4);

                    Quaternion rotation;
                    lock (App.Config.GraphicsLock) {
                        if (App.Config.Graphics.randomTrees) {
                            #if false
                            UInt32 randIdent =
                                ((UInt32)info.GetInstanceID() << 16) |
                                (((UInt32)(BitConverter.DoubleToInt64Bits(position.magnitude) >> 32)) & 0xFFFF)
                            ;
                            var r = new System.Random((int)randIdent);
                            //Log.Debug($"tree rand: {Convert.ToString(randIdent, 2)}, {r.NextDouble() * 360}");

                            rotation = Quaternion.Euler(0, (float)r.NextDouble() * 360, 0);
                            #endif

                            // same angle as in "Random Tree Rotation" mod
                            long setofBits = BitConverter.DoubleToInt64Bits(position.magnitude);
                            rotation = Quaternion.Euler(0, setofBits % 360, 0);

                        } else {
                            rotation = Quaternion.identity;
                        }
                    }

                    matrix.SetTRS(position, rotation, new Vector3(scale, scale, scale));

                    var color = info.m_defaultColor * brightness;
                    color.a = Singleton<WeatherManager>.instance.GetWindSpeed(position);
                    lock (App.Config.GraphicsLock) {
                        color.a *= App.Config.Graphics.treeMoveFactor;
                    }

                    materialBlock.Clear();
                    materialBlock.SetColor(instance.ID_Color, color);
                    materialBlock.SetVector(instance.ID_ObjectIndex, objectIndex);
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
                    info.m_lodObjectIndices[info.m_lodCount] = objectIndex;
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

    [HarmonyPatch]
    class PopulateGroupData
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(
                typeof(TreeInstance),
                "PopulateGroupData",
                new Type[] {
                    typeof(TreeInfo),
                    typeof(Vector3),
                    typeof(float),
                    typeof(float),
                    typeof(Vector4),
                    typeof(int).MakeByRefType(),
                    typeof(int).MakeByRefType(),
                    typeof(Vector3),
                    typeof(RenderGroup.MeshData),
                    typeof(Vector3).MakeByRefType(),
                    typeof(Vector3).MakeByRefType(),
                    typeof(float).MakeByRefType(),
                    typeof(float).MakeByRefType(),
                }
            );
        }

        static bool Prefix(TreeInfo info, Vector3 position, float scale, float brightness, Vector4 objectIndex, ref int vertexIndex, ref int triangleIndex, Vector3 groupPosition, RenderGroup.MeshData data, ref Vector3 min, ref Vector3 max, ref float maxRenderDistance, ref float maxInstanceDistance)
        {
            float y = info.m_generatedInfo.m_size.y * scale;
            float num = Mathf.Max(info.m_generatedInfo.m_size.x, info.m_generatedInfo.m_size.z) * scale * 0.5f;
            min = Vector3.Min(min, position - new Vector3(num, 0f, num));
            max = Vector3.Max(max, position + new Vector3(num, y, num));
            maxRenderDistance = Mathf.Max(maxRenderDistance, 30000f);
            maxInstanceDistance = Mathf.Max(maxInstanceDistance, 425f);
            Color32 color = (info.m_defaultColor * brightness).linear;

            // We must always set this to zero on level load, since the
            // renderer always uses the default value for far trees
            // There's no way for changing this dynamically ingame, because that means
            // we recalculate 200,000 trees periodically
            lock (App.Config.GraphicsLock) {
                if (App.Config.Graphics.stopDistantTrees) {
                    color.a = 0;
                } else {
                    color.a = (byte)Mathf.Clamp(Mathf.RoundToInt(Singleton<WeatherManager>.instance.GetWindSpeed(position) * 128f), 0, 255);
                }
            }

            position -= groupPosition;
            position.y += info.m_generatedInfo.m_center.y * scale;
            data.m_vertices[vertexIndex] = position + new Vector3(info.m_renderUv0B.x * scale, info.m_renderUv0B.y * scale, 0f);
            data.m_normals[vertexIndex] = objectIndex;
            data.m_uvs[vertexIndex] = info.m_renderUv0;
            data.m_uvs2[vertexIndex] = info.m_renderUv0B * scale;
            data.m_colors[vertexIndex] = color;
            vertexIndex++;
            data.m_vertices[vertexIndex] = position + new Vector3(info.m_renderUv1B.x * scale, info.m_renderUv1B.y * scale, 0f);
            data.m_normals[vertexIndex] = objectIndex;
            data.m_uvs[vertexIndex] = info.m_renderUv1;
            data.m_uvs2[vertexIndex] = info.m_renderUv1B * scale;
            data.m_colors[vertexIndex] = color;
            vertexIndex++;
            data.m_vertices[vertexIndex] = position + new Vector3(info.m_renderUv2B.x * scale, info.m_renderUv2B.y * scale, 0f);
            data.m_normals[vertexIndex] = objectIndex;
            data.m_uvs[vertexIndex] = info.m_renderUv2;
            data.m_uvs2[vertexIndex] = info.m_renderUv2B * scale;
            data.m_colors[vertexIndex] = color;
            vertexIndex++;
            data.m_vertices[vertexIndex] = position + new Vector3(info.m_renderUv3B.x * scale, info.m_renderUv3B.y * scale, 0f);
            data.m_normals[vertexIndex] = objectIndex;
            data.m_uvs[vertexIndex] = info.m_renderUv3;
            data.m_uvs2[vertexIndex] = info.m_renderUv3B * scale;
            data.m_colors[vertexIndex] = color;
            vertexIndex++;
            data.m_triangles[triangleIndex++] = vertexIndex - 4;
            data.m_triangles[triangleIndex++] = vertexIndex - 3;
            data.m_triangles[triangleIndex++] = vertexIndex - 2;
            data.m_triangles[triangleIndex++] = vertexIndex - 2;
            data.m_triangles[triangleIndex++] = vertexIndex - 3;
            data.m_triangles[triangleIndex++] = vertexIndex - 1;

            return false;
        }
    }
}

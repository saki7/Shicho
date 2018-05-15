extern alias Cities;

using ColossalFramework;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

using Harmony;


namespace Shicho.Patcher.CameraControllerPatch
{
    using Shicho.Core;

    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("LateUpdate")]
    class LateUpdate
    {
        static void Postfix(
            ref CameraController __instance,

            // note: don't use `ref` for disabled objects
            TiltShiftEffect ___m_TiltShift,
            DepthOfField ___m_DepthOfField,
            FilmGrainEffect ___m_FilmGrain,
            SMAA ___m_SMAA,

            float ___m_DefaultMaxBlurSize
        ) {
            if (!Singleton<LoadingManager>.instance.m_loadingComplete) {
                return;
            }
            if (!InfoManager.instance || InfoManager.instance.CurrentMode != InfoManager.InfoMode.None) {
                return;
            }

            var cc = ToolsModifierControl.cameraController;
            if (cc == null) return;

            var light = App.Instance.MainLight;
            if (!light) return;

            lock (App.Config.GraphicsLock) {
                light.shadows = LightShadows.Soft;
                var gr = App.Config.Graphics;

                if (gr.shadowStrength) {
                    light.shadowStrength = gr.shadowStrength.Value;
                }
                if (gr.lightIntensity) {
                    light.intensity = gr.lightIntensity.Value;
                }

                if (___m_DepthOfField != null) {
                    //___m_DepthOfField.enabled = gr.dofEnabled;

                    // TODO
                    ___m_DepthOfField.blurSampleCount = DepthOfField.BlurSampleCount.High;
                    ___m_DepthOfField.highResolution = true;

                    ___m_DepthOfField.visualizeFocus = gr.dofDebug;
                    ___m_DepthOfField.aperture = gr.dofAperture;
                    ___m_DepthOfField.focalLength = gr.dofFocalDistance;
                    ___m_DepthOfField.focalSize = gr.dofFocalRange;

                    ___m_DepthOfField.maxBlurSize = gr.dofMaxBlurSize;

                    // near
                    ___m_DepthOfField.nearBlur = gr.dofNearBlur;
                    ___m_DepthOfField.foregroundOverlap = gr.dofFGOverlap;

                    // focus to target object
                    ___m_DepthOfField.focalTransform = null;

                    float time = Mathf.Clamp((__instance.m_currentSize - __instance.m_minDistance) / __instance.m_MaxTiltShiftDistance, 0f, 1f);
                    ___m_DepthOfField.focalLength = (__instance.m_currentSize + __instance.m_FocalLength.Evaluate(time)) * gr.dofFocalDistance;
                    ___m_DepthOfField.focalSize = __instance.m_FocalSize.Evaluate(time) * gr.dofFocalRange;
                    ___m_DepthOfField.aperture = __instance.m_Aperture.Evaluate(time) * gr.dofAperture;
                    ___m_DepthOfField.maxBlurSize = __instance.m_MaxBlurSize.Evaluate(time) * gr.dofMaxBlurSize;


                    if (___m_DepthOfField.Dx11Support()) {
                        ___m_DepthOfField.blurType = DepthOfField.BlurType.DX11;
                        ___m_DepthOfField.dx11BokehScale = gr.dofBokehScale;
                        ___m_DepthOfField.dx11BokehIntensity = gr.dofBokehIntensity;
                        ___m_DepthOfField.dx11BokehThreshold = gr.dofBokehMinLuminanceThreshold;
                        ___m_DepthOfField.dx11SpawnHeuristic = gr.dofBokehSpawnHeuristic;

                    } else {
                        ___m_DepthOfField.blurType = DepthOfField.BlurType.DiscBlur;
                    }
                }

                if (___m_TiltShift != null) {
                    //___m_TiltShift.enabled = true;
                    float time = Mathf.Clamp((__instance.m_currentSize - __instance.m_minDistance) / __instance.m_MaxTiltShiftDistance, 0f, 1f);

                    ___m_TiltShift.m_Mode = gr.tiltShiftMode;
                    ___m_TiltShift.m_MaxBlurSize = ___m_DefaultMaxBlurSize /* default 5? */ * gr.tiltShiftMaxBlurSize;
                    ___m_TiltShift.m_BlurArea = Mathf.Lerp(__instance.m_MaxTiltShiftArea, __instance.m_MinTiltShiftArea, Mathf.Clamp((__instance.m_currentSize - __instance.m_minDistance) / __instance.m_MaxTiltShiftDistance, 0f, 1f)) * gr.tiltShiftAreaSize;
                }

                if (___m_FilmGrain != null) {
                    //___m_FilmGrain.enabled = true;

                    ___m_FilmGrain.m_Scale = gr.filmGrainScale;
                    ___m_FilmGrain.m_AmountScalar = gr.filmGrainAmountScalar;
                    ___m_FilmGrain.m_Amount = gr.filmGrainAmountFactor;
                    ___m_FilmGrain.m_MiddleRange = gr.filmGrainMiddleRange;
                }

                if (___m_SMAA != null) {
                    // no "disabled" getter, so overwrite it here
                    ___m_SMAA.enabled = gr.smaaEnabled;

                    // only 1 pass is implemented?
                    //___m_SMAA.Passes = gr.smaaPasses;
                    ___m_SMAA.Passes = 1;
                }
            }
        }
    }


    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("isTiltShiftDisabled", PropertyMethod.Getter)]
    class isTiltShiftDisabledPatch
    {
        static void Postfix(ref bool __result)
        {
            lock (App.Config.GraphicsLock) {
                __result = !App.Config.Graphics.tiltShiftEnabled;
            }
        }
    }
    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("isDepthOfFieldDisabled", PropertyMethod.Getter)]
    class isDepthOfFieldDisabledPatch
    {
        static void Postfix(ref bool __result)
        {
            lock (App.Config.GraphicsLock) {
                __result = !App.Config.Graphics.dofEnabled;
            }
        }
    }
    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("isFilmGrainDisabled", PropertyMethod.Getter)]
    class isFilmGrainDisabledPatch
    {
        static void Postfix(ref bool __result)
        {
            lock (App.Config.GraphicsLock) {
                __result = !App.Config.Graphics.filmGrainEnabled;
            }
        }
    }
}

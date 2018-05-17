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
        // Some external mods override this :(
        private static class Defaults
        {
            public const float
                minDistance = 40f,
                maxDistance = 3000f,

                maxTiltDistance = 5000f, // NB: not same as below
                MaxTiltShiftDistance = 2000f, // NB: different from above value

                MinTiltShiftArea = 0f, // NB: uninitialized in vanilla
                MaxTiltShiftArea = 5.55f,

                minShadowDistance = 400f,
                maxShadowDistance = 4000f
            ;

            // TODO: make some values logarithmic
            public static readonly AnimationCurve
                FocalLength = AnimationCurve.Linear(0f, 10f, 1f, 10f),
                FocalSize = AnimationCurve.Linear(0f, 0.05f, 1f, 0.05f),
                Aperture = AnimationCurve.Linear(0f, 11.5f, 1f, 11.5f),
                MaxBlurSize = AnimationCurve.Linear(0f, 2f, 1f, 2f)
            ;
        }

        private static void ResetCameraDefaults(ref CameraController cc)
        {
            cc.m_minDistance = Defaults.minDistance;
            cc.m_maxDistance = Defaults.maxDistance;

            cc.m_maxTiltDistance = Defaults.maxTiltDistance;
            cc.m_MaxTiltShiftDistance = Defaults.MaxTiltShiftDistance;

            cc.m_MinTiltShiftArea = Defaults.MinTiltShiftArea;
            cc.m_MaxTiltShiftArea = Defaults.MaxTiltShiftArea;

            cc.m_FocalLength = Defaults.FocalLength;
            cc.m_FocalSize = Defaults.FocalSize;
            cc.m_Aperture = Defaults.Aperture;
            cc.m_MaxBlurSize = Defaults.MaxBlurSize;
        }

        static void Prefix(
            ref CameraController __instance,
            Camera ___m_camera
        ) {
            var hasToolbar = true;
            lock (App.Config.UILock) {
                hasToolbar = App.Config.UI.masterToolbarVisibility;
            }
            __instance.m_unlimitedCamera = true; // hasToolbar;

            lock (App.Config.GraphicsLock) {
                var gr = App.Config.Graphics;

                if (gr.fieldOfView.Enabled) {
                    ___m_camera.fieldOfView = gr.fieldOfView.Value / 2f;
                }
            }

            //Log.Debug($"{ColossalFramework.UI.UIInput.hoveredComponent}");
        }

        static void Postfix(
            ref CameraController __instance,

            // note: don't use `ref` for disabled objects
            TiltShiftEffect ___m_TiltShift,
            DepthOfField ___m_DepthOfField,
            FilmGrainEffect ___m_FilmGrain,
            SMAA ___m_SMAA,

            Camera ___m_camera,
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

            float cameraTime = Mathf.Clamp((__instance.m_currentSize - __instance.m_minDistance) / __instance.m_MaxTiltShiftDistance, 0f, 1f);

            bool hasToolbar = true, alwaysFullRect = false;
            lock (App.Config.UILock) {
                hasToolbar = App.Config.UI.masterToolbarVisibility;
                alwaysFullRect = App.Config.UI.alwaysFullRect;
            }
            if (!alwaysFullRect && hasToolbar) {
                ___m_camera.rect = new Rect(0f, 0.105f, 1f, 0.895f);
                //Log.Debug($"has toolbar");

            } else {
                // Vanilla & some external mod's "free camera" thingy
                ___m_camera.rect = new Rect(0f, 0f, 1f, 1f);
                //___m_camera.pixelRect = new Rect(0, 0, 2560, 1440);
                // Log.Debug($"no toolbar: {__instance.m_currentSize} {___m_camera.orthographic} {___m_camera.orthographicSize}");
            }

            lock (App.Config.GraphicsLock) {
                var gr = App.Config.Graphics;

                light.shadows = LightShadows.Soft;

                if (gr.shadowStrength) {
                    light.shadowStrength = gr.shadowStrength.Value;
                }
                if (gr.lightIntensity) {
                    light.intensity = gr.lightIntensity.Value;
                }

                // NB: revert values set from external mods
                if (gr.dofEnabled || gr.tiltShiftEnabled) {
                    ResetCameraDefaults(ref __instance);
                }

                if (___m_DepthOfField != null) {
                    //___m_DepthOfField.enabled = gr.dofEnabled;

                    // TODO
                    ___m_DepthOfField.blurSampleCount = DepthOfField.BlurSampleCount.High;
                    ___m_DepthOfField.highResolution = true;

                    ___m_DepthOfField.visualizeFocus = gr.dofDebug;

                    // near
                    ___m_DepthOfField.nearBlur = gr.dofNearBlur;
                    ___m_DepthOfField.foregroundOverlap = gr.dofFGOverlap;

                    // focus to target object
                    ___m_DepthOfField.focalTransform = null;

                    ___m_DepthOfField.focalLength = (__instance.m_currentSize + __instance.m_FocalLength.Evaluate(cameraTime)) * gr.dofFocalDistance;
                    ___m_DepthOfField.focalSize = __instance.m_FocalSize.Evaluate(cameraTime) * gr.dofFocalRange;
                    ___m_DepthOfField.aperture = __instance.m_Aperture.Evaluate(cameraTime) * gr.dofAperture;
                    ___m_DepthOfField.maxBlurSize = __instance.m_MaxBlurSize.Evaluate(cameraTime) * gr.dofMaxBlurSize;


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

                    ___m_TiltShift.m_Mode = gr.tiltShiftMode;
                    ___m_TiltShift.m_MaxBlurSize = ___m_DefaultMaxBlurSize /* default 5? */ * gr.tiltShiftMaxBlurSize;
                    ___m_TiltShift.m_BlurArea = Mathf.Lerp(__instance.m_MaxTiltShiftArea, __instance.m_MinTiltShiftArea, cameraTime) * gr.tiltShiftAreaSize;
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

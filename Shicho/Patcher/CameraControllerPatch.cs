extern alias Cities;

using Shicho.Core;

using ColossalFramework;
using ColossalFramework.UI;

using UnityEngine;
using UnityStandardAssets.ImageEffects;

using Harmony;

using System;
using System.Reflection;


namespace Shicho.Patcher.CameraControllerPatch
{
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
            CameraController __instance,
            Camera ___m_camera
        ) {
            __instance.m_unlimitedCamera = true; // hasToolbar;

            {
                // suppress Harmony bug for untouched values
                var dummy = ___m_camera.fieldOfView;
            }

            lock (App.Config.GraphicsLock) {
                var gr = App.Config.Graphics;

                if (gr.fieldOfView.Enabled) {
                    ___m_camera.fieldOfView = gr.fieldOfView.Value / 2f;
                }
            }

            //if (__instance.m_freeCamera) {
            //    __instance.SetOverrideModeOff();
            //}

            //Log.Debug($"{ColossalFramework.UI.UIInput.hoveredComponent}");
        }

        static void Postfix(
            CameraController __instance,

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
                // Log.Debug($"no toolbar: {__instance.__instance.m_currentSize} {___m_camera.orthographic} {___m_camera.orthographicSize}");
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
    [HarmonyPatch("UpdateCurrentPosition")]
    class UpdateCurrentPositionPatch
    {
        static bool Prefix(
            CameraController __instance,

            ref bool ___m_cachedFreeCamera,
            ref Vector3 ___m_cachedPosition,
            ref Vector2 ___m_cachedAngle,
            ref float ___m_cachedSize,
            ref float ___m_cachedHeight,

            ref InstanceID ___m_targetInstance,
            ref float ___m_targetTimer,

            ref bool ___m_overrideMode,
            ref Vector3 ___m_overridePos,
            ref Vector2 ___m_overrideAngle,
            ref float ___m_overrideZoom,
            ref float ___m_overrideTimer
        ) {
            if (App.Instance.IsDive) {


            } else {
                float num = (!___m_cachedFreeCamera && !Singleton<ToolManager>.instance.m_properties.m_mode.IsFlagSet(ItemClass.Availability.ThemeEditor)) ? (90f - (90f - __instance.m_targetAngle.y) * (__instance.m_maxTiltDistance * 0.5f / (__instance.m_maxTiltDistance * 0.5f + __instance.m_targetSize))) : __instance.m_targetAngle.y;
                if (___m_overrideMode) {
                    if (___m_overrideTimer != 1f) {
                        ___m_overrideTimer = Mathf.Min(1f, ___m_overrideTimer + Time.deltaTime * 2f);
                        float overrideTimer = ___m_overrideTimer;
                        overrideTimer = 3f * overrideTimer * overrideTimer - 2f * overrideTimer * overrideTimer * overrideTimer;
                        __instance.m_currentPosition = Vector3.Lerp(__instance.m_targetPosition, ___m_overridePos, overrideTimer);
                        __instance.m_currentSize = Mathf.Lerp(__instance.m_targetSize, ___m_overrideZoom, overrideTimer);
                        __instance.m_currentAngle.x = Mathf.LerpAngle(__instance.m_targetAngle.x, ___m_overrideAngle.x, overrideTimer);
                        __instance.m_currentAngle.y = Mathf.Lerp(num, ___m_overrideAngle.y, overrideTimer);
                        __instance.m_currentHeight = Mathf.Lerp(__instance.m_targetHeight, 0f, overrideTimer);

                    } else {
                        float t = Mathf.Pow(0.01f, Time.deltaTime);
                        __instance.m_currentPosition = Vector3.Lerp(___m_overridePos, __instance.m_currentPosition, t);
                        __instance.m_currentSize = Mathf.Lerp(___m_overrideZoom, __instance.m_currentSize, t);
                        __instance.m_currentAngle.x = Mathf.LerpAngle(___m_overrideAngle.x, __instance.m_currentAngle.x, t);
                        __instance.m_currentAngle.y = Mathf.Lerp(___m_overrideAngle.y, __instance.m_currentAngle.y, t);
                        __instance.m_currentHeight = Mathf.Lerp(0f, __instance.m_currentHeight, t);
                    }

                } else if (___m_overrideTimer != 1f) {
                    ___m_overrideTimer = Mathf.Min(1f, ___m_overrideTimer + Time.deltaTime * 2f);
                    float overrideTimer2 = ___m_overrideTimer;
                    overrideTimer2 = 3f * overrideTimer2 * overrideTimer2 - 2f * overrideTimer2 * overrideTimer2 * overrideTimer2;
                    __instance.m_currentPosition = Vector3.Lerp(___m_overridePos, __instance.m_targetPosition, overrideTimer2);
                    __instance.m_currentSize = Mathf.Lerp(___m_overrideZoom, __instance.m_targetSize, overrideTimer2);
                    __instance.m_currentAngle.x = Mathf.LerpAngle(___m_overrideAngle.x, __instance.m_targetAngle.x, overrideTimer2);
                    __instance.m_currentAngle.y = Mathf.Lerp(___m_overrideAngle.y, num, overrideTimer2);
                    __instance.m_currentHeight = Mathf.Lerp(0f, __instance.m_targetHeight, overrideTimer2);

                } else if (!___m_targetInstance.IsEmpty) {
                    if (___m_targetTimer != 1f) {
                        float a = (!___m_cachedFreeCamera && !Singleton<ToolManager>.instance.m_properties.m_mode.IsFlagSet(ItemClass.Availability.ThemeEditor)) ? (90f - (90f - ___m_cachedAngle.y) * (__instance.m_maxTiltDistance * 0.5f / (__instance.m_maxTiltDistance * 0.5f + ___m_cachedSize))) : ___m_cachedAngle.y;
                        ___m_targetTimer = Mathf.Min(1f, ___m_targetTimer + Time.deltaTime * 2f);
                        float targetTimer = ___m_targetTimer;
                        targetTimer = 3f * targetTimer * targetTimer - 2f * targetTimer * targetTimer * targetTimer;
                        __instance.m_currentPosition = Vector3.Lerp(___m_cachedPosition, __instance.m_targetPosition, targetTimer);
                        __instance.m_currentSize = Mathf.Lerp(___m_cachedSize, __instance.m_targetSize, targetTimer);
                        __instance.m_currentAngle.x = Mathf.LerpAngle(___m_cachedAngle.x, __instance.m_targetAngle.x, targetTimer);
                        __instance.m_currentAngle.y = Mathf.Lerp(a, num, targetTimer);
                        __instance.m_currentHeight = Mathf.Lerp(___m_cachedHeight, __instance.m_targetHeight, targetTimer);

                    } else {
                        float t2 = Mathf.Pow(1E-08f, Time.deltaTime);
                        float b = __instance.m_currentPosition.y - __instance.m_currentHeight;
                        float a2 = __instance.m_targetPosition.y - __instance.m_targetHeight;
                        b = Mathf.Lerp(a2, b, t2);
                        __instance.m_currentPosition = __instance.m_targetPosition;
                        __instance.m_currentPosition.y = __instance.m_targetHeight + b;
                        __instance.m_currentSize = Mathf.Lerp(__instance.m_targetSize, __instance.m_currentSize, t2);
                        __instance.m_currentAngle.x = Mathf.LerpAngle(__instance.m_targetAngle.x, __instance.m_currentAngle.x, t2);
                        __instance.m_currentAngle.y = Mathf.Lerp(num, __instance.m_currentAngle.y, t2);
                        __instance.m_currentHeight = __instance.m_targetHeight;
                    }

                } else {
                    float t3 = Mathf.Pow(1E-08f, Time.deltaTime);
                    __instance.m_currentPosition = Vector3.Lerp(__instance.m_targetPosition, __instance.m_currentPosition, t3);
                    __instance.m_currentSize = Mathf.Lerp(__instance.m_targetSize, __instance.m_currentSize, t3);
                    __instance.m_currentAngle.x = Mathf.LerpAngle(__instance.m_targetAngle.x, __instance.m_currentAngle.x, t3);
                    __instance.m_currentAngle.y = Mathf.Lerp(num, __instance.m_currentAngle.y, t3);
                    __instance.m_currentHeight = Mathf.Lerp(__instance.m_targetHeight, __instance.m_currentHeight, t3);
                }
            }
            return false;
        }
    }


    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("UpdateTransform")]
    class UpdateTransformPatch
    {
        static bool Prefix(
            CameraController __instance,

            Camera ___m_camera,

            float ___m_originalNearPlane,
            float ___m_originalFarPlane,

            SavedFloat ___m_ShadowsDistance
        ) {
            if (!App.Instance.IsDive) return true;

            ___m_camera.nearClipPlane = Mathf.Max(___m_originalNearPlane, __instance.m_currentSize * 0.005f);
            ___m_camera.farClipPlane = Mathf.Max(___m_originalFarPlane, __instance.m_currentSize * 2f);
            float num = __instance.m_currentSize * Mathf.Max(0f, 1f - __instance.m_currentHeight / __instance.m_maxDistance) / Mathf.Tan(0.0174532924f * ___m_camera.fieldOfView);
            var rotation = Quaternion.AngleAxis(__instance.m_currentAngle.x, Vector3.up) * Quaternion.AngleAxis(__instance.m_currentAngle.y, Vector3.right);
            var vector = __instance.m_currentPosition; // + rotation * new Vector3(0f, 0f, 0f - num);

            //float heightOfs = 0;
            //CalculateCameraHeightOffsetPatch.Prefix(__instance, ref heightOfs, vector, num);
            //vector.y += heightOfs;

            //vector = ___instance.ClampCameraPosition(vector);

            vector += __instance.m_cameraShake * Mathf.Sqrt(num);
            __instance.transform.rotation = rotation;
            __instance.transform.position = vector;

            float t = Mathf.Clamp01(num / __instance.m_maxDistance * 5f / 6f);
            float num2 = 1f;
            if (___m_ShadowsDistance.value == 0) {
                num2 = 0.7f;
            } else if (___m_ShadowsDistance.value == 1) {
                num2 = 1f;
            } else if (___m_ShadowsDistance.value == 2) {
                num2 = 2f;
            } else if (___m_ShadowsDistance.value == 3) {
                num2 = 4f;
            }

            QualitySettings.shadowDistance = Mathf.Lerp(__instance.m_minShadowDistance * num2, __instance.m_maxShadowDistance * num2, t);
            Singleton<RenderManager>.instance.CameraHeight = Mathf.Max(num * Mathf.Sin(__instance.m_currentAngle.y * 0.0174532924f), num * 0.168749988f);
            Singleton<RenderManager>.instance.ShadowDistance = 200f;
            return false;
        }
    }


    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("HandleKeyEvents")]
    class HandleKeyEventsPatch
    {
        static bool Prefix(
            CameraController __instance,
            float multiplier,

            ref Vector3 ___m_velocity,
            ref Vector2 ___m_angleVelocity,
            ref float ___m_zoomVelocity,

            SavedInputKey ___m_cameraMoveLeft,
            SavedInputKey ___m_cameraMoveRight,
            SavedInputKey ___m_cameraMoveForward,
            SavedInputKey ___m_cameraMoveBackward,

            SavedInputKey ___m_cameraRotateLeft,
            SavedInputKey ___m_cameraRotateRight,
            SavedInputKey ___m_cameraRotateUp,
            SavedInputKey ___m_cameraRotateDown,

            SavedInputKey ___m_cameraZoomAway,
            SavedInputKey ___m_cameraZoomCloser
        ) {
            if (App.Instance.IsDive) return false;

            var delta = Vector3.zero;
            var analogAction = SteamController.GetAnalogAction(SteamController.AnalogInput.CameraScroll);

            if (analogAction.sqrMagnitude > 0.0001f) {
                delta.x = analogAction.x;
                delta.z = analogAction.y;
                __instance.ClearTarget();
            }

            if (___m_cameraMoveLeft.IsPressed()) {
                delta.x -= 1f;
                __instance.ClearTarget();
            }
            if (___m_cameraMoveRight.IsPressed()) {
                delta.x += 1f;
                __instance.ClearTarget();
            }
            if (___m_cameraMoveForward.IsPressed()) {
                delta.z += 1f;
                __instance.ClearTarget();
            }
            if (___m_cameraMoveBackward.IsPressed()) {
                delta.z -= 1f;
                __instance.ClearTarget();
            }

            if (__instance.m_analogController) {
                float num = UnityEngine.Input.GetAxis("Horizontal") * __instance.m_GamepadMotionZoomScalar.x;
                float num2 = UnityEngine.Input.GetAxis("Vertical") * __instance.m_GamepadMotionZoomScalar.z;
                if (num != 0f) {
                    delta.x = num;
                }
                if (num2 != 0f) {
                    delta.z = num2;
                }
            }
            ___m_velocity += delta * multiplier * Time.deltaTime;

            var rotDelta = Vector2.zero;
            if (!__instance.m_freeCamera) {
                if (___m_cameraRotateLeft.IsPressed()) {
                    rotDelta.x += 200f;
                }
                if (___m_cameraRotateRight.IsPressed()) {
                    rotDelta.x -= 200f;
                }
                if (___m_cameraRotateUp.IsPressed()) {
                    rotDelta.y += 200f;
                }
                if (___m_cameraRotateDown.IsPressed()) {
                    rotDelta.y -= 200f;
                }
            }
            ___m_angleVelocity += rotDelta * multiplier * Time.deltaTime;
            float num3 = 0f;
            if (___m_cameraZoomCloser.IsPressed() || SteamController.GetDigitalAction(SteamController.DigitalInput.ZoomIn)) {
                num3 -= 50f;
            }
            if (___m_cameraZoomAway.IsPressed() || SteamController.GetDigitalAction(SteamController.DigitalInput.ZoomOut)) {
                num3 += 50f;
            }
            ___m_zoomVelocity += num3 * multiplier * Time.deltaTime;
            return false;
        }
    }

    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("HandleMouseEvents")]
    class HandleMouseEventsPatch
    {
        static bool Prefix()
        {
            return !App.Instance.IsDive;
        }
    }


    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("CalculateCameraHeightOffset")]
    class CalculateCameraHeightOffsetPatch
    {
        static bool Prefix(ref float __result)
        {
            if (!App.Instance.IsDive) return true;

            __result = 0;
            return false;
        }
    }

    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("UpdateFreeCamera")]
    class UpdateFreeCameraPatch
    {
        static bool Prefix(
            CameraController __instance,
            ref bool ___m_cachedFreeCamera
        ) {
            if (__instance.m_freeCamera != ___m_cachedFreeCamera) {
                ___m_cachedFreeCamera = __instance.m_freeCamera;
                UIView.Show(UIView.HasModalInput() || !__instance.m_freeCamera);

                Tool.SupportTool.ApplyVisibility();
            }
            return false;

            #if false
            if (___m_cachedFreeCamera) {
                m_camera.rect = new Rect(0f, 0f, 1f, 1f);
            } else {
                m_camera.rect = new Rect(0f, 0.105f, 1f, 0.895f);
            }
            #endif
        }
    }


    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("isTiltShiftDisabled", MethodType.Getter)]
    class isTiltShiftDisabledPatch
    {
        static bool Prefix(ref bool __result)
        {
            lock (App.Config.GraphicsLock) {
                __result = !App.Config.Graphics.tiltShiftEnabled;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("isDepthOfFieldDisabled", MethodType.Getter)]
    class isDepthOfFieldDisabledPatch
    {
        static bool Prefix(ref bool __result)
        {
            lock (App.Config.GraphicsLock) {
                __result = !App.Config.Graphics.dofEnabled;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("isFilmGrainDisabled", MethodType.Getter)]
    class isFilmGrainDisabledPatch
    {
        static bool Prefix(ref bool __result)
        {
            lock (App.Config.GraphicsLock) {
                __result = !App.Config.Graphics.filmGrainEnabled;
            }
            return false;
        }
    }
}

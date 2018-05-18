using Shicho.Core;
using Shicho.GUI;

using ColossalFramework.UI;
using UnityEngine;
using System;


namespace Shicho.Tool
{
    using ColossalFramework.Plugins;
    using ICities;
    using Shicho.GUI.Extension;
    using System.Linq;

    public static class CameraPage
    {
        public static void Setup(ref UIPanel page)
        {
            page.padding = Helper.Padding(6, 2);
            page.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 8, 0));
            page.autoFitChildrenVertically = true;

            {
                var pane = page.AddUIComponent<UIPanel>();
                pane.width = page.width - page.padding.horizontal;
                pane.padding = Helper.Padding(4, 6);
                pane.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 2, 0));
                pane.autoFitChildrenVertically = true;

                Helper.AddCheckBox(
                    ref pane,
                    "Bokeh Camera",
                    tooltip: null,
                    initialValue: App.Config.Graphics.dofEnabled,
                    (c, isChecked) => {
                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.dofEnabled = isChecked;
                        }
                    },
                    font: FontStore.Get(11)
                );

                {
                    var cb = Helper.AddCheckBox(
                        ref pane,
                        "DOF Analyzer",
                        tooltip: null,
                        initialValue: false, // always false for debugging purpose
                        (c, isChecked) => {
                            lock (App.Config.GraphicsLock) {
                                App.Config.Graphics.dofDebug = isChecked;
                            }
                        },
                        font: FontStore.Get(11)
                    );

                    cb.label.textColor = Helper.RGB(250, 40, 40);
                    cb.color = Helper.RGBA(255, 255, 255, 80);
                }
            }

            {
                var pane = page.AddUIComponent<UIPanel>();
                pane.width = page.width - page.padding.horizontal;
                pane.padding = Helper.Padding(4, 12, 4, 0);
                pane.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 2, 0));
                pane.autoFitChildrenVertically = true;

                Helper.AddLabel(
                    ref pane,
                    "Focus & Perspective",
                    font: FontStore.Get(12),
                    color: Helper.RGB(220, 230, 250),
                    bullet: "LineDetailButton"
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Field of view",
                    tooltip: null,
                    opts: new SliderOption<float>(
                        minValue: 20f,
                        maxValue: 178f,
                        stepSize: 1f,
                        defaultValue: App.Config.Graphics.fieldOfView.Value,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.fieldOfView, value);
                        }
                    ) {
                        hasField = true,
                        isEnabled = App.Config.Graphics.fieldOfView.Enabled,
                        onSwitched = App.Config.Graphics.fieldOfView.LockedSwitch(App.Config.GraphicsLock),
                    },
                    color: Helper.RGB(160, 160, 160),
                    indentPadding: 10
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Aperture",
                    $"default: {Mod.Config.GraphicsDefault.dofAperture}",
                    opts: new SliderOption<float>(
                        minValue: 0.08f,
                        maxValue: 10f,
                        stepSize: 0.01f,
                        defaultValue: App.Config.Graphics.dofAperture,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofAperture, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Focal distance",
                    $"default: {Mod.Config.GraphicsDefault.dofFocalDistance}",
                    opts: new SliderOption<float>(
                        minValue: 0.01f,
                        maxValue: 20f,
                        stepSize: 0.01f,
                        defaultValue: App.Config.Graphics.dofFocalDistance,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofFocalDistance, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Focal range",
                    $"default: {Mod.Config.GraphicsDefault.dofFocalRange}",
                    opts: new SliderOption<float>(
                        minValue: 0.5f,
                        maxValue: 10f,
                        stepSize: 0.02f,
                        defaultValue: App.Config.Graphics.dofFocalRange,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofFocalRange, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Max blur size",
                    $"default: {Mod.Config.GraphicsDefault.dofMaxBlurSize}",
                    opts: new SliderOption<float>(
                        minValue: 0f,
                        maxValue: 6f,
                        stepSize: 0.02f,
                        defaultValue: App.Config.Graphics.dofMaxBlurSize,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofMaxBlurSize, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );
            }

            // DX11 ---------------------------------------
            {
                var pane = page.AddUIComponent<UIPanel>();
                pane.width = page.width - page.padding.horizontal;
                pane.padding = Helper.Padding(4, 12, 4, 0);
                pane.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 2, 0));
                pane.autoFitChildrenVertically = true;

                Helper.AddLabel(
                    ref pane,
                    "Bokeh",
                    font: FontStore.Get(12),
                    color: Helper.RGB(220, 230, 250),
                    bullet: "PieChartWhiteFg"
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Scale",
                    $"default: {Mod.Config.GraphicsDefault.dofBokehScale}",
                    opts: new SliderOption<float>(
                        minValue: 0.1f,
                        maxValue: 50f,
                        stepSize: 0.02f,
                        defaultValue: App.Config.Graphics.dofBokehScale,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofBokehScale, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Intensity",
                    $"default: {Mod.Config.GraphicsDefault.dofBokehIntensity}",
                    opts: new SliderOption<float>(
                        minValue: 0.1f,
                        maxValue: 50f,
                        stepSize: 0.05f,
                        defaultValue: App.Config.Graphics.dofBokehIntensity,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofBokehIntensity, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Min luminance thres.",
                    $"default: {Mod.Config.GraphicsDefault.dofBokehMinLuminanceThreshold}",
                    opts: new SliderOption<float>(
                        minValue: 0.25f,
                        maxValue: 6f,
                        stepSize: 0.05f,
                        defaultValue: App.Config.Graphics.dofBokehMinLuminanceThreshold,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofBokehMinLuminanceThreshold, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Spawn frequency thres.",
                    $"default: {Mod.Config.GraphicsDefault.dofBokehSpawnHeuristic}",
                    opts: new SliderOption<float>(
                        minValue: 0.5f,
                        maxValue: 6f,
                        stepSize: 0.1f,
                        defaultValue: App.Config.Graphics.dofBokehSpawnHeuristic,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofBokehSpawnHeuristic, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );
            }

            {
                var pane = page.AddUIComponent<UIPanel>();
                pane.width = page.width - page.padding.horizontal;
                pane.padding = Helper.Padding(4, 12, 4, 0);
                pane.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 2, 0));
                pane.autoFitChildrenVertically = true;

                Helper.AddLabel(
                    ref pane,
                    "Foreground",
                    font: FontStore.Get(12),
                    color: Helper.RGB(220, 230, 250),
                    bullet: "ToolbarIconPropsPressed"
                );

                Helper.AddCheckBox(
                    ref pane,
                    "Near blur",
                    tooltip: null,
                    initialValue: App.Config.Graphics.dofNearBlur,
                    (c, isChecked) => {
                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.dofNearBlur = isChecked;
                        }
                    },
                    font: FontStore.Get(11),
                    indentPadding: 10
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Overlap amount",
                    tooltip: null,
                    opts: new SliderOption<float>(
                        minValue: 0f,
                        maxValue: 30f,
                        stepSize: 0.02f,
                        defaultValue: App.Config.Graphics.dofFGOverlap,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofFGOverlap, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );

                Helper.AddCheckBox(
                    ref pane,
                    "Tilt shift",
                    tooltip: null,
                    initialValue: App.Config.Graphics.tiltShiftEnabled,
                    (c, isChecked) => {
                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.tiltShiftEnabled = isChecked;
                        }
                    },
                    font: FontStore.Get(11),
                    indentPadding: 10
                );

                Helper.AddDropDown(
                    ref pane,
                    "Type",
                    new[] {
                        "Vertical",
                        "Radial",
                    },
                    initialValue: App.Config.Graphics.tiltShiftMode == TiltShiftEffect.TiltShiftMode.TiltShiftMode ? "Vertical" : "Radial",

                    (c, i) => {
                        var mode = TiltShiftEffect.TiltShiftMode.TiltShiftMode;

                        switch ((c as UIDropDown).selectedValue) {
                        case "Vertical":
                            mode = TiltShiftEffect.TiltShiftMode.TiltShiftMode;
                            break;

                        case "Radial":
                            mode = TiltShiftEffect.TiltShiftMode.IrisMode;
                            break;
                        }

                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.tiltShiftMode = mode;
                        }
                    }
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Max blur size",
                    null,
                    opts: new SliderOption<float>(
                        minValue: 0.02f,
                        maxValue: 12f,
                        stepSize: 0.01f,
                        defaultValue: App.Config.Graphics.tiltShiftMaxBlurSize,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.tiltShiftMaxBlurSize, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Area size",
                    null,
                    opts: new SliderOption<float>(
                        minValue: 0.12f,
                        maxValue: 30f,
                        stepSize: 0.01f,
                        defaultValue: App.Config.Graphics.tiltShiftAreaSize,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.tiltShiftAreaSize, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );
            }
        }
    }
}

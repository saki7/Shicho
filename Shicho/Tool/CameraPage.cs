using Shicho.Core;
using Shicho.GUI;

using ColossalFramework.UI;
using UnityEngine;
using System;


namespace Shicho.Tool
{
    using Shicho.GUI.Extension;

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

                {
                    var cb = Helper.AddCheckBox(ref pane, "Bokeh Camera", font: FontStore.Get(11));
                    lock (App.Config.GraphicsLock) {
                        cb.isChecked = App.Config.Graphics.dofEnabled;
                    }
                    cb.eventCheckChanged += (c, isChecked) => {
                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.dofEnabled = cb.isChecked;
                        }
                    };
                }
                {
                    var cb = Helper.AddCheckBox(ref pane, "DOF Analyzer", font: FontStore.Get(11));
                    cb.isChecked = false;
                    cb.eventCheckChanged += (c, isChecked) => {
                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.dofDebug = cb.isChecked;
                        }
                    };
                    cb.label.textColor = Helper.RGB(250, 40, 40);
                    cb.color = Helper.RGBA(255, 255, 255, 80);
                }
            }

            {
                //var aaaaName = "ContentManagerItemBackground";

                var pane = page.AddUIComponent<UIPanel>();
                pane.width = page.width - page.padding.horizontal;
                pane.padding = Helper.Padding(4, 12, 4, 0);
                pane.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 2, 0));
                pane.autoFitChildrenVertically = true;

                var l = Helper.AddLabel(
                    ref pane,
                    "Focus & Perspective",
                    font: FontStore.Get(12),
                    color: Helper.RGB(220, 230, 250),
                    bullet: "LineDetailButton"
                );

                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Aperture",
                        $"default: {Mod.Config.GraphicsDefault.dofAperture}",
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0.08f,
                            maxValue = 10f,
                            stepSize = 0.01f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofAperture, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.GraphicsLock) {
                        cfg.slider.value = App.Config.Graphics.dofAperture;
                    }
                }
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Focal distance",
                        $"default: {Mod.Config.GraphicsDefault.dofFocalDistance}",
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0.01f,
                            maxValue = 20f,
                            stepSize = 0.01f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofFocalDistance, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.GraphicsLock) {
                        cfg.slider.value = App.Config.Graphics.dofFocalDistance;
                    }
                }
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Focal range",
                        $"default: {Mod.Config.GraphicsDefault.dofFocalRange}",
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0.5f,
                            maxValue = 10f,
                            stepSize = 0.02f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofFocalRange, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.GraphicsLock) {
                        cfg.slider.value = App.Config.Graphics.dofFocalRange;
                    }
                }
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Max blur size",
                        $"default: {Mod.Config.GraphicsDefault.dofMaxBlurSize}",
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0f,
                            maxValue = 6f,
                            stepSize = 0.02f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofMaxBlurSize, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.GraphicsLock) {
                        cfg.slider.value = App.Config.Graphics.dofMaxBlurSize;
                    }
                }
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
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Scale",
                        $"default: {Mod.Config.GraphicsDefault.dofBokehScale}",
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0.1f,
                            maxValue = 50f,
                            stepSize = 0.02f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofBokehScale, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.GraphicsLock) {
                        cfg.slider.value = App.Config.Graphics.dofBokehScale;
                    }
                }
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Intensity",
                        $"default: {Mod.Config.GraphicsDefault.dofBokehIntensity}",
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0.1f,
                            maxValue = 50f,
                            stepSize = 0.05f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofBokehIntensity, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.GraphicsLock) {
                        cfg.slider.value = App.Config.Graphics.dofBokehIntensity;
                    }
                }
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Min luminance thres.",
                        $"default: {Mod.Config.GraphicsDefault.dofBokehMinLuminanceThreshold}",
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0.25f,
                            maxValue = 6f,
                            stepSize = 0.05f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofBokehMinLuminanceThreshold, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.GraphicsLock) {
                        cfg.slider.value = App.Config.Graphics.dofBokehMinLuminanceThreshold;
                    }
                }
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Spawn frequency thres.",
                        $"default: {Mod.Config.GraphicsDefault.dofBokehSpawnHeuristic}",
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0.5f,
                            maxValue = 6f,
                            stepSize = 0.1f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofBokehSpawnHeuristic, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.GraphicsLock) {
                        cfg.slider.value = App.Config.Graphics.dofBokehSpawnHeuristic;
                    }
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
                    "Foreground",
                    font: FontStore.Get(12),
                    color: Helper.RGB(220, 230, 250),
                    bullet: "ToolbarIconPropsPressed"
                );

                {
                    var cb = Helper.AddCheckBox(ref pane, "Near blur", font: FontStore.Get(11), indentPadding: 10);
                    lock (App.Config.GraphicsLock) {
                        cb.isChecked = App.Config.Graphics.dofNearBlur;
                    }
                    cb.eventCheckChanged += (c, isChecked) => {
                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.dofNearBlur = cb.isChecked;
                        }
                    };
                }
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Overlap amount",
                        $"default: {Mod.Config.GraphicsDefault.dofFGOverlap}",
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0f,
                            maxValue = 30f,
                            stepSize = 0.02f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofFGOverlap, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.GraphicsLock) {
                        cfg.slider.value = App.Config.Graphics.dofFGOverlap;
                    }
                }

                {
                    var cb = Helper.AddCheckBox(ref pane, "Tilt shift", font: FontStore.Get(11), indentPadding: 10);
                    lock (App.Config.GraphicsLock) {
                        cb.isChecked = App.Config.Graphics.tiltShiftEnabled;
                    }
                    cb.eventCheckChanged += (c, isChecked) => {
                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.tiltShiftEnabled = cb.isChecked;
                        }
                    };

                    var dd = Helper.AddDropDown(ref pane, "Type", new[] {
                            "Vertical",
                            "Radial",
                        });

                    dd.eventSelectedIndexChanged += (c, i) => {
                        var mode = TiltShiftEffect.TiltShiftMode.TiltShiftMode;

                        switch (dd.selectedValue) {
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
                    };

                    var amount = ToolHelper.AddConfig(
                        ref pane,
                        "Max blur size",
                        null,
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0.02f,
                            maxValue = 12f,
                            stepSize = 0.01f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.tiltShiftMaxBlurSize, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    var area = ToolHelper.AddConfig(
                        ref pane,
                        "Area size",
                        null,
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0.12f,
                            maxValue = 30f,
                            stepSize = 0.01f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.tiltShiftAreaSize, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );

                    lock (App.Config.GraphicsLock) {
                        dd.selectedValue = App.Config.Graphics.tiltShiftMode == TiltShiftEffect.TiltShiftMode.TiltShiftMode ? "Vertical" : "Radial";
                        amount.slider.value = App.Config.Graphics.tiltShiftMaxBlurSize;
                        area.slider.value = App.Config.Graphics.tiltShiftAreaSize;
                    }
                }
            }
        }
    }
}

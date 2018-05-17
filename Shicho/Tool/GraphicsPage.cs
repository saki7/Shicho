using Shicho.Core;
using Shicho.GUI;

using ColossalFramework.UI;
using UnityEngine;
using System;


namespace Shicho.Tool
{
    using Shicho.GUI.Extension;

    public static class GraphicsPage
    {
        public static void Setup(ref UIPanel page)
        {
            page.padding = Helper.Padding(6, 2);
            page.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 8, 0));
            page.autoFitChildrenVertically = true;

            {
                var pane = page.AddUIComponent<UIPanel>();
                pane.width = pane.parent.width - page.padding.horizontal;
                pane.padding = Helper.Padding(4, 12, 4, 0);
                pane.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 2, 0));
                pane.autoFitChildrenVertically = true;

                Helper.AddLabel(
                    ref pane,
                    "Light & Shadow",
                    font: FontStore.Get(12),
                    color: Helper.RGB(220, 230, 250),
                    bullet: "LineDetailButton"
                );

                var lightIntensity = ToolHelper.AddConfig(
                    ref pane,
                    "Light intensity",
                    "default: ≈4.2",
                    opts: new SliderOption<float>() {
                        minValue = 0.05f,
                        maxValue = 8.0f,
                        stepSize = 0.05f,
                        isEnabled = App.Config.Graphics.lightIntensity.Enabled,
                        eventSwitched = App.Config.Graphics.lightIntensity.LockedSwitch(App.Config.GraphicsLock),
                        eventValueChanged = (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.lightIntensity, value);
                        },
                    },
                    indentPadding: 12
                );

                var shadowStrength = ToolHelper.AddConfig(
                    ref pane,
                    "Shadow strength",
                    "default: 0.8",
                    opts: new SliderOption<float>() {
                        minValue = 0.1f,
                        maxValue = 1.0f,
                        stepSize = 0.05f,
                        isEnabled = App.Config.Graphics.shadowStrength.Enabled,
                        eventSwitched = App.Config.Graphics.shadowStrength.LockedSwitch(App.Config.GraphicsLock),
                        eventValueChanged = App.Config.Graphics.shadowStrength.LockedSlide(App.Config.GraphicsLock),
                    },
                    indentPadding: 12
                );

                var shadowBias = ToolHelper.AddConfig(
                    ref pane,
                    "Self-shadow mitigation",
                    "a.k.a. \"Shadow acne\" fix (default: minimal, recommended: 0.1-0.3)",
                    opts: new SliderOption<float>() {
                        minValue = 0.01f,
                        maxValue = 1.00f,
                        stepSize = 0.01f,

                        isEnabled = App.Config.Graphics.shadowBias.Enabled,
                        eventSwitched = App.Config.Graphics.shadowBias.LockedSwitch(App.Config.GraphicsLock),
                        eventValueChanged = (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.shadowBias, value);
                        },
                    },
                    indentPadding: 12
                );

                lock (App.Config.GraphicsLock) {
                    shadowStrength.slider.value = App.Config.Graphics.shadowStrength.Value;
                    lightIntensity.slider.value = App.Config.Graphics.lightIntensity.Value;
                    shadowBias.slider.value = App.Config.Graphics.shadowBias.Value;
                }

                //shadowBias_.eventKeyDown += (c, param) => {
                //    if (param.keycode == KeyCode.Return) {
                //        SyncShadowBiasInput(shadowBias_.text);
                //        SetShadowBias(shadowBiasSlider_.value);
                //    }
                //};
            }

            {
                var pane = page.AddUIComponent<UIPanel>();
                pane.width = pane.parent.width - page.padding.horizontal;
                pane.padding = Helper.Padding(4, 12, 4, 0);
                pane.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 2, 0));
                pane.autoFitChildrenVertically = true;

                Helper.AddLabel(
                    ref pane,
                    "Postprocessing",
                    font: FontStore.Get(12),
                    color: Helper.RGB(220, 230, 250),
                    bullet: "LineDetailButton"
                );

                {
                    var cb = Helper.AddCheckBox(ref pane, "SMAA", font: FontStore.Get(11), indentPadding: 10);
                    lock (App.Config.GraphicsLock) {
                        cb.isChecked = App.Config.Graphics.smaaEnabled;
                    }
                    cb.eventCheckChanged += (c, isChecked) => {
                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.smaaEnabled = cb.isChecked;
                        }
                    };
                }

                //var passes = ToolHelper.AddConfig(
                //    ref pane,
                //    "Passes",
                //    $"default: {Mod.Config.GraphicsDefault.smaaPasses}",
                //    opts: new SliderOption<float> {
                //        hasField = true,
                //        minValue = 0f,
                //        maxValue = 8f,
                //        stepSize = 1f,

                //        eventValueChanged = (c, value) => {
                //            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.smaaPasses, (int)Math.Floor(value));
                //        },
                //    },
                //    color: Helper.RGB(160, 160, 160),
                //    bullet: false
                //);
                //lock (App.Config.GraphicsLock) {
                //    passes.slider.value = App.Config.Graphics.smaaPasses;
                //}

                {
                    var cb = Helper.AddCheckBox(ref pane, "Film grain", font: FontStore.Get(11), indentPadding: 10);
                    lock (App.Config.GraphicsLock) {
                        cb.isChecked = App.Config.Graphics.filmGrainEnabled;
                    }
                    cb.eventCheckChanged += (c, isChecked) => {
                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.filmGrainEnabled = cb.isChecked;
                        }
                    };
                }
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Scale",
                        $"default: {Mod.Config.GraphicsDefault.filmGrainScale}",
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0.05f,
                            maxValue = 1f,
                            stepSize = 0.05f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.filmGrainScale, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.GraphicsLock) {
                        cfg.slider.value = App.Config.Graphics.filmGrainScale;
                    }
                }
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Amount (scalar)",
                        $"default: {Mod.Config.GraphicsDefault.filmGrainAmountScalar}",
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0.05f,
                            maxValue = 1f,
                            stepSize = 0.05f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.filmGrainAmountScalar, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.GraphicsLock) {
                        cfg.slider.value = App.Config.Graphics.filmGrainAmountScalar;
                    }
                }
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Amount (factor)",
                        $"default: {Mod.Config.GraphicsDefault.filmGrainAmountFactor}",
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0.01f,
                            maxValue = 1f,
                            stepSize = 0.01f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.filmGrainAmountFactor, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.GraphicsLock) {
                        cfg.slider.value = App.Config.Graphics.filmGrainAmountFactor;
                    }
                }
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Middle range",
                        $"default: {Mod.Config.GraphicsDefault.filmGrainMiddleRange}",
                        opts: new SliderOption<float> {
                            hasField = true,
                            minValue = 0f,
                            maxValue = 1f,
                            stepSize = 0.02f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.filmGrainMiddleRange, value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.GraphicsLock) {
                        cfg.slider.value = App.Config.Graphics.filmGrainMiddleRange;
                    }
                }
            }
        }
    }
}

extern alias Cities;
using Shicho.Core;

using ColossalFramework.UI;

using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Shicho.Tool
{
    using ColossalFramework.IO;
    using Shicho.GUI;
    using System.IO;
    using UInput = UnityEngine.Input;
    using Citizen = Cities::Citizen;
    using GUI.Extension;
    using System.Reflection;

    class SupportTool : ToolBase
    {
        public override void Awake()
        {
            base.Awake();

            Tabs = new[] {
                new TabTemplate() {
                    name = "Graphics",
                    icons = new IconSet() {
                        Normal  = "IconPolicyProHippie",
                    },
                },
                new TabTemplate() {
                    name = "Camera",
                    icons = new IconSet() {
                        Normal = "InfoPanelIconFreecamera",
                    },
                },
                new TabTemplate() {
                    name = "Citizen",
                    icons = new IconSet() {
                        Normal  = "InfoIconHappiness", // NotificationIconHappy
                    },
                },
                new TabTemplate() {
                    name = "Nature",
                    icons = new IconSet() {
                        Normal = "IconPolicyParksandRecreation",
                    },
                },
                //new TabTemplate() {
                //    name = "District",
                //    icons = new IconSet() {
                //        Normal  = "ToolbarIconDistrictDisabled",
                //        Hovered = "ToolbarIconDistrictHovered",
                //        Pressed = "ToolbarIconDistrictPressed",
                //        Focused = "ToolbarIconDistrict",
                //    },
                //},
                //new TabTemplate() {
                //    name = "Road",
                //    icons = new IconSet() {
                //        Normal  = "ToolbarIconRoadsDisabled",
                //        Hovered = "ToolbarIconRoadsHovered",
                //        Pressed = "ToolbarIconRoadsPressed",
                //        Focused = "ToolbarIconRoads",
                //    },
                //},
                new TabTemplate() {
                    name = "Misc",
                    icons = new IconSet() {
                        Normal = "Options",
                        Hovered = "OptionsHovered",
                        Pressed = "OptionsPressed",
                        Focused = "Options",
                    },
                },
                new TabTemplate() {
                    name = "About",
                    icons = new IconSet() {
                        Normal = "InfoPanelIconInfo",
                    },
                },
            };
        }

        private SliderPane AddConfig<T>(ref UIPanel page, string label, string tooltip, SliderOption<T> opts, int? labelPadding = null, int? indentPadding = null, Color32? color = null, string bullet = null)
        {
            var font = FontStore.Get(11);

            if (opts.eventSwitched != null) { // has switch
                var cb = Helper.AddCheckBox(ref page, label, tooltip, font, indentPadding: indentPadding);
                cb.eventCheckChanged += (c, isEnabled) => {
                    opts.eventSwitched.Invoke(c, isEnabled);
                };
                cb.isChecked = opts.isEnabled;

                if (labelPadding.HasValue) {
                    cb.height += labelPadding.Value;
                } else {
                    cb.height += 1;
                }

            } else {
                Helper.AddLabel(ref page, label, tooltip, font, Helper.Padding(0, 0, 2, 0), color: color, bullet: bullet);
            }

            var pane = Helper.AddSliderPane<T>(ref page, opts, font);
            return pane;
        }

        public override void Start()
        {
            base.Start();
            Title = Mod.ModInfo.ID;
            // Window.height = 420;
            Window.height = 734;

            //Log.Debug($"window: {Window.size}");
            //Log.Debug($"content: {Window.Content.size}");

            {
                var page = TabPage("Graphics");
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

                    lightIntensity_ = AddConfig(
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
                                LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.lightIntensity, value);
                            },
                        },
                        indentPadding: 12
                    );

                    var shadowStrength = AddConfig(
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

                    var shadowBias = AddConfig(
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
                                LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.shadowBias, value);
                            },
                        },
                        indentPadding: 12
                    );

                    lock (App.Config.GraphicsLock) {
                        shadowStrength.slider.value = App.Config.Graphics.shadowStrength.Value;
                        lightIntensity_.slider.value = App.Config.Graphics.lightIntensity.Value;
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

                    //var passes = AddConfig(
                    //    ref pane,
                    //    "Passes",
                    //    $"default: {Mod.Config.GraphicsDefault.smaaPasses}",
                    //    opts: new SliderOption<float> {
                    //        hasField = true,
                    //        minValue = 0f,
                    //        maxValue = 8f,
                    //        stepSize = 1f,

                    //        eventValueChanged = (c, value) => {
                    //            LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.smaaPasses, (int)Math.Floor(value));
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
                        var cfg = AddConfig(
                            ref pane,
                            "Scale",
                            $"default: {Mod.Config.GraphicsDefault.filmGrainScale}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0.05f,
                                maxValue = 1f,
                                stepSize = 0.05f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.filmGrainScale, value);
                                },
                            },
                            color: Helper.RGB(160, 160, 160)
                        );
                        lock (App.Config.GraphicsLock) {
                            cfg.slider.value = App.Config.Graphics.filmGrainScale;
                        }
                    }
                    {
                        var cfg = AddConfig(
                            ref pane,
                            "Amount (scalar)",
                            $"default: {Mod.Config.GraphicsDefault.filmGrainAmountScalar}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0.05f,
                                maxValue = 1f,
                                stepSize = 0.05f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.filmGrainAmountScalar, value);
                                },
                            },
                            color: Helper.RGB(160, 160, 160)
                        );
                        lock (App.Config.GraphicsLock) {
                            cfg.slider.value = App.Config.Graphics.filmGrainAmountScalar;
                        }
                    }
                    {
                        var cfg = AddConfig(
                            ref pane,
                            "Amount (factor)",
                            $"default: {Mod.Config.GraphicsDefault.filmGrainAmountFactor}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0.01f,
                                maxValue = 1f,
                                stepSize = 0.01f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.filmGrainAmountFactor, value);
                                },
                            },
                            color: Helper.RGB(160, 160, 160)
                        );
                        lock (App.Config.GraphicsLock) {
                            cfg.slider.value = App.Config.Graphics.filmGrainAmountFactor;
                        }
                    }
                    {
                        var cfg = AddConfig(
                            ref pane,
                            "Middle range",
                            $"default: {Mod.Config.GraphicsDefault.filmGrainMiddleRange}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0f,
                                maxValue = 1f,
                                stepSize = 0.02f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.filmGrainMiddleRange, value);
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

            {
                var page = TabPage("Camera");
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
                        var cfg = AddConfig(
                            ref pane,
                            "Aperture",
                            $"default: {Mod.Config.GraphicsDefault.dofAperture}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0.08f,
                                maxValue = 20f,
                                stepSize = 0.01f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofAperture, value);
                                },
                            },
                            color: Helper.RGB(160, 160, 160)
                        );
                        lock (App.Config.GraphicsLock) {
                            cfg.slider.value = App.Config.Graphics.dofAperture;
                        }
                    }
                    {
                        var cfg = AddConfig(
                            ref pane,
                            "Focal distance",
                            $"default: {Mod.Config.GraphicsDefault.dofFocalDistance}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0.01f,
                                maxValue = 20f,
                                stepSize = 0.01f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofFocalDistance, value);
                                },
                            },
                            color: Helper.RGB(160, 160, 160)
                        );
                        lock (App.Config.GraphicsLock) {
                            cfg.slider.value = App.Config.Graphics.dofFocalDistance;
                        }
                    }
                    {
                        var cfg = AddConfig(
                            ref pane,
                            "Focal range",
                            $"default: {Mod.Config.GraphicsDefault.dofFocalRange}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0.5f,
                                maxValue = 10f,
                                stepSize = 0.02f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofFocalRange, value);
                                },
                            },
                            color: Helper.RGB(160, 160, 160)
                        );
                        lock (App.Config.GraphicsLock) {
                            cfg.slider.value = App.Config.Graphics.dofFocalRange;
                        }
                    }
                    {
                        var cfg = AddConfig(
                            ref pane,
                            "Max blur size",
                            $"default: {Mod.Config.GraphicsDefault.dofMaxBlurSize}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0f,
                                maxValue = 6f,
                                stepSize = 0.02f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofMaxBlurSize, value);
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
                        var cfg = AddConfig(
                            ref pane,
                            "Scale",
                            $"default: {Mod.Config.GraphicsDefault.dofBokehScale}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0.1f,
                                maxValue = 50f,
                                stepSize = 0.02f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofBokehScale, value);
                                },
                            },
                            color: Helper.RGB(160, 160, 160)
                        );
                        lock (App.Config.GraphicsLock) {
                            cfg.slider.value = App.Config.Graphics.dofBokehScale;
                        }
                    }
                    {
                        var cfg = AddConfig(
                            ref pane,
                            "Intensity",
                            $"default: {Mod.Config.GraphicsDefault.dofBokehIntensity}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0.1f,
                                maxValue = 50f,
                                stepSize = 0.05f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofBokehIntensity, value);
                                },
                            },
                            color: Helper.RGB(160, 160, 160)
                        );
                        lock (App.Config.GraphicsLock) {
                            cfg.slider.value = App.Config.Graphics.dofBokehIntensity;
                        }
                    }
                    {
                        var cfg = AddConfig(
                            ref pane,
                            "Min luminance thres.",
                            $"default: {Mod.Config.GraphicsDefault.dofBokehMinLuminanceThreshold}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0.25f,
                                maxValue = 6f,
                                stepSize = 0.05f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofBokehMinLuminanceThreshold, value);
                                },
                            },
                            color: Helper.RGB(160, 160, 160)
                        );
                        lock (App.Config.GraphicsLock) {
                            cfg.slider.value = App.Config.Graphics.dofBokehMinLuminanceThreshold;
                        }
                    }
                    {
                        var cfg = AddConfig(
                            ref pane,
                            "Spawn frequency thres.",
                            $"default: {Mod.Config.GraphicsDefault.dofBokehSpawnHeuristic}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0.5f,
                                maxValue = 6f,
                                stepSize = 0.1f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofBokehSpawnHeuristic, value);
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
                        var cfg = AddConfig(
                            ref pane,
                            "Overlap amount",
                            $"default: {Mod.Config.GraphicsDefault.dofFGOverlap}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0f,
                                maxValue = 30f,
                                stepSize = 0.02f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.dofFGOverlap, value);
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

                        var amount = AddConfig(
                            ref pane,
                            "Max blur size",
                            null,
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0.02f,
                                maxValue = 12f,
                                stepSize = 0.01f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.tiltShiftMaxBlurSize, value);
                                },
                            },
                            color: Helper.RGB(160, 160, 160)
                        );
                        var area = AddConfig(
                            ref pane,
                            "Area size",
                            null,
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0.12f,
                                maxValue = 30f,
                                stepSize = 0.01f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.tiltShiftAreaSize, value);
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

            {
                var page = TabPage("Citizen");
                page.padding = Helper.Padding(6, 2);
                page.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 8, 0));
                page.autoFitChildrenVertically = true;

                // info panel
                var citizenInfo = page.AddUIComponent<CitizenInfo>();
                citizenInfo.padding = Helper.Padding(0, 10, 4, 10);

                var panel = page.AddUIComponent<UIPanel>();
                panel.width = panel.parent.width - page.padding.horizontal - 10;
                panel.SetAutoLayout(LayoutDirection.Vertical);

                var pane = AddConfig(
                    ref panel,
                    "Health literacy",
                    "Let citizens stay at their home in hope of recovery, instead of calling 911.\n Requires decent healthcare system in city.",
                    opts: new SliderOption<float>() {
                        hasField = false,

                        minValue = 0.2f,
                        maxValue = 0.95f,
                        stepSize = 0.05f,

                        isEnabled = App.Config.AI.regenChance.Enabled,
                        eventSwitched = App.Config.AI.regenChance.LockedSwitch(App.Config.AILock),
                        eventValueChanged = (c, value) => {
                            LockedApply(App.Config.AILock, ref App.Config.AI.regenChance, value);
                        },
                    },
                    labelPadding: 4,
                    indentPadding: 12
                );
                pane.slider.value = App.Config.AI.regenChance.Value;

                var tipPanel = panel.AddUIComponent<UIPanel>();
                tipPanel.SetAutoLayout(LayoutDirection.Horizontal);
                tipPanel.relativePosition = Vector2.zero;
                tipPanel.padding = Helper.Padding(0, 0, 0, 8);
                tipPanel.width = tipPanel.parent.width - (tipPanel.padding.horizontal + panel.padding.horizontal);

                {
                    var iconLabel = Helper.AddIconLabel(
                        ref tipPanel,
                        "ambulance",
                        "Call 911",
                        wrapperWidth: tipPanel.width / 2,
                        font: FontStore.Get(10),
                        color: Helper.RGB(160, 160, 160)
                    );
                }
                {
                    var iconLabel = Helper.AddIconLabel(
                        ref tipPanel,
                        "housing",
                        "Stay in bed",
                        wrapperWidth: tipPanel.width / 2,
                        font: FontStore.Get(10),
                        color: Helper.RGB(160, 160, 160),
                        isInverted: true
                    );
                }
            }

            {
                var page = TabPage("Nature");
                page.padding = Helper.Padding(8, 12);

                var pane = AddConfig(
                    ref page,
                    "Tree movement",
                    "Sets the amount of tree branch movement.\nVanilla: maximum; Recommended: minimum",
                    opts: new SliderOption<float>() {
                        hasField = false,

                        minValue = 0.0f,
                        maxValue = 1.0f,
                        stepSize = 0.1f,

                        eventValueChanged = (c, value) => {
                            LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.treeMoveFactor, value);
                        },
                    },
                    labelPadding: 4,
                    bullet: "InfoIconMaintenance"
                );
                pane.slider.value = App.Config.Graphics.treeMoveFactor;

                {
                    var cb = Helper.AddCheckBox(ref page, "Force stop distant trees (*)", "* restart required", font: FontStore.Get(11));
                    lock (App.Config.GraphicsLock) {
                        cb.isChecked = App.Config.Graphics.stopDistantTrees;
                    }
                    cb.eventCheckChanged += (c, isChecked) => {
                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.stopDistantTrees = isChecked;
                        }
                    };
                }

                {
                    var cb = Helper.AddCheckBox(ref page, "Random tree rotation", font: FontStore.Get(11));
                    lock (App.Config.GraphicsLock) {
                        cb.isChecked = App.Config.Graphics.randomTrees;
                    }
                    cb.eventCheckChanged += (c, isChecked) => {
                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.randomTrees = isChecked;
                        }
                    };
                }
            }


            {
                var page = TabPage("Misc");
                page.padding = Helper.Padding(6, 2);
                page.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 8, 0));
                page.autoFitChildrenVertically = true;

                var thmBar = UIView.Find<UISlicedSprite>("ThumbnailBar");
                var tsBar = UIView.Find("TSBar");
                var infoPanel = UIView.Find("InfoPanel");

                {
                    var pane = page.AddUIComponent<UIPanel>();
                    pane.width = page.width - page.padding.horizontal;
                    pane.padding = Helper.Padding(4, 12, 4, 0);
                    pane.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 2, 0));
                    pane.autoFitChildrenVertically = true;

                    Helper.AddLabel(
                        ref pane,
                        "Crafting UI",
                        font: FontStore.Get(12),
                        color: Helper.RGB(220, 230, 250),
                        bullet: "IconPolicyBigBusiness"
                    );

                    // debug only
                    //UIView.Show(true);

                    {
                        var cb = Helper.AddCheckBox(ref pane, "Master toolbar", font: FontStore.Get(11), indentPadding: 10);
                        lock (App.Config.UILock) {
                            // always reset to visible at game init
                            cb.isChecked = true; // App.Config.UI.masterToolbarVisibility;
                        }

                        cb.eventCheckChanged += (c, isEnabled) => {
                            lock (App.Config.UILock) {
                                App.Config.UI.masterToolbarVisibility = isEnabled;

                                //var gamePanels = UIView.library.m_DynamicPanels
                                //    .Select((p) => p.instance.GetRootCustomControl())
                                //    .Distinct()
                                //    .Where(p =>
                                //        Game.Application.GamePanels.Any(name => name == p.name)
                                //    )
                                //;

                                if (isEnabled) {
                                    thmBar.Show();
                                    tsBar.Show();
                                    infoPanel.Show();
                                    //UIView.Show(true);

                                } else {
                                    thmBar.Hide();
                                    tsBar.Hide();
                                    infoPanel.Hide();
                                    //UIView.Show(false);
                                }
                            }
                        };
                    }
                    {
                        var cfg = AddConfig(
                            ref pane,
                            "Master toolbar opacity",
                            $"default: {Mod.Config.UIDefault.masterOpacity}",
                            opts: new SliderOption<float> {
                                hasField = true,
                                minValue = 0.05f,
                                maxValue = 1f,
                                stepSize = 0.05f,

                                eventValueChanged = (c, value) => {
                                    LockedApply(App.Config.UILock, ref App.Config.UI.masterOpacity, value);
                                    thmBar.opacity = value;
                                    tsBar.opacity = value;
                                    infoPanel.opacity = value;
                                },
                            },
                            color: Helper.RGB(160, 160, 160)
                        );
                        lock (App.Config.UILock) {
                            cfg.slider.value = App.Config.UI.masterOpacity;
                        }
                    }
                    {
                        var cb = Helper.AddCheckBox(ref pane, "Prop marker", font: FontStore.Get(11), indentPadding: 10);
                        lock (App.Config.UILock) {
                            cb.isChecked = Cities::PropManager.instance.MarkersVisible = App.Config.UI.propMarkersVisibility;
                        }
                        cb.eventCheckChanged += (c, isChecked) => {
                            lock (App.Config.UILock) {
                                Cities::PropManager.instance.MarkersVisible = App.Config.UI.propMarkersVisibility = isChecked;
                            }
                        };
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
                        "City view",
                        font: FontStore.Get(12),
                        color: Helper.RGB(220, 230, 250),
                        bullet: "ToolbarIconZoomOutGlobe"
                    );

                    {
                        var cb = Helper.AddCheckBox(ref pane, "District name", font: FontStore.Get(11), indentPadding: 10);
                        lock (App.Config.UILock) {
                            cb.isChecked = DistrictManager.instance.NamesVisible = App.Config.UI.districtNamesVisibility;
                        }
                        cb.eventCheckChanged += (c, isChecked) => {
                            lock (App.Config.UILock) {
                                DistrictManager.instance.NamesVisible = App.Config.UI.districtNamesVisibility = isChecked;
                            }
                        };
                    }
                    {
                        var cb = Helper.AddCheckBox(ref pane, "City border", font: FontStore.Get(11), indentPadding: 10);
                        lock (App.Config.UILock) {
                            cb.isChecked = GameAreaManager.instance.BordersVisible = App.Config.UI.areaBordersVisiblity;
                        }
                        cb.eventCheckChanged += (c, isChecked) => {
                            lock (App.Config.UILock) {
                                GameAreaManager.instance.BordersVisible = App.Config.UI.areaBordersVisiblity = isChecked;
                            }
                        };
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
                        "Game system interface",
                        font: FontStore.Get(12),
                        color: Helper.RGB(220, 230, 250),
                        bullet: "InfoIconMaintenance"
                    );

                    {
                        var cb = Helper.AddCheckBox(ref pane, "Notifications", font: FontStore.Get(11), indentPadding: 10);
                        lock (App.Config.UILock) {
                            cb.isChecked = NotificationManager.instance.NotificationsVisible = App.Config.UI.notificationsVisibility;
                        }
                        cb.eventCheckChanged += (c, isChecked) => {
                            lock (App.Config.UILock) {
                                NotificationManager.instance.NotificationsVisible = App.Config.UI.notificationsVisibility = isChecked;

                                // speed up easing animation
                                if (!NotificationManager.instance.NotificationsVisible) {
                                    var m_notificationAlpha = typeof(NotificationManager).GetField("m_notificationAlpha", BindingFlags.NonPublic | BindingFlags.Instance);
                                    m_notificationAlpha.SetValue(NotificationManager.instance, NotificationManager.instance.NotificationsVisible ? 1f : 0f);
                                }
                            }
                        };
                    }
                    {
                        var cb = Helper.AddCheckBox(ref pane, "Tutorial", font: FontStore.Get(11), indentPadding: 10);
                        lock (App.Config.UILock) {
                            cb.isChecked = !(GuideManager.instance.TutorialDisabled = App.Config.UI.tutorialDisabled);
                        }
                        cb.eventCheckChanged += (c, isChecked) => {
                            lock (App.Config.UILock) {
                                GuideManager.instance.TutorialDisabled = App.Config.UI.tutorialDisabled = !isChecked;
                            }
                        };
                    }
                }
                // "IconPolicySmallBusiness"
            }

            {
                var page = TabPage("About");
                page.padding = Helper.Padding(8, 12);

                var version = page.AddUIComponent<UILabel>();
                var font = Instantiate(version.font);
                font.size = 12;

                version.font = font;
                version.text = $"Mod version: {Mod.ModInfo.Version}";

                void AddBar()
                {
                    var bar = page.AddUIComponent<UISprite>();
                    bar.width = bar.parent.width;
                    bar.height = 3;
                    bar.spriteName = "RocketProgressBarFill";
                    bar.color = Helper.RGB(80, 80, 80);
                }

                void AddInfo(string name, string value)
                {
                    var label = page.AddUIComponent<UILabel>();
                    label.font = version.font;
                    label.text = $"{name}: {value}";
                }

                AddBar();
                AddInfo("Game version", Cities::BuildConfig.applicationVersion);

                AddBar();
                AddInfo("RAM", Util.ToByteUnits((Int64)SystemInfo.systemMemorySize * 1024 * 1024));
                AddInfo("VRAM", Util.ToByteUnits((Int64)SystemInfo.graphicsMemorySize * 1024 * 1024));
            }

            //Log.Info($"ffff: {Config.SelectedTabIndex}");
            SelectTab(Config.SelectedTabIndex);
            Window.Icon = Resources.shicho_logo_outline_white_24;

            {
                var disabledCover = Window.AddUIComponent<UIPanel>();
                disabledCover.isVisible = false;
                disabledCover.pivot = UIPivotPoint.TopLeft;
                //disabledCover.anchor = UIAnchorStyle.Left | UIAnchorStyle.Top;
                disabledCover.name = "disabledCover";
                disabledCover.relativePosition = new Vector2(0, Window.TitleBarHeight);
                //disabledCover.isVisible = !isToolActive;
                //disabledCover.FitTo(Window.Content);
                disabledCover.size = new Vector2(
                    Window.Content.width, Window.Content.height
                );

                disabledCover.backgroundSprite = "AssetEditorItemBackgroundPressed";
                disabledCover.color = Helper.RGBA(235, 235, 255, 220);

                var sprite = disabledCover.AddUIComponent<UISprite>();
                sprite.spriteName = "IconWarning";
                sprite.pivot = UIPivotPoint.TopCenter;
                sprite.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.CenterVertical;
                sprite.disabledColor = Helper.RGB(0, 255, 128);
                sprite.size = new Vector2(64, 64);

                var label = Helper.AddLabel(
                    ref disabledCover,
                    "External tool is active",
                    font: FontStore.Get(18),
                    color: Helper.RGB(20, 20, 20)
                );
                label.padding = Helper.Padding(80, 0, 0, 0);
                label.pivot = UIPivotPoint.TopCenter;
                label.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.CenterVertical;
                label.relativePosition = new Vector2(0, disabledCover.height / 2);
                label.textColor = Helper.RGBA(0, 0, 0, 255);

                //Log.Debug($"{disabledCover.position}, {disabledCover.size}");

                //Window.eventPositionChanged += (c, pos) => {
                //    disabledCover.absolutePosition = c.absolutePosition;
                //    Log.Debug($"dc: {c.absolutePosition}, {Window.absolutePosition}");
                //};
                //Window.eventSizeChanged += (c, size) => {
                //    disabledCover.size = size;
                //    Log.Debug($"dasefawaw: {c.size}, {Window.size}");
                //};

                //Window.Content.eventIsEnabledChanged += (c, flag) => {
                //    disabledCover.isVisible = isToolActive;
                //};
            }

            //Window.Content.FitChildren();
            //Log.Debug($"fff: {Window.titleBar_.size} {Window.Content.size}");
            Window.Content.clipChildren = true;

            //Window.FitTo(Window.Content);
            Window.clipChildren = false;
            Window.Show();
        }

        public void Update()
        {
            var keyMod = Input.KeyMod.None;

            if (UInput.GetKey(KeyCode.LeftControl) || UInput.GetKey(KeyCode.RightControl)) {
                keyMod |= Input.KeyMod.Ctrl;
            }
            if (UInput.GetKey(KeyCode.LeftAlt) || UInput.GetKey(KeyCode.RightAlt)) {
                keyMod |= Input.KeyMod.Alt;
            }
            if (UInput.GetKey(KeyCode.LeftShift) || UInput.GetKey(KeyCode.RightShift)) {
                keyMod |= Input.KeyMod.Shift;
            }

            if (UInput.GetKeyDown(App.Config.mainKey.Code)) {
                if ((App.Config.mainKey.Mod & keyMod) == App.Config.mainKey.Mod) {
                    lock (App.Config.UILock) {
                        // should be visible this time?
                        if (!Config.IsVisible) {
                            if (!App.Config.UI.masterToolbarVisibility) {
                                App.Config.UI.masterToolbarVisibility = true;
                                UIView.Show(true);
                            }
                            SetVisible(true);
                        } else {
                            SetVisible(false);
                        }
                    }
                }
            }
        }

        public void LateUpdate()
        {
            //if (Window.Content.isEnabled == isToolActive) {
            //    Window.Content.isEnabled = !isToolActive;
            //}
        }

        private void LockedApply<T>(object lockObj, ref Mod.Config.Switchable<T> target, T value)
        {
            // don't change the switch...
            lock (lockObj) {
                target.Value = value;
            }
        }

        private void LockedApply<T>(object lockObj, ref T target, T value)
        {
            //Log.Debug($"LockedApply: {value}");
            lock (lockObj) {
                target = value;
            }
        }

        public static readonly Rect DefaultRect = new Rect(
            0f, 0f, 280f, 734f
        );

        public override TabbedWindowConfig ConfigProxy {
            get => App.Config.GUI.SupportTool;
        }

        private bool isToolActive { get => Cities::InfoManager.instance.CurrentMode != Cities::InfoManager.InfoMode.None; }

        private SliderPane lightIntensity_;
    }
}

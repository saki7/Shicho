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

    class SupportTool : ToolBase
    {
        public override void Awake()
        {
            base.Awake();

            Tabs = new[] {
                new TabTemplate() {
                    name = "Rendering",
                    icons = new IconSet() {
                        Normal  = "IconPolicyProHippie",
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
                //new TabTemplate() {
                //    name = "Misc",
                //    icons = new IconSet() {
                //        Normal = "OptionsDisabled",
                //        Hovered = "OptionsHovered",
                //        Pressed = "OptionsPressed",
                //        Focused = "Options",
                //    },
                //},
                new TabTemplate() {
                    name = "About",
                    icons = new IconSet() {
                        Normal = "InfoPanelIconInfo",
                    },
                },
            };
        }

        struct SliderOption
        {
            public float minValue, maxValue, stepSize;
            public PropertyChangedEventHandler<float> eventValueChanged;
        }

        private void AddConfig(out UISlider slider, out UITextField field, ref UIPanel page, string label, string tooltip, SliderOption sliderOption)
        {
            {
                var labelObj = page.AddUIComponent<UILabel>();
                labelObj.text = label;
                labelObj.tooltip = tooltip;
                labelObj.font = Instantiate(labelObj.font);
                labelObj.font.size = 12;
                labelObj.padding.bottom = 2;
            }

            {
                var panel = page.AddUIComponent<UIPanel>();
                panel.width = panel.parent.width - page.padding.horizontal;
                panel.autoSize = false;
                panel.autoLayout = false;
                panel.autoLayoutDirection = LayoutDirection.Horizontal;
                //panel.backgroundSprite = "Menubar";
                panel.pivot = UIPivotPoint.MiddleLeft;

                var sliderObj = panel.AddUIComponent<UISlider>();
                slider = sliderObj;

                sliderObj.autoSize = true;
                sliderObj.relativePosition = Vector2.zero;
                sliderObj.pivot = UIPivotPoint.MiddleLeft;
                sliderObj.anchor = UIAnchorStyle.Left | UIAnchorStyle.CenterVertical;

                sliderObj.minValue = sliderOption.minValue;
                sliderObj.maxValue = sliderOption.maxValue;
                sliderObj.stepSize = sliderOption.stepSize;
                sliderObj.scrollWheelAmount = sliderObj.stepSize * 2 + float.Epsilon;
                sliderObj.backgroundSprite = "BudgetSlider";

                {
                    var thumb = slider.AddUIComponent<UISprite>();
                    slider.thumbObject = thumb;
                    thumb.spriteName = "SliderBudget";
                    sliderObj.height = thumb.height + 8;
                    sliderObj.thumbOffset = new Vector2(1, 1);
                }
                sliderObj.width = page.width - 88;

                var fieldObj = panel.AddUIComponent<UITextField>();
                field = fieldObj;

                fieldObj.autoSize = false;
                fieldObj.width = page.width - sliderObj.width - page.padding.horizontal - 20;
                fieldObj.relativePosition = new Vector2(fieldObj.parent.width - fieldObj.width, 0);
                fieldObj.anchor = UIAnchorStyle.CenterVertical | UIAnchorStyle.Right;
                fieldObj.height -= 4;

                fieldObj.readOnly = false;
                fieldObj.builtinKeyNavigation = true;

                fieldObj.numericalOnly = true;
                fieldObj.allowFloats = true;

                fieldObj.canFocus = true;
                fieldObj.selectOnFocus = true;
                fieldObj.submitOnFocusLost = true;

                fieldObj.cursorBlinkTime = 0.5f;
                fieldObj.cursorWidth = 1;
                fieldObj.selectionSprite = "EmptySprite";
                fieldObj.normalBgSprite = "TextFieldPanel";
                //field.hoveredBgSprite = "TextFieldPanelHovered";
                fieldObj.focusedBgSprite = "TextFieldPanel";

                fieldObj.clipChildren = true;

                fieldObj.colorizeSprites = true;
                fieldObj.color = new Color32(30, 30, 30, 255);
                fieldObj.textColor = new Color32(250, 250, 250, 255);
                fieldObj.font = Instantiate(fieldObj.font);
                fieldObj.font.size = 11;
                fieldObj.horizontalAlignment = UIHorizontalAlignment.Left;
                fieldObj.padding = Helper.Padding(0, 6);

                //field.padding.top -= 5;
                panel.height = fieldObj.height;

                //Log.Debug($"Page: {page.position}, {page.size}");
                //Log.Debug($"Panel: {panel.position}, {panel.size}");
                //Log.Debug($"Slider: {slider.position}, {slider.size}");
                //Log.Debug($"Field: {field.position}, {field.size}");

                sliderObj.eventValueChanged += (c, value) => {
                    fieldObj.text = value.ToString();
                    sliderOption.eventValueChanged?.Invoke(c, value);
                };

                fieldObj.eventTextSubmitted += (c, text) => {
                    try {
                        sliderObj.value = float.Parse(text);

                    } catch (Exception e) {
                        Log.Error($"failed to set new value \"{text}\": {e}");
                    }
                };
            }
        }

        public override void Start()
        {
            base.Start();
            Title = Mod.ModInfo.ID;

            {
                var page = TabPage("Rendering");
                page.padding = Helper.Padding(8, 12);
                page.clipChildren = false;
                page.autoLayout = true;
                page.autoLayoutDirection = LayoutDirection.Vertical;
                page.autoFitChildrenHorizontally = true;

                AddConfig(
                    out var shadowStrengthSlider_,
                    out var shadowStrength_,
                    ref page,
                    "Shadow strength",
                    "default: 0.8",
                    sliderOption: new SliderOption() {
                        minValue = 0.1f,
                        maxValue = 1.0f,
                        stepSize = 0.05f,
                        eventValueChanged = (c, value) => {
                            LockedApply(ref App.Config.GraphicsLock, ref App.Config.Graphics.shadowStrength, value);
                        },
                    }
                );

                AddConfig(
                    out var lightIntensitySlider_,
                    out var lightIntensity_,
                    ref page,
                    "Light intensity",
                    "default: ≈4.2",
                    sliderOption: new SliderOption() {
                        minValue = 0.05f,
                        maxValue = 8.0f,
                        stepSize = 0.05f,
                        eventValueChanged = (c, value) => {
                            LockedApply(ref App.Config.GraphicsLock, ref App.Config.Graphics.lightIntensity, value);
                        },
                    }
                );

                AddConfig(
                    out shadowBiasSlider_,
                    out shadowBias_,
                    ref page,
                    "Self-shadow mitigation",
                    "a.k.a. \"Shadow acne\" fix (default: minimal, recommended: 0.1-0.3)",
                    sliderOption: new SliderOption() {
                        minValue = 0.01f,
                        maxValue = 1.00f,
                        stepSize = 0.01f,
                        eventValueChanged = (c, value) => {
                            LockedApply(ref App.Config.GraphicsLock, ref App.Config.Graphics.shadowBias, value);
                        },
                    }
                );

                lock (App.Config.GraphicsLock) {
                    shadowStrengthSlider_.value = App.Config.Graphics.shadowStrength;
                    lightIntensitySlider_.value = App.Config.Graphics.lightIntensity;
                    shadowBiasSlider_.value = App.Config.Graphics.shadowBias;
                }

                //shadowBias_.eventKeyDown += (c, param) => {
                //    if (param.keycode == KeyCode.Return) {
                //        SyncShadowBiasInput(shadowBias_.text);
                //        SetShadowBias(shadowBiasSlider_.value);
                //    }
                //};
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
                    bar.color = new Color32(80, 80, 80, 255);
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
                    SetVisible(!Config.IsVisible);
                }
            }
        }

        private UISlider shadowBiasSlider_;
        private UITextField shadowBias_;

        private void LockedApply(ref object lockObj, ref float target, float value)
        {
            //Log.Debug($"LockedApply: {value}");
            lock (lockObj) {
                target = value;
            }
        }

        public static readonly Rect DefaultRect = new Rect(
            0f, 0f, 280f, 800f
        );

        public override TabbedWindowConfig ConfigProxy {
            get => App.Config.GUI.SupportTool;
        }
    }
}

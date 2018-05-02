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

                var label = page.AddUIComponent<UILabel>();
                label.text = "Self-shadow mitigation";
                label.tooltip = "a.k.a. \"Shadow acne\" fix";
                label.font = Instantiate(label.font);
                label.font.size = 12;
                label.padding.bottom = 2;

                {
                    var panel = page.AddUIComponent<UIPanel>();
                    panel.width = panel.parent.width - page.padding.horizontal;
                    panel.autoSize = false;
                    panel.autoLayout = false;
                    panel.autoLayoutDirection = LayoutDirection.Horizontal;
                    //panel.backgroundSprite = "Menubar";
                    panel.pivot = UIPivotPoint.MiddleLeft;

                    var slider = panel.AddUIComponent<UISlider>();
                    shadowBiasSlider_ = slider;
                    slider.autoSize = true;
                    slider.relativePosition = Vector2.zero;
                    slider.pivot = UIPivotPoint.MiddleLeft;
                    slider.anchor = UIAnchorStyle.Left | UIAnchorStyle.CenterVertical;

                    slider.minValue = 0.01f;
                    slider.maxValue = 1.00f;
                    slider.stepSize = 0.01f;
                    slider.scrollWheelAmount = slider.stepSize * 2 + float.Epsilon;
                    slider.backgroundSprite = "BudgetSlider";

                    {
                        var thumb = shadowBiasSlider_.AddUIComponent<UISprite>();
                        shadowBiasSlider_.thumbObject = thumb;
                        thumb.spriteName = "SliderBudget";
                        slider.height = thumb.height + 8;
                        slider.thumbOffset = new Vector2(1, 1);
                    }
                    slider.width = page.width - 88;

                    var field = panel.AddUIComponent<UITextField>();
                    shadowBias_ = field;
                    field.autoSize = false;
                    field.width = page.width - slider.width - page.padding.horizontal - 20;
                    field.relativePosition = new Vector2(field.parent.width - field.width, 0);
                    field.anchor = UIAnchorStyle.CenterVertical | UIAnchorStyle.Right;
                    field.height -= 4;

                    field.readOnly = false;
                    field.builtinKeyNavigation = true;

                    field.numericalOnly = true;
                    field.allowFloats = true;

                    field.canFocus = true;
                    field.selectOnFocus = true;
                    field.submitOnFocusLost = true;

                    field.cursorBlinkTime = 0.5f;
                    field.cursorWidth = 1;
                    field.selectionSprite = "EmptySprite";
                    field.normalBgSprite = "TextFieldPanel";
                    //field.hoveredBgSprite = "TextFieldPanelHovered";
                    field.focusedBgSprite = "TextFieldPanel";

                    field.clipChildren = true;

                    field.colorizeSprites = true;
                    field.color = new Color32(30, 30, 30, 255);
                    field.textColor = new Color32(250, 250, 250, 255);
                    field.font = Instantiate(field.font);
                    field.font.size = 11;
                    field.horizontalAlignment = UIHorizontalAlignment.Left;
                    field.padding = Helper.Padding(0, 6);

                    //field.padding.top -= 5;
                    panel.height = field.height;

                    //Log.Debug($"Page: {page.position}, {page.size}");
                    //Log.Debug($"Panel: {panel.position}, {panel.size}");
                    //Log.Debug($"Slider: {slider.position}, {slider.size}");
                    //Log.Debug($"Field: {field.position}, {field.size}");
                }

                shadowBiasSlider_.eventValueChanged += (c, value) => {
                    shadowBias_.text = value.ToString();
                    ApplyShadowBias(value);
                };

                lock (App.Config.GraphicsLock) {
                    shadowBiasSlider_.value = App.Config.Graphics.shadowBias;
                }

                shadowBias_.eventTextSubmitted += (c, text) => {
                    SyncShadowBiasInput(text);
                    ApplyShadowBias(shadowBiasSlider_.value);
                };
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

        private void SyncShadowBiasInput(string text)
        {
            try {
                shadowBiasSlider_.value = float.Parse(text);

            } catch (Exception e) {
                Log.Error($"failed to set new value \"{text}\": {e}");
            }
        }

        private void ApplyShadowBias(float value)
        {
            Log.Debug($"ApplyShadowBias: {value}");

            lock (App.Config.GraphicsLock) {
                App.Config.Graphics.shadowBias = value;
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

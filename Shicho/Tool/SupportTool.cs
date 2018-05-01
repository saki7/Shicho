using Shicho.Core;

using ColossalFramework.UI;

using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Shicho.Tool
{
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
            Title = $"Shicho (v{Mod.ModInfo.Version})";

            {
                var page = TabPage("Rendering");
                page.padding = Helper.Padding(8, 12);
                page.clipChildren = false;

                var label = page.AddUIComponent<UILabel>();
                label.text = "Surface shadow smoothness";
                label.tooltip = "a.k.a. \"Shadow acne\" fix";
                label.font.size = 12;
                label.padding.bottom = 8;

                {
                    var panel = page.AddUIComponent<UIPanel>();
                    panel.autoLayout = true;
                    panel.autoLayoutDirection = LayoutDirection.Horizontal;

                    var slider = panel.AddUIComponent<UISlider>();
                    shadowBiasSlider_ = slider;
                    slider.minValue = 0.01f;
                    slider.maxValue = 1.00f;
                    slider.stepSize = 0.10f;
                    slider.backgroundSprite = "BudgetSlider";
                    //slider.fillPadding.right = 14;

                    Log.Debug($"{slider.fillIndicatorObject == null}");

                    {
                        var thumb = shadowBiasSlider_.AddUIComponent<UISprite>();
                        shadowBiasSlider_.thumbObject = thumb;
                        thumb.spriteName = "SliderBudget";
                        slider.height = thumb.height - 4;
                        slider.thumbOffset = new Vector2(1, 1);
                    }
                    slider.width = page.width - 88;
                    slider.height = 8;

                    var box = panel.AddUIComponent<UITextField>();
                    shadowBias_ = box;
                    box.submitOnFocusLost = true;
                    box.isInteractive = true;
                    box.readOnly = false;
                    box.enabled = true;
                    box.builtinKeyNavigation = true;
                    box.canFocus = true;
                    box.selectOnFocus = true;
                    box.numericalOnly = true;
                    box.allowFloats = true;

                    box.cursorBlinkTime = 0.5f;
                    box.cursorWidth = 1;
                    box.selectionSprite = "EmptySprite";
                    box.normalBgSprite = "TextFieldPanel";
                    box.hoveredBgSprite = "TextFieldPanelHovered";
                    box.focusedBgSprite = "TextFieldPanel";

                    Log.Debug($"{box.maxLength}");

                    box.padding.top -= 5;

                    //box.relativePosition = new Vector2(
                    //    slider.position.x + slider.width,
                    //    slider.position.y
                    //);
                }

                shadowBiasSlider_.eventValueChanged += (c, value) => {
                    shadowBias_.text = value.ToString();
                    SetShadowBias(value);
                };
                shadowBiasSlider_.value = 0.2f; // default

                shadowBias_.eventLostFocus += (c, param) => {
                    SyncShadowBiasInput(shadowBias_.text);
                    SetShadowBias(shadowBiasSlider_.value);
                };
                shadowBias_.eventKeyDown += (c, param) => {
                    if (param.keycode == KeyCode.Return) {
                        SyncShadowBiasInput(shadowBias_.text);
                        SetShadowBias(shadowBiasSlider_.value);
                    }
                };
            }

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

        private void SetShadowBias(float value)
        {
            Log.Debug($"SetShadowBias: {value}");
        }

        class ShadowBiasMethod
        {
        }

        static class Defaults
        {
            public const int FontSize = 16;

            public const int TitleFontSize = 16;
            public const int TabFontSize = 16;

            public const float TabHeight = TabFontSize * 2.2f;
            public const float Top = TitleFontSize * 1.1f + TabHeight;
        }

        public static readonly Rect DefaultRect = new Rect(
            0f, 0f, 280f, 800f
        );

        public override TabbedWindowConfig ConfigProxy {
            get => App.Config.GUI.SupportTool;
        }
    }
}

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

    class SupportTool : ToolBase
    {
        private class CitizenInfo : UIPanel
        {
            public override void Awake()
            {
                base.Awake();
                mgr_ = Cities::CitizenManager.instance;
                ResetCount();

                updateInterval_ = 0.5f;
                elapsed_ = lastUpdatedAt_ = 0;

                datas_ = new Dictionary<string, DataPrinter>() {
                    {"Population", () => new [] {$"{counts_.total - counts_.dead}"}},
                    {"(Dead)",       () => PartialToTotal(counts_.dead)},
                    {"(Sick)",       () => PartialToTotal(counts_.sick)},
                    {"(Criminal)",   () => PartialToTotal(counts_.criminal)},
                    {"(Arrested)",   () => PartialToTotal(counts_.arrested)},
                };
            }

            private string[] PartialToTotal(uint count)
            {
                return new [] {count.ToString(), $"({(float)count / counts_.total:P1})"};
            }

            const int RowHeight = 18;

            public override void Start()
            {
                base.Start();
                width = parent.width;
                autoSize = true;

                autoLayout = true;
                autoLayoutDirection = LayoutDirection.Horizontal;
                autoLayoutPadding = Helper.ZeroOffset;

                keyCol_ = AddUIComponent<UIPanel>();
                keyCol_.autoSize = false;
                keyCol_.height = RowHeight;
                UIFont keyFont = null, valueFont = null;

                keyCol_.width = width * 0.32f;
                keyCol_.autoLayout = true;
                keyCol_.autoLayoutDirection = LayoutDirection.Vertical;

                valueCol_ = AddUIComponent<UIPanel>();
                valueCol_.width = width - keyCol_.width;
                valueCol_.autoLayout = true;
                valueCol_.autoLayoutDirection = LayoutDirection.Vertical;

                height = keyCol_.height * datas_.Keys.Count;

                foreach (var key in datas_.Keys) {
                    var keyLabel = keyCol_.AddUIComponent<UILabel>();
                    if (!keyFont) {
                        keyFont = Instantiate(keyLabel.font);
                        keyFont.size = RowHeight - 7;
                    }
                    keyLabel.textColor = new Color32(180, 180, 180, 255);
                    keyLabel.font = keyFont;
                    keyLabel.text = key;

                    var valuePanel = valueCol_.AddUIComponent<UIPanel>();
                    valuePanel.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
                    valuePanel.height = keyLabel.height;
                    valuePanel.autoLayout = true;
                    valuePanel.autoLayoutDirection = LayoutDirection.Horizontal;

                    for (int i = 0; i < 2; ++i) {
                        var valueLabel = valuePanel.AddUIComponent<UILabel>();
                        valueLabel.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;

                        valueLabel.textAlignment = UIHorizontalAlignment.Right;
                        valueLabel.width = valueLabel.parent.width / 2;
                        valueLabel.height = keyLabel.height;

                        if (i >= 1) {
                            valueLabel.padding.left = 4;
                            valueLabel.textColor = new Color32(230, 230, 230, 255);
                        }

                        // Log.Debug($"label '{key}': {valueLabel.position}, {valueLabel.size}");

                        if (!valueFont) {
                            valueFont = Instantiate(valueLabel.font);
                            valueFont.size = 12;
                        }
                        valueLabel.font = valueFont;
                    }
                }
            }

            public override void Update()
            {
                base.Update();

                elapsed_ += Time.deltaTime;
                if (elapsed_ - lastUpdatedAt_ > updateInterval_) {
                    ResetCount();

                    DataQuery.Citizens((ref Citizen c, uint id) => {
                        ++counts_.total;

                        if (c.Dead) {
                            ++counts_.dead;
                            return true;
                        } // no else-if here

                        if (c.Arrested) {
                            ++counts_.arrested;
                        }
                        if (c.Criminal) {
                            ++counts_.criminal;
                        }

                        var healthLevel = Citizen.GetHealthLevel(c.m_health);
                        if (healthLevel == Citizen.Health.Sick) {
                            ++counts_.sick;
                        }
                        return true;
                    });

                    float maxWidth = 0;
                    foreach (var e in valueCol_.components.Select((c, id) => new {c, id})) {
                        var panel = e.c as UIPanel;
                        var values = datas_[(keyCol_.components[e.id] as UILabel).text].Invoke();

                        for (int i = 0; i < values.Length; ++i) {
                            var label = panel.components[i] as UILabel;
                            label.text = values[i];
                            maxWidth = Math.Max(maxWidth, label.width);
                        }
                    }

                    foreach (var e in valueCol_.components) {
                        var label = e.components[0] as UILabel;
                        label.autoSize = false;
                        label.width = maxWidth;
                    }

                    lastUpdatedAt_ = elapsed_;
                }
            }

            private void ResetCount()
            {
                counts_ = new CountData();
            }

            private delegate string[] DataPrinter();
            Dictionary<string, DataPrinter> datas_;
            private UIPanel keyCol_, valueCol_;

            float updateInterval_, elapsed_, lastUpdatedAt_;

            public static Cities::CitizenManager mgr_;

            struct CountData
            {
                public uint total, sick, dead, arrested, criminal;
            };
            private CountData counts_;
        }

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
                new TabTemplate() {
                    name = "Citizen",
                    icons = new IconSet() {
                        Normal  = "InfoIconHappiness", // NotificationIconHappy
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
                    out lightIntensitySlider_,
                    out lightIntensity_,
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
                var page = TabPage("Citizen");
                page.padding = Helper.Padding(8, 12);
                page.autoLayout = true;
                page.autoLayoutDirection = LayoutDirection.Vertical;
                page.autoLayoutPadding = Helper.Padding(0, 0, 8, 0);

                // info panel
                page.AddUIComponent<CitizenInfo>();

                {
                    var box = Helper.AddCheckBox(
                        ref page,
                        label: "Auto-heal",
                        tooltip: "Randomly heal citizens by certain interval. Reduces ambulance usage"
                    );

                    // at last
                    box.eventCheckChanged += (c, isChecked) => {
                        SetAutoHeal(isChecked);
                    };

                    lock (App.Config.AILock) {
                        box.isChecked = App.Config.AI.doAutoHeal;
                    }
                    // Debug.Log($"{box.position}, {box.size}");
                }
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

            {
                var disabledCover = Window.AddUIComponent<UISprite>();
                disabledCover.isVisible = true;
                //disabledCover.spriteName = "ToolbarIconGroup1Disabled";
                //disabledCover.FitTo(disabledCover.parent);
                disabledCover.relativePosition = new Vector2(0, 0);
                disabledCover.size = disabledCover.parent.size;
                disabledCover.disabledColor = new Color32(0, 255, 128, 255);
                disabledCover.zOrder = 10;
                disabledCover.forceZOrder = 10;
                //disabledCover.BringToFront();

                //Log.Debug($"{disabledCover.position}, {disabledCover.size}");

                //Window.eventSizeChanged += (c, size) => {
                //    disabledCover.size = size;
                //};

                //Window.Content.eventIsEnabledChanged += (c, flag) => {
                //    disabledCover.isVisible = !flag;
                //};
            }

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

        public void LateUpdate()
        {
            var isToolActive = Cities::InfoManager.instance.CurrentMode != Cities::InfoManager.InfoMode.None;

            if (Window.Content.isEnabled == isToolActive) {
                lightIntensitySlider_.isEnabled = !isToolActive;
                Window.Content.isEnabled = !isToolActive;
            }
        }

        private void SetAutoHeal(bool doAutoHeal)
        {
            // Log.Debug($"SetAutoHeal: {doAutoHeal}");
            LockedApply(ref App.Config.AILock, ref App.Config.AI.doAutoHeal, doAutoHeal);
        }


        private UISlider shadowBiasSlider_, lightIntensitySlider_;
        private UITextField shadowBias_, lightIntensity_;

        private void LockedApply<T>(ref object lockObj, ref T target, T value)
        {
            //Log.Debug($"LockedApply: {value}");
            lock (lockObj) {
                target = value;
            }
        }

        public static readonly Rect DefaultRect = new Rect(
            0f, 0f, 280f, 256f
        );

        public override TabbedWindowConfig ConfigProxy {
            get => App.Config.GUI.SupportTool;
        }
    }
}

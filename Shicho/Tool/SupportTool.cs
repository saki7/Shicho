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

    class SupportTool : ToolBase
    {
        private class CitizenInfo : UIPanel
        {
            const int RowHeight = 18;

            public override void Awake()
            {
                base.Awake();
                ResetCount();

                mgr_ = Cities::CitizenManager.instance;
                updateInterval_ = 0.5f;
                elapsed_ = lastUpdatedAt_ = 0;

                datas_ = new Dictionary<string, DataPrinter>() {
                    {"Population",   () => new [] {$"{counts_.total - counts_.dead}"}},
                    {"(Dead)",       () => PartialToTotal(counts_.dead)},
                    {"(Sick)",       () => PartialToTotal(counts_.sick)},
                    {"(Criminal)",   () => PartialToTotal(counts_.criminal)},
                    {"(Arrested)",   () => PartialToTotal(counts_.arrested)},
                };
            }

            public override void Start()
            {
                base.Start();


                width = parent.width;
                autoSize = true;
                this.SetAutoLayout(LayoutDirection.Horizontal, Helper.ZeroOffset);

                keyCol_ = AddUIComponent<UIPanel>();
                keyCol_.autoSize = false;
                keyCol_.height = RowHeight;
                UIFont keyFont = null, valueFont = null;

                keyCol_.width = width * 0.32f;
                keyCol_.SetAutoLayout(LayoutDirection.Vertical);

                valueCol_ = AddUIComponent<UIPanel>();
                valueCol_.width = width - keyCol_.width;
                valueCol_.SetAutoLayout(LayoutDirection.Vertical);

                height = keyCol_.height * datas_.Keys.Count;

                foreach (var key in datas_.Keys) {
                    var keyLabel = keyCol_.AddUIComponent<UILabel>();
                    if (!keyFont) {
                        keyFont = Instantiate(keyLabel.font);
                        keyFont.size = RowHeight - 7;
                    }
                    keyLabel.textColor = Helper.RGB(180, 180, 180);
                    keyLabel.font = keyFont;
                    keyLabel.text = key;

                    var valuePanel = valueCol_.AddUIComponent<UIPanel>();
                    valuePanel.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
                    valuePanel.height = keyLabel.height;
                    valuePanel.SetAutoLayout(LayoutDirection.Horizontal);

                    for (int i = 0; i < 2; ++i) {
                        var valueLabel = valuePanel.AddUIComponent<UILabel>();
                        valueLabel.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;

                        valueLabel.textAlignment = UIHorizontalAlignment.Right;
                        valueLabel.width = valueLabel.parent.width / 2;
                        valueLabel.height = keyLabel.height;

                        if (i >= 1) {
                            valueLabel.padding.left = 4;
                            valueLabel.textColor = Helper.RGB(230, 230, 230);
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

                        if ((c.m_flags & Citizen.Flags.DummyTraffic) != Citizen.Flags.None) {
                            --counts_.total;
                        }

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

            private string[] PartialToTotal(uint count)
            {
                return new [] {count.ToString(), $"({(float)count / counts_.total:P1})"};
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

        private SliderPane AddConfig<T>(ref UIPanel page, string label, string tooltip, SliderOption<T> opts, int? labelPadding = null)
        {
            var font = FontStore.Get(11);

            if (opts.eventSwitched != null) { // has switch
                var cb = Helper.AddCheckBox(ref page, label, tooltip, font);
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
                Helper.AddLabel(ref page, label, tooltip, font, Helper.Padding(0, 0, 2, 0), bullet: true);
            }

            var pane = Helper.AddSliderPane<T>(ref page, opts, font);
            return pane;
        }

        public override void Start()
        {
            base.Start();
            Title = Mod.ModInfo.ID;

            //Log.Debug($"window: {Window.size}");
            //Log.Debug($"content: {Window.Content.size}");

            {
                var page = TabPage("Rendering");
                page.padding = Helper.Padding(8, 12);
                page.clipChildren = false;
                page.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 2, 0));
                page.autoFitChildrenHorizontally = true;

                var shadowStrength = AddConfig(
                    ref page,
                    "Shadow strength",
                    "default: 0.8",
                    opts: new SliderOption<float>() {
                        minValue = 0.1f,
                        maxValue = 1.0f,
                        stepSize = 0.05f,
                        isEnabled = App.Config.Graphics.shadowStrength.Enabled,
                        eventSwitched = App.Config.Graphics.shadowStrength.LockedSwitch(App.Config.GraphicsLock),
                        eventValueChanged = App.Config.Graphics.shadowStrength.LockedSlide(App.Config.GraphicsLock),
                    }
                );

                lightIntensity_ = AddConfig(
                    ref page,
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
                    }
                );

                var shadowBias = AddConfig(
                    ref page,
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
                    }
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
                var page = TabPage("Citizen");
                page.padding = Helper.Padding(8, 12);
                page.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 8, 0));

                // info panel
                page.AddUIComponent<CitizenInfo>();

                var panel = page.AddUIComponent<UIPanel>();
                panel.width = panel.parent.width - page.padding.horizontal;
                panel.SetAutoLayout(LayoutDirection.Vertical);

                var pane = AddConfig(
                    ref panel,
                    "Health literacy",
                    "Let citizens stay at their home in hope of recovery, instead of calling 911",
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
                    labelPadding: 4
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
                var disabledCover = Window.AddUIComponent<UISprite>();
                disabledCover.isVisible = true;
                //disabledCover.spriteName = "ToolbarIconGroup1Disabled";
                //disabledCover.FitTo(disabledCover.parent);
                disabledCover.relativePosition = new Vector2(0, 0);
                disabledCover.size = disabledCover.parent.size;
                disabledCover.disabledColor = Helper.RGB(0, 255, 128);
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
                lightIntensity_.slider.isEnabled = !isToolActive;
                Window.Content.isEnabled = !isToolActive;
            }
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
            0f, 0f, 280f, 256f
        );

        public override TabbedWindowConfig ConfigProxy {
            get => App.Config.GUI.SupportTool;
        }

        private SliderPane lightIntensity_;
    }
}

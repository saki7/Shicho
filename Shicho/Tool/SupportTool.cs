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

        public override void Start()
        {
            base.Start();

            Title = Mod.ModInfo.ID;
            // Window.height = 420;
            Window.size = new Vector2(DefaultRect.width, DefaultRect.height);

            //Log.Debug($"window: {Window.size}");
            //Log.Debug($"content: {Window.Content.size}");

            {
                var page = TabPage("Graphics");
                GraphicsPage.Setup(ref page);
            }
            {
                var page = TabPage("Camera");
                CameraPage.Setup(ref page);
            }
            {
                var page = TabPage("Citizen");
                CitizenPage.Setup(ref page);
            }
            {
                var page = TabPage("Nature");
                NaturePage.Setup(ref page);
            }
            {
                var page = TabPage("Misc");
                MiscPage.Setup(Window, ref page);

                lock (App.Config.UILock) {
                    if (!App.Config.UI.pauseOutline) {
                        var obj = UIView.Find<UISlicedSprite>("PauseOutline");
                        obj.color = Helper.RGBA(0, 0, 0, 0);
                    }
                }
            }
            {
                var page = TabPage("About");
                AboutPage.Setup(ref page);
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

            ApplyVisibility();

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


        public static void ApplyVisibility()
        {
            lock (App.Config.UILock) {
                Cities::NotificationManager.instance.NotificationsVisible = App.Config.UI.notificationsVisibility;
                Cities::GameAreaManager.instance.BordersVisible = App.Config.UI.areaBordersVisiblity;
                Cities::DistrictManager.instance.NamesVisible = App.Config.UI.districtNamesVisibility;
                Cities::PropManager.instance.MarkersVisible = App.Config.UI.propMarkersVisibility;
                Cities::GuideManager.instance.TutorialDisabled = App.Config.UI.tutorialDisabled;
                Cities::DisasterManager.instance.MarkersVisible = App.Config.UI.disasterVisibility;
                Cities::NetManager.instance.RoadNamesVisible = App.Config.UI.roadNameVisibility;
            }
        }

        public static readonly Rect DefaultRect = new Rect(
            0f, 0f, 300f, 778f
        );

        public override TabbedWindowConfig ConfigProxy {
            get => App.Config.GUI.SupportTool;
        }

        private bool isToolActive { get => Cities::InfoManager.instance.CurrentMode != Cities::InfoManager.InfoMode.None; }
    }
}

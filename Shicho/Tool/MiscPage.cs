extern alias Cities;

using Shicho.Core;
using Shicho.GUI;

using ColossalFramework.UI;
using UnityEngine;

using System;
using System.Reflection;


namespace Shicho.Tool
{
    using Shicho.GUI.Extension;

    public static class MiscPage
    {
        private static UISlicedSprite thmBar_;
        private static UIComponent tsBar_, infoPanel_;

        public static void Setup(Window win, ref UIPanel page)
        {
            page.padding = Helper.Padding(6, 2);
            page.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 8, 0));
            page.autoFitChildrenVertically = true;

            thmBar_ = UIView.Find<UISlicedSprite>("ThumbnailBar");
            tsBar_ = UIView.Find("TSBar");
            infoPanel_ = UIView.Find("InfoPanel");

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
                        cb.isChecked = true && infoPanel_.isVisible; // App.Config.UI.masterToolbarVisibility;
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
                                thmBar_.Show();
                                tsBar_.Show();
                                infoPanel_.Show();
                                //UIView.Show(true);

                            } else {
                                thmBar_.Hide();
                                tsBar_.Hide();
                                infoPanel_.Hide();
                                //UIView.Show(false);
                            }
                        }
                    };
                }
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        "Master toolbar opacity",
                        $"default: {Mod.Config.UIDefault.masterOpacity}",
                        opts: new SliderOption<float> {
                            hasField = false,
                            minValue = 0.05f,
                            maxValue = 1f,
                            stepSize = 0.05f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.UILock, ref App.Config.UI.masterOpacity, value);
                                SetMasterOpacity(value);
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.UILock) {
                        cfg.slider.value = App.Config.UI.masterOpacity;
                    }
                }
                {
                    var cfg = ToolHelper.AddConfig(
                        ref pane,
                        $"{Mod.ModInfo.ID} opacity",
                        $"default: {Mod.Config.UIDefault.supportToolOpacity}",
                        opts: new SliderOption<float> {
                            hasField = false,
                            minValue = 0.05f,
                            maxValue = 1f,
                            stepSize = 0.05f,

                            eventValueChanged = (c, value) => {
                                ToolHelper.LockedApply(App.Config.UILock, ref App.Config.UI.supportToolOpacity, value);
                                win.opacity = value;
                            },
                        },
                        color: Helper.RGB(160, 160, 160)
                    );
                    lock (App.Config.UILock) {
                        win.opacity = cfg.slider.value = App.Config.UI.supportToolOpacity;
                    }
                }
                {
                    var cb = Helper.AddCheckBox(ref pane, "Prop marker", font: FontStore.Get(11), indentPadding: 10);
                    cb.eventCheckChanged += (c, isChecked) => {
                        lock (App.Config.UILock) {
                            Cities::PropManager.instance.MarkersVisible = App.Config.UI.propMarkersVisibility = isChecked;
                        }
                    };
                    lock (App.Config.UILock) {
                        cb.isChecked = Cities::PropManager.instance.MarkersVisible = App.Config.UI.propMarkersVisibility;
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
                    "City view",
                    font: FontStore.Get(12),
                    color: Helper.RGB(220, 230, 250),
                    bullet: "ToolbarIconZoomOutGlobe"
                );

                {
                    var cb = Helper.AddCheckBox(ref pane, "District name", font: FontStore.Get(11), indentPadding: 10);
                    cb.eventCheckChanged += (c, isChecked) => {
                        lock (App.Config.UILock) {
                            DistrictManager.instance.NamesVisible = App.Config.UI.districtNamesVisibility = isChecked;
                        }
                    };
                    lock (App.Config.UILock) {
                        cb.isChecked = DistrictManager.instance.NamesVisible = App.Config.UI.districtNamesVisibility;
                    }
                }
                {
                    var cb = Helper.AddCheckBox(ref pane, "City border", font: FontStore.Get(11), indentPadding: 10);
                    cb.eventCheckChanged += (c, isChecked) => {
                        lock (App.Config.UILock) {
                            GameAreaManager.instance.BordersVisible = App.Config.UI.areaBordersVisiblity = isChecked;
                        }
                    };
                    lock (App.Config.UILock) {
                        cb.isChecked = GameAreaManager.instance.BordersVisible = App.Config.UI.areaBordersVisiblity;
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
        }

        private static void SetMasterOpacity(float value)
        {
            thmBar_.opacity = value;
            tsBar_.opacity = value;
            infoPanel_.opacity = value;
        }
    }
}

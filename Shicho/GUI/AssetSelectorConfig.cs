using Shicho.Core;

using ColossalFramework.UI;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shicho.GUI
{
    class AssetSelectorConfig
    {
        public AssetSelectorConfig(UITabContainer tsContainer)
        {
            tsContainer_ = tsContainer;
        }

        public bool Enabled {
            get {
                lock (App.Config.UILock) {
                    return App.Config.UI.assetSelectorEnabled;
                }
            }

            set {
                lock (App.Config.UILock) {
                    App.Config.UI.assetSelectorEnabled = value;
                    Log.Debug($"tr: {App.Config.UI.assetSelectorTransform}, {tsContainer_.absolutePosition}, {tsContainer_.size}");

                    if (value) {
                        //   "TSContainer"(UITabContainer)
                        // > "LandscapingPanel"(UIPanel)
                        // > "GTSContainer"(UITabContainer)
                        // > "PropsParksPanel"(UIPanel) >
                        // > ["FilterPanel"(UIPanel), "ScrollablePanel"(UIScrollablePanel), "Scrollbar"(UIScrollbar)]

                        tsContainer_.absolutePosition = new Vector3(
                            ConfigToX(App.Config.UI.assetSelectorTransform.x),
                            ConfigToY(App.Config.UI.assetSelectorTransform.y),
                            tsContainer_.absolutePosition.z
                        );
                        tsContainer_.size = new Vector2(
                            ConfigToWidth(App.Config.UI.assetSelectorTransform.w),
                            ConfigToHeight(App.Config.UI.assetSelectorTransform.z)
                        );

                        RefreshScrollablePanel();
                        RealignTabs();

                    } else {
                        // Mimic vanilla default
                        tsContainer_.absolutePosition = new Vector2(590.5f, 878.0f);
                        tsContainer_.size = new Vector2(859.0f, 109.0f);

                        ForEachScrollablePanel(sc => {
                            sc.wrapLayout = false;
                            sc.autoLayoutStart = LayoutStart.TopLeft;
                            sc.autoLayoutDirection = LayoutDirection.Horizontal;
                            sc.scrollPosition = Vector2.zero;
                            // sc.autoLayout = false;

                            FitScrollablePanelToParent(sc, 50);

                            //Log.Debug($"{sc.size} {sc.parent.size} {sc.parent.parent.size}");

                            if (IsFindIt(sc)) {
                                sc.scrollWheelDirection = UIOrientation.Vertical;

                            } else {
                                sc.scrollWheelDirection = UIOrientation.Horizontal;
                            }
                        });

                        RealignTabs();
                    }

                    RefreshFindIt();
                }
            }
        }

        public bool SyncFindIt {
            get {
                lock (App.Config.UILock) {
                    return App.Config.UI.assetSelectorSyncFindIt;
                }
            }
            
            set {
                lock (App.Config.UILock) {
                    App.Config.UI.assetSelectorSyncFindIt = value;
                }

                if (Enabled) {
                    lock (App.Config.UILock) {
                        RefreshScrollablePanel();
                    }
                }
            }
        }

        private static bool IsFindIt(UIScrollablePanel scrollablePanel)
        {
            return scrollablePanel.parent.name == "FindItDefaultPanel";
        }

        private void RefreshFindIt()
        {
            var root = UIView.Find<UIPanel>("FindItDefaultPanel");
            if (!root) return;

            var scrollBar = root.Find<UIScrollbar>("UIScrollbar");
            scrollBar.height = scrollBar.parent.height;
            scrollBar.relativePosition = new Vector2(scrollBar.relativePosition.x, 0);

            foreach (var comp in scrollBar.components) {
                comp.height = comp.parent.height;
            }

            var scrollablePanel = root.Find<UIScrollablePanel>("ScrollablePanel");
            scrollablePanel.width = root.width;

            var searchBoxPanel = root.Find<UIPanel>("UISearchBox");
            root.width = tsContainer_.width;
            searchBoxPanel.width = root.width;

            searchBoxPanel.components[0].relativePosition = new Vector2(300, -75);
            searchBoxPanel.components[2].width = root.width;
        }

        private static void FitScrollablePanelToParent(UIScrollablePanel sc, float relX)
        {
            // offset by scroll button margin
            sc.parent.size = sc.parent.parent.size;

            sc.relativePosition = new Vector2(relX, sc.relativePosition.y);
            sc.size = new Vector2(sc.parent.size.x - sc.relativePosition.x * 2, sc.parent.size.y);
        }

        private void RefreshScrollablePanel()
        {
            ForEachScrollablePanel(sc => {
                //Log.Debug($"sc: {sc}, FindIt?: {IsFindIt(sc)}, sync?: {App.Config.UI.assetSelectorSyncFindIt}");
                if (IsFindIt(sc)) return;

                sc.autoLayout = true;
                sc.wrapLayout = true;
                sc.autoLayoutStart = LayoutStart.TopLeft;

                FitScrollablePanelToParent(sc, 25);

                sc.scrollWheelDirection = App.Config.UI.assetSelectorScrollDirection;
                sc.autoLayoutDirection = App.Config.UI.assetSelectorScrollDirection == UIOrientation.Horizontal ? LayoutDirection.Vertical : LayoutDirection.Horizontal;
            });

            RefreshFindIt();
        }

        private delegate void ScrollablePanelProc(UIScrollablePanel sc);
        private void ForEachScrollablePanel(ScrollablePanelProc proc)
        {
            foreach (var categoryPanelComp in tsContainer_.components) {
                var categoryPanel = categoryPanelComp as UIPanel;
                if (!categoryPanel) continue;
                //Log.Debug($"categoryPanel: {categoryPanel}");

                var gts = categoryPanel.Find<UITabContainer>("GTSContainer");
                if (!gts) continue;
                //Log.Debug($"gts: {gts}");

                foreach (var panelComp in gts.components) {
                    var panel = panelComp as UIPanel;
                    if (!panel) continue;
                    // Log.Debug($"panel: {panel}");

                    var scrollable = panel.Find<UIScrollablePanel>("ScrollablePanel");
                    if (!scrollable) continue;
                    proc(scrollable);
                }
            }
        }

        private void RealignTabs()
        {
            foreach (var categoryPanelComp in tsContainer_.components) {
                var categoryPanel = categoryPanelComp as UIPanel;
                if (!categoryPanel) continue;

                var ts = categoryPanel.Find<UITabstrip>("GroupToolstrip");
                if (!ts) continue;

                ts.relativePosition = new Vector2(0, ts.relativePosition.y);

                var tabWidth = 58f;
                if ((tabWidth + 2 * 2) * ts.tabCount > tsContainer_.width) {
                    tabWidth = tsContainer_.width / ts.tabCount - 2 * 2;
                }

                foreach (var tab in ts.tabs) {
                    tab.width = tabWidth;
                }
            }
        }

        public UIOrientation ScrollDirection {
            get {
                lock (App.Config.UILock) {
                    return App.Config.UI.assetSelectorScrollDirection;
                }
            }

            set {
                lock (App.Config.UILock) {
                    App.Config.UI.assetSelectorScrollDirection = value;
                }

                if (Enabled) {
                    lock (App.Config.UILock) {
                        RefreshScrollablePanel();
                    }
                }
            }
        }

        public float X {
            get {
                lock (App.Config.UILock) {
                    return App.Config.UI.assetSelectorTransform.x;
                }
            }

            set {
                lock (App.Config.UILock) {
                    App.Config.UI.assetSelectorTransform.x = value;
                }

                if (Enabled) {
                    var newPos = tsContainer_.absolutePosition;
                    newPos.x = ConfigToX(value);
                    tsContainer_.absolutePosition = newPos;
                }
            }
        }

        public float Y {
            get {
                lock (App.Config.UILock) {
                    return App.Config.UI.assetSelectorTransform.y;
                }
            }

            set {
                lock (App.Config.UILock) {
                    App.Config.UI.assetSelectorTransform.y = value;
                }

                if (Enabled) { 
                    var newPos = tsContainer_.absolutePosition;
                    newPos.y = ConfigToY(value);
                    tsContainer_.absolutePosition = newPos;
                }
            }
        }

        public float Width {
            get {
                lock (App.Config.UILock) {
                    return App.Config.UI.assetSelectorTransform.w;
                }
            }

            set {
                lock (App.Config.UILock) {
                    App.Config.UI.assetSelectorTransform.w = value;
                }

                if (Enabled) { 
                    tsContainer_.width = ConfigToWidth(value);
                    RealignTabs();
                }
            }
        }

        public float Height {
            get {
                lock (App.Config.UILock) {
                    return App.Config.UI.assetSelectorTransform.z;
                }
            }

            set {
                lock (App.Config.UILock) {
                    App.Config.UI.assetSelectorTransform.z = value;
                }

                if (Enabled) {
                    tsContainer_.height = ConfigToHeight(value);
                }
            }
        }

        private float ConfigToX(float value) => Helper.ScreenRectAsScreen.width * value;
        private float ConfigToY(float value) => Helper.ScreenRectAsScreen.height * value;
        private float ConfigToWidth(float value) => Helper.ScreenRectAsScreen.width * value;
        private float ConfigToHeight(float value) => Helper.ScreenRectAsScreen.height * value;

        private UITabContainer tsContainer_;
    }
}

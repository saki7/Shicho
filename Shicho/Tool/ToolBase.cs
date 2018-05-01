using Shicho.Core;

using ColossalFramework.UI;
using UnityEngine;

using System;
using System.Linq;


namespace Shicho.Tool
{
    using Helper = GUI.Helper;
    using GUI.Extension;

    interface ITool : GUI.IConfigurableComponent<GUI.TabbedWindowConfig>
    {
        GUI.ConfigID ID { get; }
        GUI.TabbedWindowConfig ConfigProxy { get; }
        GUI.TabTemplate[] Tabs { get; }

        void SetVisible(bool flag);
    }

    abstract class ToolBase
        : MonoBehaviour
        , ITool
    {
        public virtual void Awake()
        {
            win_ = UIView.GetAView().AddUIComponent(typeof(GUI.Window)) as GUI.Window;
            win_.Config = ConfigProxy;

            Config.ID = new GUI.ConfigID() {
                Value = GetInstanceID(),
            };

            Config.Rect.RelocateIn(Helper.ScreenRectAsUI);
        }

        public virtual void Start()
        {
            win_.position = Config.Rect.position;
            win_.size = Config.Rect.size;

            //Log.Debug($"{win_.position}, {win_.size}");

            win_.eventClosed += (c, param) => {
                SetVisible(false);
            };
            win_.eventSizeChanged += (c, size) => {
                Config.Rect.size = size;
            };
            win_.eventPositionChanged += (c, pos) => {
                Config.Rect.position = pos;
            };

            if (Config.SelectedTabIndex > Tabs.Length) {
                Config.SelectedTabIndex = 0;
            }

            win_.Content.SetAutoLayout(LayoutDirection.Vertical);

            {
                var tabTemplate = FindObjectOfType<UITabstrip>()
                    .GetComponentInChildren<UIButton>()
                ;

                tabs_ = win_.Content.AddUIComponent<UITabstrip>();
                //tabs_.relativePosition = Vector2.zero;
                tabs_.tabPages = win_.AddUIComponent<UITabContainer>();
                //tabs_.tabContainer.position = Vector2.zero;
                tabs_.tabContainer.position = Vector2.zero;

                // tabc_.autoSize = true;
                // tabc_.autoSize = true;
                // tabc_.relativePosition = Vector3.zero;

                foreach (var v in Tabs.Select((tab, i) => new {tab, i})) {
                    {
                        var btn = tabs_.AddTab(v.tab.name, tabTemplate, true);
                        tabs_.selectedIndex = v.i; // important for setting current target obj

                        btn.tabIndex = v.i; // misc value
                        btn.relativePosition = Vector2.zero;
                        btn.text = "";
                        btn.tooltip = v.tab.name;
                        v.tab.icons.AssignTo(ref btn);

                        //btn.spritePadding = Helper.Padding(4, 22);
                        btn.width = btn.height = 32;
                    }

                    var page = tabs_.tabContainer.components[v.i] as UIPanel;
                    page.isVisible = false;

                    //Log.Debug($"{tabs_.tabContainer.absolutePosition}, {tabs_.tabContainer.position}, {tabs_.tabContainer.relativePosition}, {tabs_.tabContainer.padding}");
                    //Log.Debug($"{page.absolutePosition}, {page.position}, {page.relativePosition}, {page.padding}, {page.autoLayoutPadding}");

                    Window.Content.eventSizeChanged += (c, size) => {
                        page.width = size.x;
                    };

                    // page.relativePosition = Vector2.zero;
                    page.SetAutoLayout(LayoutDirection.Vertical);

                    //var bg = page.AddUIComponent<UITiledSprite>();
                    //bg.spriteName = "InfoPanelBack";
                    //bg.width = 200;
                    //bg.height = 400;
                    //bg.relativePosition = Vector2.zero;

                    var desc = page.AddUIComponent<UILabel>();
                    desc.text = $"'{v.tab.name}' here!!!";
                    //desc.zOrder = 1;
                }

                Log.Debug($"si: {Config.SelectedTabIndex}");
                tabs_.startSelectedIndex = Config.SelectedTabIndex;
                //tabs_.tabIndex = Config.SelectedTabIndex;
                // tabs_.selectedIndex = Config.SelectedTabIndex;
                tabs_.eventSelectedIndexChanged += (c, i) => {
                    Log.Debug($"tab: {i}");
                    Config.SelectedTabIndex = i;
                };
            }
        }

        public virtual void OnDestroy()
        {
            //Log.Debug($"OnDestroy()");
            //Destroy(tabs_);
            Destroy(win_);
        }

        public void SetVisible(bool flag)
        {
            Config.IsVisible = flag;

            if (flag) {
                Window.Show();

            } else {
                Window.Hide();
            }
        }

        public GUI.ConfigID ID { get => Config.ID; }
        public string Title {
            get => win_.Title;
            protected set => win_.Title = value;
        }

        private GUI.Window win_;
        protected GUI.Window Window { get => win_; }
        public GUI.TabbedWindowConfig Config { get => win_.Config; set => win_.Config = value; }
        public abstract GUI.TabbedWindowConfig ConfigProxy { get; }

        public GUI.TabTemplate[] Tabs { get; protected set; }
        private UITabstrip tabs_;
    }
}

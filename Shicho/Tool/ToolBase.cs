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
        GUI.TabbedWindowConfig ConfigProxy { get; set; }
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

            var scr = new Rect(0, 0, Screen.width, Screen.height);
            Config.Rect.xMin = 0;
            Config.Rect.xMax = Screen.width - Config.Rect.width;
            Config.Rect.yMin = 0;
            Config.Rect.yMax = Screen.height - Config.Rect.height;
        }

        public virtual void Start()
        {
            Config.ID = new GUI.ConfigID() {
                Value = GetInstanceID(),
            };

            win_.position = Config.Rect.position;
            win_.size = Config.Rect.size;

            win_.eventClosed += (c, param) => {
                SetVisible(false);
            };
            win_.eventSizeChanged += (c, size) => {
                Config.Rect.size = size;
            };
            win_.eventPositionChanged += (c, pos) => {
                Config.Rect.position = pos;
            };

            if (Config.TabIndex > Tabs.Length) {
                Config.TabIndex = 0;
            }

            win_.Content.SetAutoLayout(LayoutDirection.Horizontal);

            {
                tabs_ = win_.Content.AddUIComponent<UITabstrip>();
                tabs_.relativePosition = Vector2.zero;
                tabs_.tabPages = win_.AddUIComponent<UITabContainer>();

                // tabc_.autoSize = true;
                // tabc_.autoSize = true;
                // tabc_.relativePosition = Vector3.zero;

                var tabTemplate = FindObjectOfType<UITabstrip>()
                    .GetComponentInChildren<UIButton>()
                ;

                foreach (var v in Tabs.Select((tab, i) => new {tab, i})) {
                    {
                        var btn = tabs_.AddTab(v.tab.name, tabTemplate, true);
                        tabs_.selectedIndex = v.i;

                        btn.relativePosition = Vector2.zero;
                        btn.text = "";
                        btn.tooltip = v.tab.name;
                        Log.Debug($"{v.tab.icons}");
                        v.tab.icons.AssignTo(ref btn);

                        btn.normalBgSprite = "ButtonMenu";
                        btn.hoveredBgSprite = "ButtonMenuHovered";
                        btn.pressedBgSprite = "ButtonMenuPressed";
                        btn.focusedBgSprite = "ButtonMenuFocused";
                        btn.disabledBgSprite = "ButtonMenuDisabled";

                        //btn.spritePadding = Helper.Padding(4, 22);
                        btn.width = btn.height = 32;
                    }

                    var page = tabs_.tabContainer.components[v.i] as UIPanel;
                    page.isVisible = false;

                    Window.Content.eventSizeChanged += (c, size) => {
                        page.width = size.x;
                    };

                    // page.relativePosition = Vector2.zero;
                    page.SetAutoLayout(LayoutDirection.Vertical);

                    var bg = page.AddUIComponent<UITiledSprite>();
                    bg.spriteName = "InfoPanelBack";
                    bg.width = 200;
                    bg.height = 400;
                    bg.relativePosition = Vector2.zero;

                    var desc = page.AddUIComponent<UILabel>();
                    desc.text = $"'{v.tab.name}' here!!!";
                    bg.zOrder = 1;
                }

                tabs_.eventTabIndexChanged += (c, i) => {
                    //tabs_.tab
                    //tabs_.tabContainer.components[i].is
                };
            }
        }

        public virtual void OnDestroy()
        {
            ConfigProxy = Config.Clone() as GUI.TabbedWindowConfig;

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
        public abstract GUI.TabbedWindowConfig ConfigProxy { get; set; }

        public GUI.TabTemplate[] Tabs { get; protected set; }
        private UITabstrip tabs_;
    }
}

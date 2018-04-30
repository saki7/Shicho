using Shicho.Core;

using ColossalFramework.UI;
using UnityEngine;

using System;

namespace Shicho.Tool
{
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

            var nav = win_.Content.AddUIComponent<GUI.Panel>();
            nav.backgroundSprite = "IconAssetParks";
            nav.SetAutoLayout(LayoutDirection.Vertical);

            {
                tabc_ = win_.Content.AddUIComponent<UITabContainer>();

                // tabc_.autoSize = true;
                // tabc_.autoSize = true;
                // tabc_.relativePosition = Vector3.zero;

                foreach (var tab in Tabs) {
                    {
                        var btn = nav.AddUIComponent<UIButton>();
                        tab.icons.AssignTo(ref btn);
                    }

                    var page = tabc_.AddTabPage(tab.name, tab.content);
                    var desc = GUI.Helper.AddDefaultComponent<UILabel>(page);
                    desc.text = $"'{tab.name}' here!!!";
                }
                // tabc_.tabIndex = 0;
                // tabc_.selectedIndex = 0;
                // tabc_.Show();
                //tabc_.BringToFront();
            }
        }

        public virtual void OnDestroy()
        {
            ConfigProxy = Config.Clone() as GUI.TabbedWindowConfig;
            Destroy(tabc_);
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
        private UITabContainer tabc_;
    }
}

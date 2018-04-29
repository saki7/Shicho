using Shicho.Core;

using ColossalFramework.UI;
using UnityEngine;

using System;

namespace Shicho.Tool
{
    interface ITool : GUI.IConfigurableComponent<GUI.TabbedWindowConfig>
    {
        GUI.ConfigID ID { get; }
        GUI.TabbedWindowConfig ConfigProxy { get; set; }
        string[] Tabs { get; }

        void SetVisible(bool flag);
    }

    abstract class ToolBase
        : MonoBehaviour
        , ITool
    {
        public virtual void Awake()
        {
            win_ = UIView.GetAView().AddUIComponent(typeof(GUI.Panel)) as GUI.Panel;
            win_.Config = ConfigProxy;

            var scr = new Rect(0, 0, Screen.width, Screen.height);
            Config.Rect.xMin = 0;
            Config.Rect.xMax = Screen.width - Config.Rect.width;
            Config.Rect.yMin = 0;
            Config.Rect.yMax = Screen.height - Config.Rect.height;
        }

        public virtual void Start()
        {
            if (Config.TabIndex > Tabs.Length) {
                Config.TabIndex = 0;
            }

            Config.ID = new GUI.ConfigID() {
                Value = GetInstanceID(),
            };

            win_.position = Config.Rect.position;
            win_.size = Config.Rect.size;

            win_.eventClosed += () => {
                SetVisible(false);
            };
            win_.eventSizeChanged += (c, size) => {
                Config.Rect.size = size;
            };
            win_.eventPositionChanged += (c, pos) => {
                Config.Rect.position = pos;
            };
        }

        public virtual void OnDestroy()
        {
            Log.Debug("ffff222");
            ConfigProxy = Config.Clone() as GUI.TabbedWindowConfig;
            UnityEngine.Object.Destroy(win_);
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

        private GUI.Panel win_;
        protected GUI.Panel Window { get => win_; }
        public GUI.TabbedWindowConfig Config { get => win_.Config; set => win_.Config = value; }
        public abstract GUI.TabbedWindowConfig ConfigProxy { get; set; }

        public abstract string[] Tabs { get; }
    }
}

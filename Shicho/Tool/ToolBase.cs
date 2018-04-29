using ColossalFramework.UI;
using UnityEngine;

using System;

namespace Shicho.Tool
{
    interface ITool : GUI.IComponent<GUI.TabbedWindowConfig>
    {
        GUI.ConfigID ID { get; }
        GUI.TabbedWindowConfig ConfigProxy { get; set; }

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
        }

        public virtual void OnDestroy()
        {
            ConfigProxy = win_.Config;
            UnityEngine.Object.Destroy(win_);
        }

        public void SetVisible(bool flag)
        {
            if (flag) {
                Window.Show();

            } else {
                Window.Hide();
            }
        }

        public GUI.ConfigID ID { get => Config.ID; }

        private GUI.Panel win_;
        protected GUI.Panel Window { get => win_; }
        public GUI.TabbedWindowConfig Config { get => win_.Config; set => win_.Config = value; }
        public abstract GUI.TabbedWindowConfig ConfigProxy { get; set; }
    }
}

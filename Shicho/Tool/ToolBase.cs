using ColossalFramework.UI;
using UnityEngine;

using System;

namespace Shicho.Tool
{
    interface ITool<ConfigT>
        : GUI.IConfigurable<ConfigT>
        where ConfigT: GUI.IConfig, new()
    {
        GUI.ConfigID ID { get; }
        ConfigT ConfigProxy { get; set; }

        void SetVisible(bool flag);
    }

    abstract class ToolBase<ConfigT>
        : MonoBehaviour
        , GUI.IConfigurable<ConfigT>
        , ITool<ConfigT>
        where ConfigT: GUI.IConfig, new()
    {
        public virtual void Start()
        {
            win_.Config = ConfigProxy;
            win_ = UIView.GetAView().AddUIComponent(typeof(GUI.Panel<ConfigT>)) as GUI.Panel<ConfigT>;
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

        private GUI.Panel<ConfigT> win_;
        protected GUI.Panel<ConfigT> Window { get => win_; }
        public ConfigT Config { get => win_.Config; set => win_.Config = value; }
        public abstract ConfigT ConfigProxy { get; set; }
    }
}

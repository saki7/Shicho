using ColossalFramework.UI;
using UnityEngine;

using System;

namespace Shicho.GUI
{
    using Extension;

    struct TabTemplate
    {
        public IconSet icons;
        public string name;
        public Panel content;
    }

    public class Window
        : UIPanel
        , IConfigurableComponent<TabbedWindowConfig>
    {
        public override void Awake()
        {
            base.Awake();
            Config.ID = new ConfigID() {
                Value = GetInstanceID(),
            };

            isInteractive = true;
            color = new UnityEngine.Color32(20, 20, 40, 255);
            minimumSize = new Vector2(220, 120);
            maximumSize = new Vector2(Screen.width, Screen.height) / 4;
            autoSize = true;
            this.SetAutoLayout(LayoutDirection.Vertical);

            backgroundSprite = "MenuPanel";

            titleBar_ = AddUIComponent<TitleBar>();
            titleBar_.relativePosition = Vector3.zero;
            titleBar_.ControlType |= WindowControlType.Closable;
            titleBar_.Control.Close.tooltip = App.Config.mainKey.ToString();

            content_ = AddUIComponent<Panel>();
            content_.relativePosition = Vector2.zero;
            content_.minimumSize = new Vector2(400, 200);
            //OnSizeChanged();
        }

        public override void Start()
        {
            base.Start();
        }

        public new T AddUIComponent<T>()
            where T: UIComponent
        {
            return Helper.AddDefaultComponent<T>(this);
        }

        public event MouseEventHandler eventClosed {
            add => titleBar_.eventClosed += value;
            remove => titleBar_.eventClosed -= value;
        }

        protected override void OnSizeChanged()
        {
            titleBar_.width = width;
            //titleBar_.Control.width = width;

            content_.width = width;
            content_.height = height - titleBar_.height;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            //Destroy(titleBar_);
            //Destroy(content_);

            foreach (var child in GetComponentsInChildren<UIComponent>()) {
                Destroy(child);
            }
        }

        public TabbedWindowConfig Config { get; set; } = new TabbedWindowConfig();

        private TitleBar titleBar_;
        private Panel content_;
        public Panel Content { get => content_; }

        public string Title {
            get => titleBar_.Title;
            set => titleBar_.Title = value;
        }

        public Texture2D Icon {
            get => titleBar_.Icon;
            set => titleBar_.Icon = value;
        }
    }
}

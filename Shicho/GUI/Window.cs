using Shicho.Core;

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
            this.SetAutoLayout(LayoutDirection.Vertical);

            Config.ID = new ConfigID() {
                Value = GetInstanceID(),
            };

            titleBar_ = AddUIComponent<TitleBar>();
            //titleBar_.eventTitleChanged += (c, text) => {
            //    content_.relativePosition = new Vector2(0, c.height);
            //};

            content_ = AddUIComponent<Panel>();
        }

        public override void Start()
        {
            base.Start();

            isInteractive = true;
            color = new UnityEngine.Color32(20, 20, 40, 255);
            //minimumSize = new Vector2(220, 120);
            //maximumSize = new Vector2(Screen.width, Screen.height) / 4;
            backgroundSprite = "MenuPanel";

            titleBar_.width = width;
            titleBar_.ControlType |= WindowControlType.Closable;
            titleBar_.Control.Close.tooltip = App.Config.mainKey.ToString();

            content_.width = width;
            content_.height = height - titleBar_.height;
            //content_.minimumSize = new Vector2(400, 200);

            //Log.Debug($"Window: {position}, {size}");
            //Log.Debug($"titleBar: {titleBar_.position}, {titleBar_.size}");
            //Log.Debug($"content: {content_.position}, {content_.size}");
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

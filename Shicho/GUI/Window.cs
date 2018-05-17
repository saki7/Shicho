using Shicho.Core;

using ColossalFramework.UI;
using UnityEngine;

using System;

namespace Shicho.GUI
{
    using Extension;

    struct TabTemplate
    {
        public string name;
        public IconSet icons;
    }

    public class Window
        : UIPanel
        , IConfigurableComponent<TabbedWindowConfig>
    {
        public override void Awake()
        {
            base.Awake();
            titleBar_ = AddUIComponent<TitleBar>();

            clipChildren = true; // IMPORTANT

            // NB: initialization order dependency fix
            // VERY IMPORTANT!
            titleBar_.eventSizeChanged += (c, size) => OnSizeChanged();

            content_ = AddUIComponent<UIPanel>();

            eventSizeChanged += (c, size) => {
                titleBar_.width = size.x;

                content_.size = new Vector2(
                    size.x,
                    size.y - titleBar_.height
                );

                //Log.Debug($"Window: {position}, {size}");
                //Log.Debug($"titleBar: {titleBar_.position}, {titleBar_.size}");
                //Log.Debug($"content: {content_.position}, {content_.size}");
            };
        }

        public override void Start()
        {
            base.Start();

            // DON'T DO THIS
            //this.SetAutoLayout(LayoutDirection.Vertical);
            autoLayout = false;

            isInteractive = true;
            anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
            backgroundSprite = "MenuPanel2";

            //titleBar_.width = width;
            //titleBar_.height = 48;
            titleBar_.relativePosition = Vector2.zero;
            titleBar_.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
            titleBar_.ControlType |= WindowControlType.Closable;
            titleBar_.Control.Close.tooltip = App.Config.mainKey.ToString();
            //titleBar_.autoFitChildrenVertically = true;
            titleBar_.FitChildrenVertically();
            titleBar_.zOrder = 1;
            //Log.Debug($"titleBar: {titleBar_.position}, {titleBar_.size}");

            content_.relativePosition = new Vector2(0, titleBar_.height);
            content_.height = height - titleBar_.height;
            //content_.padding.top = 6;
            content_.zOrder = 0;
            content_.clipChildren = true;
        }

        public event MouseEventHandler eventClosed {
            add => titleBar_.eventClosed += value;
            remove => titleBar_.eventClosed -= value;
        }

        public TabbedWindowConfig Config { get; set; } = new TabbedWindowConfig();

        private TitleBar titleBar_;
        public float TitleBarHeight { get => titleBar_.height; }

        private UIPanel content_;
        public UIPanel Content { get => content_; }

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

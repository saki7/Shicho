using ColossalFramework.UI;
using UnityEngine;

using System;

namespace Shicho.GUI
{
    class Panel
        : UIPanel
        , IConfigurableComponent<TabbedWindowConfig>
    {
        public override void Awake()
        {
            base.Awake();
            Config.ID = new ConfigID() {
                Value = GetInstanceID(),
            };

            // autoLayout = true;
            isInteractive = true;
            color = new UnityEngine.Color32(20, 20, 40, 255);
            minimumSize = new Vector2(220, 120);
            maximumSize = new Vector2(Screen.width, Screen.height) / 4;

            backgroundSprite = "MenuPanel";

            drag_ = AddUIComponent<UIDragHandle>();
            drag_.height = 40;
            drag_.relativePosition = Vector3.zero;

            title_ = AddUIComponent<UILabel>();
            title_.text = "(Unnamed panel)";
            title_.isInteractive = false;
            title_.textAlignment = UIHorizontalAlignment.Center;
            title_.autoSize = false;
            title_.padding = new RectOffset(0, 0, 10, 10);
            title_.autoHeight = true;

            close_ = AddUIComponent<UIButton>();
            close_.normalBgSprite = "buttonclose";
            close_.pressedBgSprite = "buttonclosepressed";
            close_.hoveredBgSprite = "buttonclosehover";
            close_.tooltip = App.Config.mainKey.ToString();
            close_.eventClicked += (c, param) => {
                eventClosed?.Invoke();
            };

            OnSizeChanged();
        }

        public override void Start()
        {
            base.Start();
        }

        public delegate void OnClosedCallback();
        public event OnClosedCallback eventClosed;

        protected override void OnSizeChanged()
        {
            title_.width = width;
            drag_.width = width;
            drag_.height = height; // any drag

            close_.relativePosition = new Vector2(width - close_.width, 0);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            UnityEngine.Object.Destroy(title_);
            UnityEngine.Object.Destroy(drag_);
        }

        public TabbedWindowConfig Config { get; set; } = new TabbedWindowConfig();
        private UIDragHandle drag_;
        private UILabel title_;
        private UIButton close_;

        public string Title {
            get => title_.text;
            set => title_.text = value;
        }
    }
}

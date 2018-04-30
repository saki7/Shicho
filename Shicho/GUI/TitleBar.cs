using Shicho.Core;

using ColossalFramework.UI;
using UnityEngine;

using System;
using System.Linq;

namespace Shicho.GUI
{
    using Extension;

    [Flags]
    public enum WindowControlType
    {
        None        = 0b00000000,
        Closable    = 0b00000001,
    }

    public class WindowControl : UIPanel
    {
        public override void Awake()
        {
            base.Awake();
            this.SetAutoLayout(LayoutDirection.Horizontal);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (Close != null) Destroy(Close);
        }

        public WindowControlType Type {
            get => type_;

            set {
                type_ = value;

                if ((type_ & WindowControlType.Closable) != WindowControlType.None) {
                    Close = AddUIComponent<UIButton>();
                    IconSet.AssignRaw(
                        ref Close,
                        normal: "buttonclose",
                        pressed: "buttonclosepressed",
                        hovered: "buttonclosehover"
                    );
                }

                width = this.components.Where(c => c.GetType() == typeof(UIButton)).Sum(c => c.width);
                eventTypeChanged?.Invoke(this);
            }
        }

        public delegate void OnTypeChangedHandler(WindowControl c);
        public event OnTypeChangedHandler eventTypeChanged;

        private WindowControlType type_;
        public UIButton Close;
    }

    public class TitleBar : UIPanel
    {
        public override void Awake()
        {
            base.Awake();
            autoLayout = false;

            title_ = AddUIComponent<UILabel>();
            title_.text = "(Unnamed panel)";
            title_.padding = Helper.Padding(8, 8, 0, 8);

            title_.eventTextChanged += (c, text) => {
                OnSizeChanged();
            };

            control_ = AddUIComponent<WindowControl>();
            control_.eventTypeChanged += (c) => {
                c.relativePosition = new Vector2(width - c.width, 0);
                drag_.width = width - c.width;
            };

            drag_ = AddUIComponent<UIDragHandle>();
            drag_.relativePosition = Vector3.zero;

            eventSizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(UIComponent c, Vector2 size)
        {
            control_.relativePosition = new Vector2(width - control_.width, 0);

            drag_.width = size.x - control_.width;
            drag_.height = title_.height;
        }

        public override void Start()
        {
            base.Start();
            width = parent.width;
            drag_.target = parent;

            control_.Close.eventClicked += (c, param) => {
                eventClosed?.Invoke(c, param);
            };
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Destroy(title_);
            Destroy(control_);
            Destroy(drag_);
        }

        private UIDragHandle drag_;

        private WindowControl control_;
        public WindowControl Control { get => control_; }
        public WindowControlType ControlType {
            get => control_.Type;
            set => control_.Type = value;
        }

        public event MouseEventHandler eventClosed;

        private UILabel title_;
        public string Title {
            get => title_.text;
            set => title_.text = value;
        }
    }
}

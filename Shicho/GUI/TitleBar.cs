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

            clipChildren = true;

            icon_ = AddUIComponent<UISprite>();
            icon_.name = "InfoPanelIconInfo";

            title_ = AddUIComponent<UILabel>();
            title_.text = "(Unnamed panel)";
            title_.padding = Helper.Padding(10, 8, 2, 8);
            height = title_.height + title_.padding.bottom; // icon_.height

            control_ = AddUIComponent<WindowControl>();

            drag_ = AddUIComponent<UIDragHandle>();
            drag_.relativePosition = Vector2.zero;

            control_.eventTypeChanged += (c) => {
                c.relativePosition = new Vector2(width - c.width, 0);
                drag_.width = width - c.width;
            };

            title_.eventTextChanged += (c, text) => {
                eventTitleChanged?.Invoke(c, text);
            };

            eventSizeChanged += (c, size) => {
                control_.relativePosition = new Vector2(width - control_.width, 0);
                control_.height = size.y;

                drag_.width = size.x - control_.width;
                drag_.height = size.y;

                // Log.Debug($"esc: {size}, control_: {control_.size}, drag_: {drag_.size}");
            };
        }

        public override void Start()
        {
            base.Start();

            //autoSize = false;

            icon_.relativePosition = new Vector2(4, 3);
            icon_.width = icon_.height = 24;
            icon_.SendToBack();

            //title_.padding.left += (int)icon_.width + 1;
            title_.relativePosition = new Vector2(icon_.width + 1, 0);
            title_.font = FontStore.Get(15);

            color = Helper.RGB(20, 20, 40);
            backgroundSprite = "GenericTabDisabled";

            //anchor = UIAnchorStyle.All;
            size = new Vector2(
                parent.width,
                title_.height + title_.padding.bottom // icon_.height
            );

            zOrder = 0;

            drag_.target = parent;

            if (control_.Close) {
                control_.Close.eventClicked += (c, param) => {
                    eventClosed?.Invoke(c, param);
                };
            }
        }

        private UIDragHandle drag_;

        private WindowControl control_;
        public WindowControl Control { get => control_; }
        public WindowControlType ControlType {
            get => control_.Type;
            set => control_.Type = value;
        }

        public event PropertyChangedEventHandler<string> eventTitleChanged;
        public event MouseEventHandler eventClosed;

        private UIComponent icon_;
        private Texture2D iconData_;

        public Texture2D Icon {
            get => iconData_;

            set {
                iconData_ = value;
                if (icon_ != null) {
                    RemoveUIComponent(icon_);
                    Destroy(icon_);
                }

                var sprite = AddUIComponent<UITextureSprite>();
                sprite.texture = iconData_;
                icon_ = sprite;
            }
        }

        private UILabel title_;
        public string Title {
            get => title_.text;
            set => title_.text = value;
        }
    }
}

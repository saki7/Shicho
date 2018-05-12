using ColossalFramework.UI;
using UnityEngine;
using System;

namespace Shicho.GUI
{
    public static class Helper
    {
        public static RectOffset ZeroOffset {
            get => new RectOffset(0, 0, 0, 0);
        }

        public static Vector2 ScreenResolution {
            get => UIView.GetAView().GetScreenResolution();
        }

        public static Rect ScreenRectAsScreen {
            get {
                //Log.Debug($"{v.GetScreenResolution()}, {v.scale}, {v.ratio}, {v.inputScale}");

                var res = ScreenResolution;
                return new Rect(
                    x: 0, y: 0, width: res.x, height: res.y
                );
            }
        }

        public static Rect ScreenRectAsUI {
            get => ScreenToUI(ScreenRectAsScreen);
        }

        // UI coordinates = same axis as Screen, center point at "center"
        public static Rect ScreenToUI(Rect r)
        {
            var res = ScreenResolution;
            return new Rect(
                x: r.x - res.x / 2,
                y: r.y - res.y / 2,
                width: r.width,
                height: r.height
            );
        }

        // CSS-like padding specifiers
        public static RectOffset Padding(int top, int? right = null, int? bottom = null, int? left = null)
        {
            if (!right.HasValue) {
                return new RectOffset(top: top, bottom: top, left: 0, right: 0);
            }
            if (!bottom.HasValue) {
                return new RectOffset(top: top, bottom: top, left: right.Value, right: right.Value);
            }
            if (!left.HasValue) {
                return new RectOffset(top: top, right: right.Value, bottom: bottom.Value, left: 0);
            }
            return new RectOffset(top: top, right: right.Value, bottom: bottom.Value, left: left.Value);
        }

        public static T AddDefaultComponent<T>(object parentObj, Vector2? relPos = null)
            where T: UIComponent
        {
            if (!(parentObj is UIComponent)) {
                throw new ArgumentException($"parentObj ({parentObj.GetType()}) must be UIComponent");
            }

            var parent = parentObj as UIComponent;
            var c = parent.AddUIComponent<T>();
            c.relativePosition = relPos ?? Vector2.zero;
            return c;
        }

        public static UILabel AddLabel(ref UIPanel parent, string label, string tooltip = "", UIFont font = null, RectOffset padding = null)
        {
            var obj = parent.AddUIComponent<UILabel>();
            obj.text = label;
            obj.tooltip = tooltip;
            if (font != null) {
                obj.font = font;
            }
            if (padding != null) {
                obj.padding = padding;
            }
            return obj;
        }

        public static UICheckBox AddCheckBox(ref UIPanel parent, string label, string tooltip = "", UIFont font = null)
        {
            var box = parent.AddUIComponent<UICheckBox>();
            box.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;

            box.label = box.AddUIComponent<UILabel>();
            box.label.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;

            box.label.text = label;
            box.label.tooltip = tooltip;

            if (font != null) {
                box.label.font = font;
            }
            box.label.padding.left = box.label.font.size + 6;
            box.label.relativePosition = new Vector2(0, -(box.label.font.size - 10));

            var uncheckedSprite = box.AddUIComponent<UISprite>() as UISprite;
            uncheckedSprite.spriteName = "AchievementCheckedFalse";
            uncheckedSprite.relativePosition = new Vector2(0, 2);
            uncheckedSprite.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left;
            uncheckedSprite.width = uncheckedSprite.height = box.label.font.size;

            var checkedSprite = uncheckedSprite.AddUIComponent<UISprite>() as UISprite;
            checkedSprite.spriteName = "AchievementCheckedTrue";
            checkedSprite.relativePosition = Vector2.zero;
            checkedSprite.anchor = uncheckedSprite.anchor;
            checkedSprite.size = uncheckedSprite.size;

            box.checkedBoxObject = checkedSprite;
            return box;
        }


        public static void DeepDestroy(UIComponent component)
        {
            if (component == null) return;
            var children = component.GetComponentsInChildren<UIComponent>();

            if (children != null && children.Length > 0) {
                for (int i = 0; i < children.Length; ++i) {
                    if (children[i].parent == component) {
                        DeepDestroy(children[i]);
                    }
                }
            }
            UnityEngine.Object.Destroy(component);
            component = null;
        }
    }
}

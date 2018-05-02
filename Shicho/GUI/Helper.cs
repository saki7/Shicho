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

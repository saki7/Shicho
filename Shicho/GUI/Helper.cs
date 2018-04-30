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
    }
}

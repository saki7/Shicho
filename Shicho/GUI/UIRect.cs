using Shicho.Core;

using ColossalFramework.UI;
using UnityEngine;

using System;
using System.Xml.Serialization;


namespace Shicho.GUI
{
    [Serializable]
    public struct UIRect
    {
        // NB: this class is based on UI (Colossal) coordinates, not GUI (Unity) coordinates

        [NonSerialized]
        [XmlIgnore]
        public Vector2 position, size;

        public float x { get => position.x; set => position.x = value; }
        public float y { get => position.y; set => position.y = value; }

        public float width { get => size.x; set => size.x = value; }
        public float height { get => size.y; set => size.y = value; }

        public void RelocateIn(Rect outer)
        {
            //Log.Debug($"{outer}, x [{outer.xMin}, {outer.xMax}], y [{outer.yMin}, {outer.yMax}]");

            x = Mathf.Clamp(x, outer.xMin, outer.xMax);
            y = Mathf.Clamp(y, outer.yMin, outer.yMax);
        }

        public static implicit operator UIRect(Rect rect)
        {
            return new UIRect() {
                x = rect.x,
                y = rect.y,
                width = rect.width,
                height = rect.height,
            };
        }

        public static explicit operator Rect(UIRect rect)
        {
            return new UnityEngine.Rect(
                x: rect.x, y: rect.y, width: rect.width, height: rect.height
            );
        }

        public override string ToString()
        {
            return $"(x: {x}, y: {y}, width: {width}, height: {height})";
        }
    }
}

using UnityEngine;

using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Shicho.GUI
{
    [Serializable]
    public struct ConfigID
    {
        public Int64 Value;
    }

    interface IConfig : ICloneable
    {
        ConfigID ID { get; set; }
    }

    [Serializable]
    public struct SimpleRect
    {
        [NonSerialized]
        [XmlIgnore]
        public Vector2 position, size;

        public float x { get => position.x; set => position.x = value; }
        public float y { get => position.y; set => position.y = value; }

        public float width  { get => size.x; set => size.x = value; }
        public float height { get => size.y; set => size.y = value; }

        public void RelocateIn(UnityEngine.Rect outer)
        {
            x = Mathf.Clamp(x, outer.x, outer.xMax - width);
            y = Mathf.Clamp(x, outer.y, outer.yMax - height);
        }

        public static implicit operator SimpleRect(UnityEngine.Rect rect)
        {
            return new SimpleRect() {
                x = rect.x,
                y = rect.y,
                width = rect.width,
                height = rect.height,
            };
        }

        public static explicit operator UnityEngine.Rect(SimpleRect rect)
        {
            return new UnityEngine.Rect(
                x: rect.x, y: rect.y, width: rect.width, height: rect.height
            );
        }
    }

    [Serializable]
    public class WindowConfig : IConfig
    {
        [NonSerialized]
        [XmlIgnore]
        private ConfigID id_;

        [XmlIgnore]
        public ConfigID ID { get => id_; set => id_ = value; }

        [SerializeField]
        public bool IsVisible = false;

        [SerializeField]
        public SimpleRect Rect;

        [SerializeField]
        [XmlArray]
        IConfig[] Children;

        public object Clone() => MemberwiseClone();
    }

    [Serializable]
    public class TabbedWindowConfig : WindowConfig
    {
        public int TabIndex = 0;
    }
}

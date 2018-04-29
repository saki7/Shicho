using UnityEngine;

using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


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
    struct SimpleRect
    {
        public float x, y, width, height;

        public static explicit operator SimpleRect(UnityEngine.Rect rect)
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
        private ConfigID id_;
        public ConfigID ID { get => id_; set => id_ = value; }

        public bool IsVisible = false;

        [NonSerialized]
        public Rect Rect;
        private SimpleRect? RectS;

        IConfig[] Children;


        //[OnDeserializing]
        //private void OnDeserializingMethod(StreamingContext ctx)
        //{
        //}

        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext ctx)
        {
            if (RectS.HasValue) {
                Rect = (UnityEngine.Rect)RectS;
                RectS = null;
            }
        }

        [OnSerializing]
        private void OnSerializingMethod(StreamingContext ctx)
        {
            RectS = (SimpleRect)Rect;
        }

        [OnSerialized]
        private void OnSerializedMethod(StreamingContext ctx)
        {
            RectS = null;
        }

        public object Clone() => MemberwiseClone();
    }

    [Serializable]
    public class TabbedWindowConfig : WindowConfig
    {
        public int TabIndex = 0;
    }
}

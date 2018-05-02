using Shicho.Core;

using ColossalFramework.UI;

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
        public UIRect Rect;

        //[SerializeField]
        //[XmlArray]
        //IConfig[] Children;

        public object Clone() => MemberwiseClone();
    }

    [Serializable]
    public class TabbedWindowConfig : WindowConfig
    {
        public int SelectedTabIndex = 0;
    }
}

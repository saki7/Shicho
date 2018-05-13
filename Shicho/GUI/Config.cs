using Shicho.Core;

using ColossalFramework.UI;

using UnityEngine;

using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Shicho.GUI
{
    interface IConfig : ICloneable
    {
    }

    [Serializable]
    public class WindowConfig : IConfig
    {
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

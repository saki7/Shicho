using ColossalFramework.UI;
using UnityEngine;

using System;

namespace Shicho.GUI
{
    interface IComponent<ConfigT>
        where ConfigT : IConfig
    {
        ConfigT Config { get; set; }
    }
}

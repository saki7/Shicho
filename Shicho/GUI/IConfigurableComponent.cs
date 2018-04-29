using ColossalFramework.UI;
using UnityEngine;

using System;

namespace Shicho.GUI
{
    interface IConfigurableComponent<ConfigT>
        where ConfigT : IConfig
    {
        ConfigT Config { get; set; }
    }
}

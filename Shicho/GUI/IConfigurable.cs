using ColossalFramework.UI;
using UnityEngine;

using System;

namespace Shicho.GUI
{
    interface IConfigurable<ConfigT>
    {
        ConfigT Config { get; set; }
    }
}

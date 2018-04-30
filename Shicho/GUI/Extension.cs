using ColossalFramework.UI;
using UnityEngine;

using System;

namespace Shicho.GUI.Extension
{
    public static class PanelExtension
    {
        public static void SetAutoLayout(
            this UIPanel c,
            LayoutDirection direction,
            RectOffset padding = null,
            LayoutStart start = LayoutStart.TopLeft
        ) {
            c.autoLayout = true;
            c.autoLayoutDirection = direction;
            c.autoLayoutStart = start;
            c.autoLayoutPadding = padding ?? Helper.ZeroOffset;
        }
    }
}

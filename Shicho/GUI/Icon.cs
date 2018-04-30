using ColossalFramework.UI;
using System;

namespace Shicho.GUI
{
    public struct IconSet
    {
        public string Normal, Hovered, Focused, Pressed;

        public static void AssignRaw(ref UIButton btn, string normal, string hovered, string pressed, string focused = null)
        {
            btn.normalBgSprite = normal;
            btn.hoveredBgSprite = hovered;
            btn.focusedBgSprite = focused;
            btn.pressedBgSprite = pressed;
        }


        public void AssignTo(ref UIInteractiveComponent c)
        {
            c.normalBgSprite = Normal;
            c.hoveredBgSprite = Hovered;
            c.focusedBgSprite = Focused;
        }

        public void AssignTo(ref UIButton btn)
        {
            var obj = btn as UIInteractiveComponent;
            AssignTo(ref obj);
            btn.pressedBgSprite = Pressed;
        }
    }
}

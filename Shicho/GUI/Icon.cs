using ColossalFramework.UI;
using System;

namespace Shicho.GUI
{
    public struct IconSet
    {
        public string Normal, Hovered, Focused, Pressed;

        public override string ToString()
        {
            return $"Normal: {Normal}, Hovered: {Hovered}, Focused: {Focused}, Pressed: {Pressed}";
        }

        public static void AssignRaw(ref UIButton btn, string normal, string hovered, string pressed, string focused = null)
        {
            btn.normalFgSprite = normal;
            btn.hoveredFgSprite = hovered;
            btn.focusedFgSprite = focused;
            btn.pressedFgSprite = pressed;
        }


        public void AssignTo(ref UIInteractiveComponent c)
        {
            c.normalFgSprite = Normal;
            c.hoveredFgSprite = Hovered;
            c.focusedFgSprite = Focused;
        }

        public void AssignTo(ref UIButton btn)
        {
            var obj = btn as UIInteractiveComponent;
            AssignTo(ref obj);
            btn.pressedFgSprite = Pressed;
        }
    }
}

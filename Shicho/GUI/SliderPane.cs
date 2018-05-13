using ColossalFramework.UI;
using System;

namespace Shicho.GUI
{
    public class SliderOption<T>
    {
        public bool hasField = true;
        public bool isEnabled;

        public float /* fixed type */ minValue, maxValue, stepSize;
        public Mod.Config.Switchable<T>.SlideHandler eventValueChanged;
        public Mod.Config.Switchable<T>.SwitchHandler eventSwitched;
    }

    public class SliderPane
    {
        public UIPanel wrapper;
        public UISlider slider;
        public UITextField field;
    }
}

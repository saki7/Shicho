using ColossalFramework.UI;
using System;

namespace Shicho.GUI
{
    public class SliderOption<T>
    {
        public SliderOption(
            float minValue, float maxValue, float stepSize,
            float defaultValue,
            Mod.Config.Switchable<T>.SlideHandler onValueChanged
        ) {
            this.onValueChanged = onValueChanged;
        }

        public bool hasField = true;
        public bool isEnabled;

        public float /* fixed type */ defaultValue, minValue, maxValue, stepSize;
        public Mod.Config.Switchable<T>.SlideHandler onValueChanged;
        public Mod.Config.Switchable<T>.SwitchHandler onSwitched;
    }

    public class SliderPane
    {
        public UIPanel wrapper;
        public UISlider slider;
        public UITextField field;
    }
}

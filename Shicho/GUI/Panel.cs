using ColossalFramework.UI;

using System;

namespace Shicho.GUI
{
    class Panel : UIPanel
    {
        public override void Awake()
        {
            base.Awake();
            Config.ID = new ConfigID() {
                Value = (Int64)GetInstanceID(),
            };
        }

        public override void Start()
        {
            base.Start();
            autoLayout = true;

            {
                var label = AddUIComponent<UILabel>();
                label.text = "EEEEeeee1112!!!!@@";
            }
            this.Show();
        }

        public TabbedWindowConfig Config { get; set; } = new TabbedWindowConfig();
    }
}

using ColossalFramework.UI;

using System;

namespace Shicho.GUI
{
    class Panel<ConfigT>
        : UIPanel
        , IConfigurable<ConfigT>
        where ConfigT: IConfig, new()
    {
        public override void Awake()
        {
            base.Awake();
            Config.ID = new ConfigID() {
                Value = (UInt64)GetInstanceID(),
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

        public ConfigT Config { get; set; } = new ConfigT();
    }
}

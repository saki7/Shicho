extern alias Cities;

using Shicho.Core;
using ICities;

namespace Shicho.Mod
{
    public class Mod : IUserMod
    {
        public string Name { get => ModInfo.ID; }
        public string Description { get => ModInfo.Description; }

        public void OnEnabled()
        {
            Bootstrapper.Instance.Bootstrap();
            App.Instance.LoadConfig();

            if (Cities.ToolsModifierControl.toolController != null)  {
                if (Cities.ToolsModifierControl.toolController.m_mode == Cities.ItemClass.Availability.Game) {
                    App.Instance.LoadLevelData();
                }
            }
        }

        public void OnDisabled()
        {
            try {
                App.Instance.SaveConfig();

            } finally {
                Bootstrapper.Instance.Cleanup();
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
            => App.Instance.OnSettingsUI(helper);
    }
}

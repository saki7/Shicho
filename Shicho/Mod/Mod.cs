extern alias Cities;

using Shicho.Core;
using ICities;

using UnityEngine;

namespace Shicho.Mod
{
    public class Mod
        : IUserMod
        , ILoadingExtension
    {
        public string Name { get => ModInfo.ID; }
        public string Description { get => ModInfo.Description; }

        public void OnEnabled()
        {
            Bootstrapper.Instance.Bootstrap();

            if (Cities.ToolsModifierControl.toolController != null)  {
                if (Cities.ToolsModifierControl.toolController.m_mode == Cities.ItemClass.Availability.Game) {
                    App.Instance.InitGameMode();
                }
            }
        }

        public void OnDisabled()
        {
            try {
                Bootstrapper.Instance.Bootstrap();
                App.Instance.SaveConfig();

            } finally {
                Bootstrapper.Instance.Cleanup();
            }
        }

        public void OnCreated(ILoading loading)
        {
        }

        public void OnReleased()
        {
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            Bootstrapper.Instance.Bootstrap();
            App.Instance.InitGameMode();
        }

        public void OnLevelUnloading()
        {
            App.Instance.UnloadLevelData();
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            Bootstrapper.Instance.BootstrapConfig();
            App.Instance.OnSettingsUI(helper);
        }
    }
}

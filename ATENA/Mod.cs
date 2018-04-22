using ICities;
using UnityEngine;

using Harmony;

using System;
using System.Reflection;


namespace ATENA
{
    public class Mod : IUserMod
    {
        public string Name { get => ModInfo.ID; }
        public string Description { get => ModInfo.Description; }

        public void OnEnabled()
        {
            //harmony.Patch(
            //    typeof(VehicleManager).GetMethod("TheMethod"),
            //    new HarmonyMethod(typeof(ATENA.Patcher.VehicleManagerPatch.CreateVehicle).GetMethod("Prefix")),
            //    null
            //);

            Bootstrapper.Instance.Bootstrap();
            //Log.Info("OnEnabled");
        }

        public void OnDisabled()
        {
            try {
                //Log.Info("OnDisabled");
                Manager.ConfigManager.Instance.Save();

            } finally {
                Bootstrapper.Instance.Cleanup();
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            Bootstrapper.Instance.Atena.OnSettingsUI(helper);
        }
    }
}

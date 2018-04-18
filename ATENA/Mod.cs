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
            var harmony = HarmonyInstance.Create("com.github.setnahq.atena");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            //harmony.Patch(
            //    typeof(VehicleManager).GetMethod("TheMethod"),
            //    new HarmonyMethod(typeof(ATENA.Patcher.VehicleManagerPatch.CreateVehicle).GetMethod("Prefix")),
            //    null
            //);

            Bootstrapper.Instance.Bootstrap();
            //Log.Info("OnEnabled");
            cfg = GameObject.Find(ModInfo.ID).AddComponent<AtenaConfig>();
        }

        public void OnDisabled()
        {
            try {
                //Log.Info("OnDisabled");

                Manager.ConfigManager.Instance.Save();
            } finally {
                cfg = null;
                Bootstrapper.Instance.Cleanup();
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            cfg.Populate(helper);
        }

        private AtenaConfig cfg;
    }
}

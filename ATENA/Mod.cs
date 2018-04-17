using ICities;
using UnityEngine;

using Harmony;

using System;
using System.Reflection;


namespace ATENA
{
    public class Mod : IUserMod
    {
        public string Name
        {
            get {
                // Dirty hack, as in MT
                Bootstrapper.Bootstrap();
                return ModInfo.ID;
            }
        }
        public string Description => ModInfo.Description;

        public void OnEnabled()
        {
            var harmony = HarmonyInstance.Create("com.github.setnahq.atena");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            //harmony.Patch(
            //    typeof(VehicleManager).GetMethod("TheMethod"),
            //    new HarmonyMethod(typeof(ATENA.Patcher.VehicleManagerPatch.CreateVehicle).GetMethod("Prefix")),
            //    null
            //);

            Bootstrapper.Bootstrap();
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            if (cfg == null) {
                cfg = new GameObject($"{ModInfo.ID}.Config").AddComponent<AtenaConfig>();
            }

            Manager.ConfigManager.Instance.Load();
            cfg.Populate(helper);
        }

        private AtenaConfig cfg;
    }
}

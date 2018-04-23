extern alias CitiesL;
using ICities;
using ColossalFramework;
using UnityEngine;

namespace ATENA
{
    class AtenaConfig : MonoBehaviour
    {
        public void Populate(UIHelperBase helper)
        {
            //var mgr = Manager.ConfigManager.Instance;
            helper.AddButton("Force update", () => {
                Log.Warn("force updating!");
            });

            var bmgr = Singleton<CitiesL.BuildingManager>.instance;



            helper.AddButton("Spawn car", () => {
                var mgr = Singleton<CitiesL.VehicleManager>.instance;

                for (var i = 0; i < 3; ++i) {
                }
            });
        }

        private ColossalFramework.Math.Randomizer r = Singleton<CitiesL.SimulationManager>.instance.m_randomizer;
    }
}

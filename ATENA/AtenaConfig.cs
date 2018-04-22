using ICities;
using ColossalFramework;
using UnityEngine;

using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;

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

            var bmgr = Singleton<BuildingManager>.instance;



            helper.AddButton("Spawn car", () => {
                var mgr = Singleton<VehicleManager>.instance;

                for (var i = 0; i < 3; ++i) {
                }
            });
        }

        private ColossalFramework.Math.Randomizer r = Singleton<SimulationManager>.instance.m_randomizer;
    }
}

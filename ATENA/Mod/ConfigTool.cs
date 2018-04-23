extern alias CitiesL;

using ATENA.Core;

using ICities;
using UnityEngine;

namespace ATENA.Mod
{
    class ConfigTool : MonoBehaviour
    {
        public void Populate(UIHelperBase helper)
        {
            helper.AddButton("Reset", Atena.Instance.Reset);
            helper.AddButton("Print Stats", Atena.Instance.PrintStats);
            helper.AddButton("Set flow", SetFlow);
        }

        private void SetFlow()
        {
            Atena.Instance.SetFlow(CitiesL.ItemClass.Service.Citizen, 200);
        }
    }
}

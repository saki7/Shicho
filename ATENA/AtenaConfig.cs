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
            helper.AddButton("Reset", Atena.Instance.Reset);
            helper.AddButton("Set flow", SetFlow);
        }

        private void SetFlow()
        {
            Atena.Instance.SetFlow(CitiesL.ItemClass.Service.Citizen, 200);
        }
    }
}

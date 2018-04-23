extern alias CitiesL;

using ICities;
using UnityEngine;

namespace ATENA.Mod
{
    class ConfigTool : MonoBehaviour
    {
        public void Populate(UIHelperBase helper)
        {
            helper.AddButton("Reset", Atena.Instance.Reset);
            helper.AddButton("Test1", Test1);
            helper.AddButton("Set flow", SetFlow);
        }

        private void Test1()
        {
            var r = new Game.Road();
        }

        private void SetFlow()
        {
            Atena.Instance.SetFlow(CitiesL.ItemClass.Service.Citizen, 200);
        }
    }
}

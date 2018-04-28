extern alias Cities;

using Shicho.Core;

using ICities;
using UnityEngine;

namespace Shicho.Mod
{
    class ConfigTool : MonoBehaviour
    {
        public void Populate(UIHelperBase helper)
        {
            helper.AddButton("Reset", App.Instance.Reset);
            helper.AddButton("Print Stats", App.Instance.PrintStats);
            helper.AddButton("Set flow", SetFlow);
        }

        private void SetFlow()
        {
            App.Instance.SetFlow(Cities::ItemClass.Service.Citizen, 200);
        }
    }
}

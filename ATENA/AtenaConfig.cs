using ICities;
using UnityEngine;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

namespace ATENA
{
    class AtenaConfig : MonoBehaviour
    {
        public void Populate(UIHelperBase helper)
        {
            var mgr = Manager.ConfigManager.Instance;
            helper.AddButton("Force update", () => {
                Log.Warn("force updating!");
            });
        }
    }
}

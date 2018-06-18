extern alias Cities;

using Shicho.Core;

using ICities;
using UnityEngine;

using System;
using System.Linq;


namespace Shicho.Tool
{
    class ConfigTool : MonoBehaviour
    {
        public void Populate(UIHelperBase gui)
        {
            {
                var grp = gui.AddGroup("Main key binding");
                var keyModChoices = SInput.ModMap.Reverse().ToArray();

                grp.AddDropdown(
                    "Mod",
                    keyModChoices.Select(kv => kv.Value).ToArray(),
                    keyModChoices
                        .Select((kv, i) => new {Index = i, KV = kv})
                        .Where(t => t.KV.Key == App.Config.mainKey.Mod)
                        .Select(res => res.Index)
                        .First(),
                    (i) => {
                        App.Config.ChangeKeyBinding(keyModChoices[i].Key);
                    }
                );

                grp.AddDropdown(
                    "Key",
                    Enumerable.Range((int)'A', (int)'Z' - (int)'A' + 1)
                        .Select(i => ((char)i).ToString())
                        .ToArray(),
                    (int)App.Config.mainKey.Code - (int)KeyCode.A,
                    (i) => {
                        App.Config.ChangeKeyBinding(null, (KeyCode)((int)KeyCode.A) + i);
                    }
                );
            }
        }
    }
}

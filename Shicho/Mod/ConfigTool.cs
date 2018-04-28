extern alias Cities;

using Shicho.Core;

using ICities;
using UnityEngine;

using System.Linq;


namespace Shicho.Mod
{
    class ConfigTool : MonoBehaviour
    {
        public void Populate(UIHelperBase helper, Config cfg)
        {
            {
                var grp = helper.AddGroup("Main key binding");
                var keyModChoices = Config.ModMap.Reverse().ToArray();

                grp.AddDropdown(
                    "Mod",
                    keyModChoices.Select(kv => kv.Value).ToArray(),
                    keyModChoices
                        .Select((s, i) => new {i, s})
                        .Where(t => t.s.Key == cfg.boundKeyMod)
                        .Select((m, i) => i)
                        .First(),
                    (i) => {
                        App.Instance.ChangeKeyBinding(keyModChoices[i].Key);
                    }
                );

                grp.AddDropdown(
                    "Key",
                    Enumerable.Range((int)'A', (int)'Z')
                        .Select(i => ('A' + i).ToString())
                        .ToArray(),
                    (int)cfg.boundKey - (int)KeyCode.A,
                    (i) => {
                        App.Instance.ChangeKeyBinding(null, (KeyCode)((int)KeyCode.A) + i);
                    }
                );
            }
        }

        private void SetFlow()
        {
            App.Instance.SetFlow(Cities::ItemClass.Service.Citizen, 200);
        }
    }
}

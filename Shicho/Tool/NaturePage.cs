using Shicho.Core;
using Shicho.GUI;

using ColossalFramework.UI;
using UnityEngine;
using System;


namespace Shicho.Tool
{
    using Shicho.GUI.Extension;

    public static class NaturePage
    {
        public static void Setup(ref UIPanel page)
        {
            page.padding = Helper.Padding(8, 12);

            var pane = ToolHelper.AddConfig(
                ref page,
                "Tree movement",
                "Sets the amount of tree branch movement.\nVanilla: maximum; Recommended: minimum",
                opts: new SliderOption<float>() {
                    hasField = false,

                    minValue = 0.0f,
                    maxValue = 1.0f,
                    stepSize = 0.1f,

                    eventValueChanged = (c, value) => {
                        ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.treeMoveFactor, value);
                    },
                },
                labelPadding: 4,
                bullet: "InfoIconMaintenance"
            );
            pane.slider.value = App.Config.Graphics.treeMoveFactor;

            {
                var cb = Helper.AddCheckBox(ref page, "Force stop distant trees (*)", "* restart required", font: FontStore.Get(11));
                lock (App.Config.GraphicsLock) {
                    cb.isChecked = App.Config.Graphics.stopDistantTrees;
                }
                cb.eventCheckChanged += (c, isChecked) => {
                    lock (App.Config.GraphicsLock) {
                        App.Config.Graphics.stopDistantTrees = isChecked;
                    }
                };
            }

            {
                var cb = Helper.AddCheckBox(ref page, "Random tree rotation", font: FontStore.Get(11));
                lock (App.Config.GraphicsLock) {
                    cb.isChecked = App.Config.Graphics.randomTrees;
                }
                cb.eventCheckChanged += (c, isChecked) => {
                    lock (App.Config.GraphicsLock) {
                        App.Config.Graphics.randomTrees = isChecked;
                    }
                };
            }
        }
    }
}

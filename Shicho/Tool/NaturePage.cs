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
                opts: new SliderOption<float>(
                    minValue: 0.0f,
                    maxValue: 1.0f,
                    stepSize: 0.1f,
                    defaultValue: App.Config.Graphics.treeMoveFactor,

                    (c, value) => {
                        ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.treeMoveFactor, value);
                    }
                ) {
                    hasField = false,

                },
                labelPadding: 4,
                bullet: "InfoIconMaintenance"
            );

            Helper.AddCheckBox(
                ref page,
                "Force stop distant trees (*)",
                "* restart required",
                defaultValue: App.Config.Graphics.stopDistantTrees,
                (c, isChecked) => {
                    lock (App.Config.GraphicsLock) {
                        App.Config.Graphics.stopDistantTrees = isChecked;
                    }
                },
                font: FontStore.Get(11)
            );

            Helper.AddCheckBox(
                ref page,
                "Random tree rotation",
                "Close range only (technical limitation for LOD)",
                defaultValue: App.Config.Graphics.randomTrees,
                (c, isChecked) => {
                    lock (App.Config.GraphicsLock) {
                        App.Config.Graphics.randomTrees = isChecked;
                    }
                },
                font: FontStore.Get(11)
            );
        }
    }
}

using Shicho.Core;
using Shicho.GUI;

using ColossalFramework.UI;
using UnityEngine;
using System;


namespace Shicho.Tool
{
    using Shicho.GUI.Extension;

    public static class CitizenPage
    {
        public static void Setup(ref UIPanel page)
        {
            page.padding = Helper.Padding(6, 2);
            page.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 8, 0));
            page.autoFitChildrenVertically = true;

            // info panel
            var citizenInfo = page.AddUIComponent<CitizenInfo>();
            citizenInfo.padding = Helper.Padding(0, 10, 4, 10);

            var panel = page.AddUIComponent<UIPanel>();
            panel.width = panel.parent.width - page.padding.horizontal - 10;
            panel.SetAutoLayout(LayoutDirection.Vertical);

            var pane = ToolHelper.AddConfig(
                ref panel,
                "Health literacy",
                "Let citizens stay at their home in hope of recovery, instead of calling 911.\n Requires decent healthcare system in city.",
                opts: new SliderOption<float>(
                    minValue: 0.2f,
                    maxValue: 0.95f,
                    stepSize: 0.05f,
                    defaultValue: App.Config.AI.regenChance.Value,

                    (c, value) => {
                        ToolHelper.LockedApply(App.Config.AILock, ref App.Config.AI.regenChance, value);
                    }
                ) {
                    hasField = false,
                    isEnabled = App.Config.AI.regenChance.Enabled,
                    onSwitched = App.Config.AI.regenChance.LockedSwitch(App.Config.AILock),
                },
                labelPadding: 4,
                indentPadding: 12
            );

            var tipPanel = panel.AddUIComponent<UIPanel>();
            tipPanel.SetAutoLayout(LayoutDirection.Horizontal);
            tipPanel.relativePosition = Vector2.zero;
            tipPanel.padding = Helper.Padding(0, 0, 0, 8);
            tipPanel.width = tipPanel.parent.width - (tipPanel.padding.horizontal + panel.padding.horizontal);

            Helper.AddIconLabel(
                ref tipPanel,
                "ambulance",
                "Call 911",
                wrapperWidth: tipPanel.width / 2,
                font: FontStore.Get(10),
                color: Helper.RGB(160, 160, 160)
            );
            Helper.AddIconLabel(
                ref tipPanel,
                "housing",
                "Stay in bed",
                wrapperWidth: tipPanel.width / 2,
                font: FontStore.Get(10),
                color: Helper.RGB(160, 160, 160),
                isInverted: true
            );
        }
    }
}

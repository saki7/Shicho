using Shicho.Core;
using Shicho.GUI;

using ColossalFramework.UI;
using UnityEngine;
using System;


namespace Shicho.Tool
{
    using Shicho.GUI.Extension;

    public static class GraphicsPage
    {
        public static void Setup(ref UIPanel page)
        {
            page.padding = Helper.Padding(6, 2);
            page.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 8, 0));
            page.autoFitChildrenVertically = true;

            {
                var pane = page.AddUIComponent<UIPanel>();
                pane.width = pane.parent.width - page.padding.horizontal;
                pane.padding = Helper.Padding(4, 12, 4, 0);
                pane.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 2, 0));
                pane.autoFitChildrenVertically = true;

                Helper.AddLabel(
                    ref pane,
                    "Light & Shadow",
                    font: FontStore.Get(12),
                    color: Helper.RGB(220, 230, 250),
                    bullet: "LineDetailButton"
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Light intensity",
                    "default: ≈4.2",
                    opts: new SliderOption<float>(
                        minValue: 0.05f,
                        maxValue: 8.0f,
                        stepSize: 0.05f,
                        defaultValue: App.Config.Graphics.lightIntensity.Value,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.lightIntensity, value);
                        }
                    ) {
                        isEnabled = App.Config.Graphics.lightIntensity.Enabled,
                        onSwitched = App.Config.Graphics.lightIntensity.LockedSwitch(App.Config.GraphicsLock),
                    },
                    indentPadding: 12
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Shadow strength",
                    "default: 0.8",
                    opts: new SliderOption<float>(
                        minValue: 0.1f,
                        maxValue: 1.0f,
                        stepSize: 0.05f,
                        defaultValue: App.Config.Graphics.shadowStrength.Value,

                        App.Config.Graphics.shadowStrength.LockedSlide(App.Config.GraphicsLock)
                    ) {
                        isEnabled = App.Config.Graphics.shadowStrength.Enabled,
                        onSwitched = App.Config.Graphics.shadowStrength.LockedSwitch(App.Config.GraphicsLock),
                    },
                    indentPadding: 12
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Self-shadow mitigation",
                    "a.k.a. \"Shadow acne\" fix (default: minimal, recommended: 0.1-0.3)",
                    opts: new SliderOption<float>(
                        minValue: 0.01f,
                        maxValue: 1.00f,
                        stepSize: 0.01f,
                        defaultValue: App.Config.Graphics.shadowBias.Value,

                        App.Config.Graphics.shadowBias.LockedSlide(App.Config.GraphicsLock)
                    ) {
                        isEnabled = App.Config.Graphics.shadowBias.Enabled,
                        onSwitched = App.Config.Graphics.shadowBias.LockedSwitch(App.Config.GraphicsLock),
                    },
                    indentPadding: 12
                );
            }

            {
                var pane = page.AddUIComponent<UIPanel>();
                pane.width = pane.parent.width - page.padding.horizontal;
                pane.padding = Helper.Padding(4, 12, 4, 0);
                pane.SetAutoLayout(LayoutDirection.Vertical, Helper.Padding(0, 0, 2, 0));
                pane.autoFitChildrenVertically = true;

                Helper.AddLabel(
                    ref pane,
                    "Postprocessing",
                    font: FontStore.Get(12),
                    color: Helper.RGB(220, 230, 250),
                    bullet: "LineDetailButton"
                );

                Helper.AddCheckBox(
                    ref pane,
                    "SMAA",
                    tooltip: null,
                    initialValue: App.Config.Graphics.smaaEnabled,
                    (c, isChecked) => {
                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.smaaEnabled = isChecked;
                        }
                    },
                    font: FontStore.Get(11),
                    indentPadding: 10
                );

                //var passes = ToolHelper.AddConfig(
                //    ref pane,
                //    "Passes",
                //    $"default: {Mod.Config.GraphicsDefault.smaaPasses}",
                //    opts: new SliderOption<float> {
                //        hasField = true,
                //        minValue = 0f,
                //        maxValue = 8f,
                //        stepSize = 1f,

                //        eventValueChanged = (c, value) => {
                //            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.smaaPasses, (int)Math.Floor(value));
                //        },
                //    },
                //    color: Helper.RGB(160, 160, 160),
                //    bullet: false
                //);
                //lock (App.Config.GraphicsLock) {
                //    passes.slider.value = App.Config.Graphics.smaaPasses;
                //}

                Helper.AddCheckBox(
                    ref pane,
                    "Film grain",
                    tooltip: null,
                    initialValue: App.Config.Graphics.filmGrainEnabled,
                    (c, isChecked) => {
                        lock (App.Config.GraphicsLock) {
                            App.Config.Graphics.filmGrainEnabled = isChecked;
                        }
                    },
                    font: FontStore.Get(11), indentPadding: 10
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Scale",
                    $"default: {Mod.Config.GraphicsDefault.filmGrainScale}",
                    opts: new SliderOption<float>(
                        minValue: 0.05f,
                        maxValue: 1f,
                        stepSize: 0.05f,
                        defaultValue: App.Config.Graphics.filmGrainScale,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.filmGrainScale, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Amount (scalar)",
                    $"default: {Mod.Config.GraphicsDefault.filmGrainAmountScalar}",
                    opts: new SliderOption<float>(
                        minValue: 0.05f,
                        maxValue: 1f,
                        stepSize: 0.05f,
                        defaultValue: App.Config.Graphics.filmGrainAmountScalar,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.filmGrainAmountScalar, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Amount (factor)",
                    $"default: {Mod.Config.GraphicsDefault.filmGrainAmountFactor}",
                    opts: new SliderOption<float>(
                        minValue: 0.01f,
                        maxValue: 1f,
                        stepSize: 0.01f,
                        defaultValue: App.Config.Graphics.filmGrainAmountFactor,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.filmGrainAmountFactor, value);
                        }
                    ) {
                        hasField = true,
                    },
                    color: Helper.RGB(160, 160, 160)
                );

                ToolHelper.AddConfig(
                    ref pane,
                    "Middle range",
                    $"default: {Mod.Config.GraphicsDefault.filmGrainMiddleRange}",
                    opts: new SliderOption<float>(
                        minValue: 0f,
                        maxValue: 1f,
                        stepSize: 0.02f,
                        defaultValue: App.Config.Graphics.filmGrainMiddleRange,

                        (c, value) => {
                            ToolHelper.LockedApply(App.Config.GraphicsLock, ref App.Config.Graphics.filmGrainMiddleRange, value);
                        }
                    ) {
                        hasField = true,

                    },
                    color: Helper.RGB(160, 160, 160)
                );
            }
        }
    }
}

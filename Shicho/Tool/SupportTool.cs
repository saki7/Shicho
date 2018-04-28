using Shicho.Core;

using UnityEngine;
//using UnityEngine.UI;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Shicho.Tool
{
    using UInput = UnityEngine.Input;

    class SupportTool : MonoBehaviour
    {
        public void Awake()
        {
            ID = GetInstanceID();
            cfg_ = App.Config.GUI.SupportTool;
            if (cfg_.Tab.Index > Tabs.Length) {
                cfg_.Tab.Index = 0;
            }
        }

        public void Update()
        {
            var keyMod = Input.KeyMod.None;

            if (UInput.GetKey(KeyCode.LeftControl) || UInput.GetKey(KeyCode.RightControl)) {
                keyMod |= Input.KeyMod.Ctrl;
            }
            if (UInput.GetKey(KeyCode.LeftAlt) || UInput.GetKey(KeyCode.RightAlt)) {
                keyMod |= Input.KeyMod.Alt;
            }
            if (UInput.GetKey(KeyCode.LeftShift) || UInput.GetKey(KeyCode.RightShift)) {
                keyMod |= Input.KeyMod.Shift;
            }

            if (UInput.GetKeyDown(App.Config.mainKey.Code)) {
                if ((App.Config.mainKey.Mod & keyMod) == App.Config.mainKey.Mod) {
                    cfg_.IsVisible = !cfg_.IsVisible;
                }
            }
        }

        public void OnGUI()
        {
            if (cfg_.IsVisible) {
                var style = new GUIStyle(GUI.skin.window) {
                    fontSize = Defaults.TitleFontSize,
                    onNormal = {
                        background = null,
                    },
                    //padding = {
                    //    top = 2,
                    //    bottom = 4,
                    //},
                    stretchHeight = true,
                };
                cfg_.Rect = GUI.Window(ID, cfg_.Rect, DrawWindow, "Shicho", style);
            }
        }

        class ShadowBiasMethod
        {
        }

        private delegate void TabContentFunc();

        private void DrawWindow(int id)
        {
            {
                var style = new GUIStyle(GUI.skin.label) {
                    fontSize = Defaults.TabFontSize,
                    onNormal = {
                        background = null,
                    },
                    margin = {
                        left = 0,
                        right = 0,
                    },
                    padding = {
                        top = Defaults.TabFontSize / 3,
                        left = 0,
                        right = 0,
                    },
                    alignment = TextAnchor.UpperCenter,
                    fixedHeight = Defaults.TabHeight,
                };

                cfg_.Tab.Index = GUI.Toolbar(new Rect(0, HUnit / 2, cfg_.Rect.width, HUnit), cfg_.Tab.Index, Tabs, style);
            }
            var tabID = Tabs[cfg_.Tab.Index];

            switch (tabID) {
            case "Graphics":
                GraphicsTab();
                break;

            case "Log":
                LogTab();
                break;

            default:
                throw new NotImplementedException($"unknown tab {tabID}");
            }

            // NB: last for entire rect
            GUI.DragWindow();
        }

        private void GraphicsTab()
        {
            var y = Defaults.Top;
            {
                var style = new GUIStyle(GUI.skin.box) {
                    fontSize = Defaults.FontSize,
                };
                GUI.BeginGroup(new Rect(0, y, sliderWidth, 400), style);
            }
            var shadowBiasMethods = new Dictionary<string, ShadowBiasMethod>() {
                {"Default", new ShadowBiasMethod()},
                {"Hybrid (Relight v2018.04)", new ShadowBiasMethod()},
            };

            {
                var style = new GUIStyle(GUI.skin.toggle) {
                    fontSize = Defaults.FontSize,
                };
                GUILayout.SelectionGrid(0, shadowBiasMethods.Keys.ToArray(), 1, style);
            }

            GUI.EndGroup();
        }

        private void LogTab()
        {
            GUI.Label(new Rect(0, HUnit * 4, cfg_.Rect.width, HUnit), "Not implemented");
        }

        readonly string[] Tabs = new[] {"Graphics", "Log"};
        const float HUnit = 24f;
        const int sliderWidth = 200;

        static class Defaults
        {
            public const int FontSize = 16;

            public const int TitleFontSize = 16;
            public const int TabFontSize = 16;

            public const float TabHeight = TabFontSize * 2.2f;
            public const float Top = TitleFontSize * 1.1f + TabHeight;
        }

        public static readonly Rect DefaultRect = new Rect(
            0f, 0f, 280f, 800f
        );

        private Mod.Config.GUIWindowData cfg_;

        private int ID { get; set; }
    }
}

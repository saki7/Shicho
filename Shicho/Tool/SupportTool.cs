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
                var style = new GUIStyle(GUI.skin.window);
                style.onNormal.background = null;

                cfg_.Rect = GUI.Window(ID, cfg_.Rect, DrawWindow, "Shicho", style);
            }
        }

        class ShadowBiasMethod
        {
        }

        private delegate void TabContentFunc();

        private void DrawWindow(int id)
        {
            cfg_.Tab.Index = GUI.Toolbar(new Rect(0, HUnit, cfg_.Rect.width, HUnit), cfg_.Tab.Index, Tabs);
            var tabID = Tabs[cfg_.Tab.Index];

            switch (tabID) {
            case "Graphics":
                GraphicsTab();
                break;

            default:
                throw new NotImplementedException($"unknown tab {tabID}");
            }

            // NB: last for entire rect
            GUI.DragWindow();
        }

        private void GraphicsTab()
        {
            GUI.BeginGroup(new Rect(0, 0, sliderWidth, 400));
            var shadowBiasMethods = new Dictionary<string, ShadowBiasMethod>() {
                {"Default", new ShadowBiasMethod()},
                {"Hybrid (Relight v2018.04)", new ShadowBiasMethod()},
            };
            GUILayout.SelectionGrid(0, shadowBiasMethods.Keys.ToArray(), 1, "Toggle");
            GUI.EndGroup();
        }

        readonly string[] Tabs = new[] {"Graphics", "Log"};
        const float HUnit = 24f;
        const int sliderWidth = 200;

        public static readonly Rect DefaultRect = new Rect(
            0f, 0f, 280f, 800f
        );

        private Mod.Config.GUIWindowData cfg_;

        private int ID { get; set; }
    }
}

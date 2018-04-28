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
        public void Start()
        {
            ID = GetInstanceID();
            cfg_ = App.Config.GUI.SupportTool;
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
                cfg_.Rect = GUI.Window(ID, cfg_.Rect, DrawWindow, "Shicho");
            }
        }

        class ShadowBiasMethod
        {
        }

        private void DrawWindow(int id)
        {
            GUI.BeginGroup(new Rect(0, 0, sliderWidth, 400));
            var shadowBiasMethods = new Dictionary<string, ShadowBiasMethod>() {
                {"Default", new ShadowBiasMethod()},
                {"Hybrid (Relight v2018.04)", new ShadowBiasMethod()},
            };
            GUILayout.SelectionGrid(0, shadowBiasMethods.Keys.ToArray(), 1, "Toggle");
            GUI.EndGroup();

            // NB: last for entire rect
            GUI.DragWindow();
        }

        const int sliderWidth = 200;
        public static readonly Rect DefaultRect = new Rect(
            0f, 0f, 280f, 800f
        );

        private Mod.Config.GUIData cfg_;

        private int ID { get; set; }
    }
}

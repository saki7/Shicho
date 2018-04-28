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
        }

        public void Update()
        {
            var keyMod = Input.KeyMod.None;

            if (UInput.GetKey(KeyCode.LeftControl) || UInput.GetKey(KeyCode.RightControl)) {
                keyMod |= Input.KeyMod.Ctrl;
            } else if (UInput.GetKey(KeyCode.LeftAlt) || UInput.GetKey(KeyCode.RightAlt)) {
                keyMod |= Input.KeyMod.Alt;
            } else if (UInput.GetKey(KeyCode.LeftShift) || UInput.GetKey(KeyCode.RightShift)) {
                keyMod |= Input.KeyMod.Shift;
            }

            if (UInput.GetKeyDown(App.Config.boundKey)) {
                if ((App.Config.boundKeyMod & keyMod) == App.Config.boundKeyMod) {
                    isVisible_ = !isVisible_;
                }
            }
        }

        public void OnGUI()
        {
            if (isVisible_) {
                GUI.Window(ID, windowRect_, DrawWindow, "Shicho");
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
        }

        const int sliderWidth = 200;

        private int ID { get; set; }
        private Rect windowRect_ = new Rect(new Vector2(0f, 0f), new Vector2(280f, 800f));

        private bool isVisible_ = false;
    }
}

using Shicho.Core;

using ColossalFramework.UI;

using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Shicho.Tool
{
    using Shicho.GUI;
    using System.IO;
    using UInput = UnityEngine.Input;

    class SupportTool : ToolBase
    {
        class GraphicsPanel : GUI.Panel
        {
            public override void Start()
            {
                base.Start();
                backgroundSprite = "Airport";
                autoSize = true;
                width = 220;
                height = 240;
            }
        }

        public override void Awake()
        {
            base.Awake();

            Tabs = new[] {
                new TabTemplate() {
                    name = "Graphics",
                    icons = new IconSet() {
                        Normal = "MapSettingsSnapshot",
                        Hovered = "MapSettingsSnapshotHovered",
                        Pressed = "MapSettingsSnapshotPressed",
                        Focused = "MapSettingsSnapshotFocused",
                    },
                    content = new GraphicsPanel(),
                },
                new TabTemplate() {
                    name = "Log",
                    content = new GUI.Panel(),
                },
            };
        }

        public override void Start()
        {
            base.Start();
            Title = $"Shicho (v{Mod.ModInfo.Version})";

            Window.Icon = Resources.shicho_logo_outline_white_24;
            Window.Show();
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
                    SetVisible(!Config.IsVisible);
                }
            }
        }

        class ShadowBiasMethod
        {
        }

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

        public override TabbedWindowConfig ConfigProxy {
            get => App.Config.GUI.SupportTool;
            set => App.Config.GUI.SupportTool = value;
        }
    }
}

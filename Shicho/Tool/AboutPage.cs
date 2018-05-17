extern alias Cities;

using Shicho.Core;
using Shicho.GUI;

using ColossalFramework.UI;
using UnityEngine;

using System;


namespace Shicho.Tool
{
    using Shicho.GUI.Extension;

    public static class AboutPage
    {
        public static void Setup(ref UIPanel page)
        {
            page.padding = Helper.Padding(8, 12);

            var version = page.AddUIComponent<UILabel>();
            var font = UnityEngine.Object.Instantiate(version.font);
            font.size = 12;

            version.font = font;
            version.text = $"Mod version: {Mod.ModInfo.VersionStr}";

            AddBar(ref page);
            AddInfo(ref page, "Game version", Cities::BuildConfig.applicationVersion);

            AddBar(ref page);
            AddInfo(ref page, "RAM", Util.ToByteUnits((Int64)SystemInfo.systemMemorySize * 1024 * 1024));
            AddInfo(ref page, "VRAM", Util.ToByteUnits((Int64)SystemInfo.graphicsMemorySize * 1024 * 1024));
        }

        private static void AddBar(ref UIPanel page)
        {
            var bar = page.AddUIComponent<UISprite>();
            bar.width = bar.parent.width;
            bar.height = 3;
            bar.spriteName = "RocketProgressBarFill";
            bar.color = Helper.RGB(80, 80, 80);
        }

        private static void AddInfo(ref UIPanel page, string name, string value)
        {
            var label = page.AddUIComponent<UILabel>();
            label.font = FontStore.Get(12);
            label.text = $"{name}: {value}";
        }
    }
}

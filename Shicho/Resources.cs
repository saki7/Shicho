using UnityEngine;
using System;
using System.Reflection;

namespace Shicho
{
    internal static class Resources
    {
        public static readonly Texture2D shicho_logo_outline_white_24
            = LoadImage("shicho_logo_outline_white_24.png", 512, 512);

        private static Texture2D LoadImage(string name, int width, int height)
        {
            var asm = Assembly.GetExecutingAssembly();
            var s = asm.GetManifestResourceStream("Shicho.Resources." + name);

            var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            var blob = new byte[s.Length];
            s.Read(blob, 0, blob.Length);

            texture.filterMode = FilterMode.Trilinear;
            texture.LoadImage(blob);
            return texture;
        }
    }
}

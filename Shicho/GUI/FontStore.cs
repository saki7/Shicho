extern alias Cities;

using ColossalFramework.UI;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Shicho.GUI
{
    using Core.EnumerableExtension;

    public enum FontType : int
    {
        Serif,
        SansSerif,
        Monospace,
    }

    public struct FontSpec
        : IEquatable<FontSpec>
        , IComparable<FontSpec>
    {
        public FontType type;
        public uint size;

        public int CompareTo(FontSpec other)
        {
            if (type < other.type) {
                return -1;
            } else if (type > other.type) {
                return 1;
            }

            // same type...
            if (size < other.size) {
                return -1;
            } else if (size > other.size) {
                return 1;
            } else {
                return 0;
            }
        }

        public bool Equals(FontSpec other)
        {
            return size == other.size && type == other.type;
        }
    }

    internal class FontStore : IDisposable
    {
        const uint DefaultFontSize = 10;

        public static void Load()
        {
            if (instance_ != null) return;
            instance_ = new FontStore();

            {
                Instance.defaultFont_ = UnityEngine.Object.Instantiate(UIView.GetAView().defaultFont);
            }

            Instance.store_ = new Dictionary<FontSpec, UIFont>();
            foreach (uint size in new [] {10, 11, 12, 15 /* title bar */}) {
                var font = UnityEngine.Object.Instantiate(Instance.defaultFont_);
                font.size = (int)size;
                Instance.store_.Add(new FontSpec{type = FontType.SansSerif, size = size}, font);
            }
        }

        public static UIFont Get(uint size)
        {
            return GetType(FontType.SansSerif, size);
        }

        public static UIFont GetType(FontType type, uint size)
        {
            return Instance.store_[new FontSpec{type = type, size = size}];
        }

        public static void Unload()
        {
            instance_.Dispose();
            instance_ = null;
        }

        public void Dispose()
        {
            if (store_ == null) return;

            foreach (var font in store_.Values) {
                //Core.Log.Debug($"disposing font: {font.name} ({font.size})");
                UnityEngine.Object.DestroyImmediate(font);
            }
            store_.Clear();
            store_ = null;

            UnityEngine.Object.DestroyImmediate(defaultFont_);
            defaultFont_ = null;
        }

        private static FontStore instance_;
        public static FontStore Instance { get => instance_; }

        private FontStore() {}

        private UIFont defaultFont_;
        private Dictionary<FontSpec, UIFont> store_;
    }

}

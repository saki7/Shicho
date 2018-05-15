using Shicho.Core;

using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;


namespace Shicho.Mod
{
    using KeyMod = Input.KeyMod;
    using KeyModMap = Dictionary<Input.KeyMod, string>;

    [Serializable]
    [XmlRoot(ElementName = "Shicho")]
    public class Config
    {
        public void Save(string path)
        {
            Log.Info("saving...");

            var serializer = new XmlSerializer(typeof(Config));
            var writer = new StreamWriter(path);
            serializer.Serialize(writer, this);
            writer.Close();

            // Log.Info("saved.");
        }

        public static Config LoadFile(string path)
        {
            // Log.Debug("loading...");

            if (!File.Exists(path)) {
                Log.Warn($"file '{path}' does not exist!");
                return new Config();
            }

            var xmlSerialiser = new XmlSerializer(typeof(Config));
            using (var reader = new StreamReader(path)) {
                var cfg = xmlSerialiser.Deserialize(reader) as Config;

                if (cfg.Graphics.shadowBias == null) {
                    cfg.Graphics = GraphicsDefault.Clone() as GraphicsData;
                }

                if (cfg.AI.regenChance == null) {
                    cfg.AI = AIDefault.Clone() as AIData;
                }
                return cfg;
            }
            // Log.Debug("loaded.");
        }

        [Serializable]
        public class Switchable<T>
        {
            [NonSerialized]
            [XmlIgnore]
            public Type type = typeof(T);

            public bool Enabled { get; set; } = false;
            public T Value { get; set; }

            public delegate void SwitchHandler(ColossalFramework.UI.UIComponent c, bool isEnabled);
            public delegate void SlideHandler(ColossalFramework.UI.UIComponent c, float value);

            public SwitchHandler LockedSwitch(object lockTarget)
            {
                return (c, isEnabled) => {
                    lock (lockTarget) {
                        Enabled = isEnabled;
                    }
                };
            }

            public SlideHandler LockedSlide(object lockTarget)
            {
                return (c, value) => {
                    lock (lockTarget) {
                        Value = (T)Convert.ChangeType(value, type);
                    }
                };
            }

            public void AssignIfEnabled(ref T val)
            {
                if (Enabled) {
                    val = Value;
                }
            }

            public static implicit operator bool(Switchable<T> self)
            {
                return self.Enabled;
            }

            public static explicit operator T(Switchable<T> self)
            {
                return self.Value;
            }
        }

        public void ChangeKeyBinding(KeyMod? mod, KeyCode? key = null)
        {
            if (mod.HasValue) {
                mainKey.Mod = mod.Value;
            }

            if (key.HasValue) {
                mainKey.Code = key.Value;
            }
        }

        public static readonly KeyModMap ModMap = new KeyModMap() {
            {KeyMod.Ctrl, "Ctrl"}, {KeyMod.Shift, "Shift"}, {KeyMod.Alt, "Alt"},
            {KeyMod.Ctrl | KeyMod.Shift, "Ctrl-Shift"},
            {KeyMod.Ctrl | KeyMod.Alt, "Ctrl-Alt"},
            {KeyMod.Ctrl | KeyMod.Alt | KeyMod.Shift, "Ctrl-Alt-Shift"},
            {KeyMod.Alt | KeyMod.Shift, "Alt-Shift"},
        };

        [Serializable]
        public struct BoundKeyData
        {
            public KeyMod Mod { get; set; }
            public KeyCode Code { get; set; }

            public override string ToString()
            {
                return $"{ModMap[Mod]}-{Code}";
            }
        }
        public BoundKeyData mainKey = new BoundKeyData() {
            Mod = KeyMod.Alt,
            Code = KeyCode.S,
        };

        [Serializable]
        public class GUIConfigSet
        {
            public GUI.TabbedWindowConfig SupportTool;
        }

        public GUIConfigSet GUI = new GUIConfigSet() {
            SupportTool = new GUI.TabbedWindowConfig() {
                IsVisible = true,
                Rect = Tool.SupportTool.DefaultRect,
            },
        };

        [NonSerialized]
        [XmlIgnore]
        public object GraphicsLock = new object(), AILock = new object();

        [Serializable]
        public abstract class ClonableData : ICloneable
        {
            public object Clone() => MemberwiseClone();
        }

        [Serializable]
        public class GraphicsData : ClonableData
        {
            public Switchable<float>
                shadowBias, shadowStrength, lightIntensity
            ;

            public float treeMoveFactor;
            public bool randomTrees, stopDistantTrees;
        }

        [Serializable]
        public class AIData : ClonableData
        {
            public Switchable<float> regenChance;
        }


        [NonSerialized]
        [XmlIgnore]
        public static readonly GraphicsData GraphicsDefault = new GraphicsData() {
            shadowBias     = new Switchable<float>{Enabled = true,  Value = 0.20f},
            shadowStrength = new Switchable<float>{Enabled = false, Value = 0.8f},
            lightIntensity = new Switchable<float>{Enabled = false, Value = 4.2f},

            treeMoveFactor = 0.0f,
            randomTrees = true,
            stopDistantTrees = true,
        };

        [NonSerialized]
        [XmlIgnore]
        public static readonly AIData AIDefault = new AIData() {
            regenChance = new Switchable<float>{Enabled = true, Value = 0.20f},
        };

        [SerializeField]
        [XmlElement(ElementName = "Graphics")]
        public GraphicsData Graphics = GraphicsDefault.Clone() as GraphicsData;

        [SerializeField]
        [XmlElement(ElementName = "AI")]
        public AIData AI = AIDefault.Clone() as AIData;
    }
}

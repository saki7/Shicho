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

            var xmlSerializer = new XmlSerializer(typeof(Config));
            using (var reader = new StreamReader(path)) {
                var cfg = xmlSerializer.Deserialize(reader) as Config;
                System.Version lastVersion = null;

                if (cfg.lastVersion != null) {
                    try {
                        lastVersion = new System.Version(cfg.lastVersion);
                    } catch (ArgumentException e) {
                        Log.Debug($"invalid last verion number \"{cfg.lastVersion}\": {e}");
                    }
                }

                if (lastVersion == null || lastVersion < ModInfo.Version) {
                    Log.Info($"this version ({ModInfo.Version}) is newer than last version ({cfg.lastVersion})");
                    cfg.lastVersion = ModInfo.Version.ToString();
                    cfg.UI.masterOpacity = 1.0f;
                    cfg.UI.supportToolOpacity = 1.0f;
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
        public object GraphicsLock = new object(), UILock = new object(), AILock = new object();

        [Serializable]
        public struct GraphicsData
        {
            public Switchable<float>
                shadowBias, shadowStrength, lightIntensity
            ;

            public float treeMoveFactor;
            public bool randomTrees, stopDistantTrees;


            public bool dofEnabled;

            [NonSerialized]
            [XmlIgnore]
            public bool dofDebug;

            public Switchable<float> fieldOfView;

            public float
                dofAperture,
                dofFocalDistance, dofFocalRange,
                dofMaxBlurSize
            ;

            public bool dofNearBlur;
            public float dofFGOverlap;

            // DX11
            public float
                dofBokehScale,
                dofBokehIntensity,
                dofBokehMinLuminanceThreshold,
                dofBokehSpawnHeuristic
            ;

            public bool tiltShiftEnabled;
            public TiltShiftEffect.TiltShiftMode tiltShiftMode;
            public float tiltShiftMaxBlurSize, tiltShiftAreaSize;

            public bool filmGrainEnabled;
            public float
                filmGrainScale,
                filmGrainAmountScalar,
                filmGrainAmountFactor,
                filmGrainMiddleRange
            ;

            public bool smaaEnabled;
            public int smaaPasses;
        }

        [Serializable]
        public struct UIData
        {
            // for advanced users. change this in config file if you want
            public bool alwaysFullRect;
            public float masterOpacity, supportToolOpacity;

            [NonSerialized]
            [XmlIgnore]
            public bool
                masterToolbarVisibility
            ;

            public bool
                areaBordersVisiblity,
                districtNamesVisibility,
                propMarkersVisibility,

                notificationsVisibility
            ;

            public bool tutorialDisabled;
        }

        [Serializable]
        public struct AIData
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


            dofEnabled = false,
            dofDebug = false,

            fieldOfView = new Switchable<float>{Enabled = false, Value = 90f},

            dofAperture = 0.98f,
            dofFocalDistance = 1.52f,
            dofFocalRange = 6.04f,
            dofMaxBlurSize = 2.86f,

            dofNearBlur = false,
            dofFGOverlap = 5.0f,

            dofBokehScale = 10.78f,
            dofBokehIntensity = 13.85f,
            dofBokehMinLuminanceThreshold = 3.75f,
            dofBokehSpawnHeuristic = 3.0f,

            tiltShiftEnabled = false,
            tiltShiftMode = TiltShiftEffect.TiltShiftMode.TiltShiftMode,
            tiltShiftMaxBlurSize = 1.0f,
            tiltShiftAreaSize = 1.0f,

            filmGrainEnabled = false,
            filmGrainScale = 0.5f,
            filmGrainAmountScalar = 0.3f,
            filmGrainAmountFactor = 0.13f,
            filmGrainMiddleRange = 0.48f,

            smaaEnabled = true,
            smaaPasses = 1,
        };

        [NonSerialized]
        [XmlIgnore]
        public static readonly UIData UIDefault = new UIData() {
            alwaysFullRect = true,
            masterOpacity = 0.75f,
            supportToolOpacity = 0.9f,

            masterToolbarVisibility  = true,
            propMarkersVisibility    = true,

            areaBordersVisiblity     = false,
            districtNamesVisibility  = false,

            notificationsVisibility  = false,

            tutorialDisabled = true,
        };

        [NonSerialized]
        [XmlIgnore]
        public static readonly AIData AIDefault = new AIData() {
            regenChance = new Switchable<float>{Enabled = true, Value = 0.20f},
        };

        [SerializeField]
        public string lastVersion = null;

        [SerializeField]
        [XmlElement(ElementName = "Graphics")]
        public GraphicsData Graphics = GraphicsDefault;

        [SerializeField]
        [XmlElement(ElementName = "UI")]
        public UIData UI = UIDefault;

        [SerializeField]
        [XmlElement(ElementName = "AI")]
        public AIData AI = AIDefault;
    }
}

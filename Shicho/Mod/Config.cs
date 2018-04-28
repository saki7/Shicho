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
                return xmlSerialiser.Deserialize(reader) as Config;
            }
            // Log.Debug("loaded.");
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
        }

        public BoundKeyData mainKey = new BoundKeyData() {
            Mod = KeyMod.Alt,
            Code = KeyCode.S,
        };

        public class GraphicData
        {
            float shadowBias = 0.01f, shadowNormalBias = 0.3f;
        }
        public GraphicData Graphic { get; } = new GraphicData();
    }
}

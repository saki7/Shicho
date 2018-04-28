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
    [XmlRoot(ElementName = "SavedConfig")]
    public class Config
    {
        public void Save()
        {
            Log.Info("saving...");

            var serializer = new XmlSerializer(typeof(Config));
            var writer = new StreamWriter(FileName);
            serializer.Serialize(writer, this);
            writer.Close();

            Log.Info("saved.");
        }

        public void Load()
        {
            Log.Info("loading...");

            if (!File.Exists(FileName)) {
                Log.Warn($"file '{FileName}' does not exist!");
                return;
            }

            var xmlSerialiser = new XmlSerializer(typeof(Config));
            var reader = new StreamReader(FileName);
            var mgr = xmlSerialiser.Deserialize(reader) as Config;
            reader.Close();

            Log.Info("loaded.");
        }

        public static readonly KeyModMap ModMap = new KeyModMap() {
            {KeyMod.Ctrl, "Ctrl"}, {KeyMod.Shift, "Shift"}, {KeyMod.Alt, "Alt"},
            {KeyMod.Ctrl | KeyMod.Shift, "Ctrl-Shift"},
            {KeyMod.Ctrl | KeyMod.Alt, "Ctrl-Alt"},
            {KeyMod.Ctrl | KeyMod.Alt | KeyMod.Shift, "Ctrl-Alt-Shift"},
            {KeyMod.Alt | KeyMod.Shift, "Alt-Shift"},
        };

        public KeyMod boundKeyMod = KeyMod.Alt;
        public KeyCode boundKey = KeyCode.S;

        [NonSerialized]
        private const string FileName = ModInfo.ID + ".xml";
    }
}

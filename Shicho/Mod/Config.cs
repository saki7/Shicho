using Shicho.Core;

using System;
using System.IO;
using System.Xml.Serialization;


namespace Shicho.Mod
{
    [Serializable()]
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

        [NonSerialized]
        private const string FileName = ModInfo.ID + ".xml";
    }
}

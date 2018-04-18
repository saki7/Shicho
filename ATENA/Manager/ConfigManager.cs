using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;


namespace ATENA.Manager
{
    [Serializable()]
    [XmlRoot(ElementName = "SavedConfig")]
    public class ConfigManager
    {
        public static ConfigManager Instance {
            get => instance_;
        }

        private ConfigManager() {}

        public void Save()
        {
            Log.Info("saving...");

            var serializer = new XmlSerializer(typeof(ConfigManager));
            var writer = new StreamWriter(FileName);
            serializer.Serialize(writer, instance_);
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

            var xmlSerialiser = new XmlSerializer(typeof(ConfigManager));
            var reader = new StreamReader(FileName);
            var mgr = xmlSerialiser.Deserialize(reader) as ConfigManager;
            reader.Close();

            if (mgr != null) {
                instance_ = mgr;
            }
            Log.Info("loaded.");
        }

        [NonSerialized]
        private const string FileName = "ATENA.xml";

        [NonSerialized]
        private static ConfigManager instance_ = new ConfigManager();
    }
}

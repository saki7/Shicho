using ICities;
using UnityEngine;

using System;
using System.IO;


namespace ATENA
{
    internal class Atena
        : MonoBehaviour
    {
        private static Atena instance_;
        public static Atena Instance { get => instance_; }
        internal static void SetInstance(Atena a) { instance_ = a; }

        public Atena()
        {
            try {
                Log.Info("initializing...");

                Manager.ConfigManager.Instance.Load();
                cfg = GameObject.Find(ModInfo.ID).AddComponent<AtenaConfig>();

                R = new ColossalFramework.Math.Randomizer(GetDeviceSeed());

                Log.Info("initialized!");

            } catch (Exception e) {
                Log.Error($"failed to initialize: '{e}'");
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            cfg.Populate(helper);
        }

        public static ulong GetDeviceSeed()
        {
            return 114514;
        }

        private AtenaConfig cfg;

        internal ColossalFramework.Math.Randomizer R;
    }
}

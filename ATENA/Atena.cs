using UnityEngine;
using System;


namespace ATENA
{
    public class Atena
        : MonoBehaviour
    {
        public void Initialize()
        {
            try {
                Log.Info("initializing...");
                Manager.ConfigManager.Instance.Load();

                Log.Info("initialized!");

            } catch (Exception e) {
                Log.Error($"failed to initialize: '{e}'");
            }
        }
    }
}

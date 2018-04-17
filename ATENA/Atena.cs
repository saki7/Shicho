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
                Log.Info("initialized!");

            } catch (Exception e) {
                Log.Error($"failed to initialize: '{e}'");
            }
        }

        //public void OnLevelLoaded(LoadMode mode)
        //{
        //    throw new Exception("Hello World!");
        //    Debug.Log("ffff");
        //}

        //public void OnLevelUnloading()
        //{
        //    //Log.Info("terminating...");
        //    //Log.Info("terminated!");
        //}

        //public override void OnUpdate(float realTimeDelta, float smlTimeDelta)
        //{
        //}

        //public override void OnBeforeSimulationFrame()
        //{
        //}

        //public override void OnAfterSimulationFrame()
        //{
        //}

        //public override void OnBeforeSimulationTick()
        //{
        //}

        //public override void OnAfterSimulationTick()
        //{
        //}

        // private GameObject gobj;
    }
}

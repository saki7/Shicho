using ICities;
using ColossalFramework;

using UnityEngine;

using Harmony;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATENA.Patcher.VehicleManagerPatch
{
    //[HarmonyPatch(typeof(WindowStack))]
    //[HarmonyPatch("Add")]
    //[HarmonyPatch(new Type[] { typeof(Window) })]

    [HarmonyPatch(typeof(VehicleManager))]
    [HarmonyPatch("CreateVehicle")]
    //[HarmonyPatch(new Type[] {
    //    typeof(ushort), // vehicle
    //    typeof(ColossalFramework.Math.Randomizer),
    //    typeof(VehicleInfo),
    //    typeof(Vector3), // position
    //    typeof(TransferManager.TransferReason),
    //    typeof(bool), // transferToSource
    //    typeof(bool), // transferToTarget
    //})]
    class CreateVehicle
    {
        static void Prefix()
        {
            // Debug.Log("patching!!!!");
            // Log.Warn("patching!");

            var mgr = Singleton<VehicleManager>.instance;
            Log.Warn($"current: {mgr.m_vehicleCount}");
            mgr.m_vehicleCount = 1;
        }
        //static void Prefix(VehicleManager instance)
        //{
        //    Log.Warn("patching!");
        //}
    }
}

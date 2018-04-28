extern alias Cities;
using Harmony;

namespace Shicho.Patcher.VehicleManagerPatch
{
    //[HarmonyPatch(typeof(WindowStack))]
    //[HarmonyPatch("Add")]
    //[HarmonyPatch(new Type[] { typeof(Window) })]

    [HarmonyPatch(typeof(Cities::VehicleManager))]
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
        static void Prefix(ref Cities::VehicleInfo info, ref Cities::TransferManager.TransferReason type)
        {
            // Debug.Log("patching!!!!");
            // Log.Warn("patching!");

            // var mgr = __instance;
            //Log.Warn($"current: {mgr.m_vehicleCount}");
            // mgr.m_totalTrafficFlow = 50000;
            // mgr.m_parkedCount = 227;
            // mgr.m_vehicleCount = 2123;

            //info.m_isLargeVehicle = true;
            //info.m_vehicleType = VehicleInfo.VehicleType.Plane;
            //info.m_maxSpeed = 100;
            //info.m_acceleration = 3;

            //type = Cities::TransferManager.TransferReason.Worker0;
        }
        //static void Prefix(VehicleManager instance)
        //{
        //    Log.Warn("patching!");
        //}
    }
}

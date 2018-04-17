using ICities;
using ColossalFramework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ATENA.Extension
{
    internal class ManagerSet
    {
        public VehicleManager vehicle;
    }

    public class ThreadExt
        : ThreadingExtensionBase
    {
        public override void OnCreated(IThreading instance)
        {
            Log.Info("created");

            mgr = new ManagerSet{
                vehicle = Singleton<VehicleManager>.instance,
            };

            Log.Warn(mgr.vehicle.m_maxTrafficFlow);
        }

        public override void OnReleased()
        {
            Log.Info("released");
        }

        public override void OnUpdate(float realDelta, float simuDelta)
        {
        }

        public override void OnBeforeSimulationFrame()
        {
        }

        public override void OnAfterSimulationFrame()
        {
        }

        public override void OnBeforeSimulationTick()
        {
        }

        public override void OnAfterSimulationTick()
        {
        }

        private ManagerSet mgr;
    }
}

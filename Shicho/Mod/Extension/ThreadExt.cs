extern alias Cities;

using Shicho.Core;

using ICities;

namespace Shicho.Mod.Extension
{
    class ThreadExt
        : ThreadingExtensionBase
    {
        public override void OnCreated(IThreading instance)
        {
            Log.Debug("created");
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
    }
}

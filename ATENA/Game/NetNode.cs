extern alias CitiesL;

using ATENA.Core;

using ColossalFramework;


namespace ATENA.Game
{
    using Flags = CitiesL.NetNode.Flags;
    class NetNode : IGameObject<CitiesL.NetNode, Flags>
    {
        public NetNode()
        {

        }

        public static bool HasFlags(CitiesL.NetNode obj, Flags flags)
        {
            return (obj.m_flags & flags) != Flags.None;
        }

        bool IGameObject<CitiesL.NetNode, Flags>.HasFlags(CitiesL.NetNode obj, Flags flags)
        {
            return NetNode.HasFlags(obj, flags);
        }

        public void Dispose() {}
    }
}

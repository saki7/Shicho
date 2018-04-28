﻿extern alias Cities;

using ATENA.Core;

using ColossalFramework;


namespace ATENA.Game
{
    using Flags = Cities::NetNode.Flags;
    class NetNode : IGameObject<Cities::NetNode, Flags>
    {
        public NetNode()
        {

        }

        public static bool HasFlags(Cities::NetNode obj, Flags flags)
        {
            return (obj.m_flags & flags) != Flags.None;
        }

        bool IGameObject<Cities::NetNode, Flags>.HasFlags(Cities::NetNode obj, Flags flags)
        {
            return NetNode.HasFlags(obj, flags);
        }

        public void Dispose() {}
    }
}

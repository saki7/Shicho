using System;

namespace ATENA.Game
{
    internal interface IGameObject<T, FlagsT> : IDisposable
    {
        bool HasFlags(T obj, FlagsT flags);
    }
}

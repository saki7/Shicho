using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATENA.Game
{
    public class GameError : Exception
    {
        public GameError(Type type, string message)
            : base($"[{type}] {message}")
        {}
    }
}

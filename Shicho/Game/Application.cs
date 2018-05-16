extern alias Cities;

using System;
using System.Linq;


namespace Shicho.Game
{
    public static class Application
    {
        public static readonly string[] GamePanels = new [] {
            "CityInfo",

        }.Select(name => $"(Library) {name}Panel").ToArray();
    }
}

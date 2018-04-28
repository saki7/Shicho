using Shicho.Core;

using ICities;
using System;

namespace Shicho.Loader
{
    class LevelLoader : LoadingExtensionBase
    {
        override public void OnLevelLoaded(LoadMode mode)
        {
            App.Instance.LoadLevelData();
        }

        override public void OnLevelUnloading()
        {
            App.Instance.UnloadLevelData();
        }
    }
}

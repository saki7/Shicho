using ATENA.Core;

using ICities;

namespace ATENA.Mod
{
    public class Mod : IUserMod
    {
        public string Name { get => ModInfo.ID; }
        public string Description { get => ModInfo.Description; }

        public void OnEnabled()
        {
            Bootstrapper.Instance.Bootstrap();
            //Log.Info("OnEnabled");

            if (cfg_ != null) {
                Log.Warn("old Config instance remaining");
            }

            cfg_ = new Config();
            cfg_.Load();
        }

        public void OnDisabled()
        {
            try {
                //Log.Info("OnDisabled");
                cfg_.Save();

            } finally {
                Bootstrapper.Instance.Cleanup();
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
            => Atena.Instance.OnSettingsUI(helper);

        private Config cfg_;
    }
}

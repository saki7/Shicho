using ICities;

namespace ATENA
{
    public class Mod : IUserMod
    {
        public string Name
        {
            get {
                // Dirty hack, as in MT
                Bootstrapper.Bootstrap();
                return ModInfo.ID;
            }
        }
        public string Description => ModInfo.Description;

        public Mod()
        {
            Bootstrapper.Bootstrap();
        }

        //public string Description {
        //    get {
        //        return Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
        //            .OfType<AssemblyDescriptionAttribute>()
        //            .FirstOrDefault().ToString()
        //        ;
        //    }
        //}

        //private static readonly System.Diagnostics.FileVersionInfo VersionInfo =
        //    System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location)
        //;
    }
}

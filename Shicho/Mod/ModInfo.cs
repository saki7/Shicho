using System.Diagnostics;
using System.Reflection;

namespace Shicho.Mod
{
    public static class ModInfo
    {
        public const string
            ID = "Shicho",
            Description = "Fundamental support mod for hardcore builders",
            COMIdentifier = "com.setnahq.shicho"
        ;

        public static string Version {
            get => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        public static FileVersionInfo VersionInfo {
            get => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        }
    }
}

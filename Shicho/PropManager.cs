extern alias Cities;

using Shicho.Core;

using System.Linq;


namespace Shicho
{
    class PropManager
    {
        public PropManager()
        {}

        public void Fetch()
        {
        }

        private static Cities::PropManager mgr_ = Cities::PropManager.instance;
    }
}

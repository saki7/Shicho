extern alias Cities;

namespace Shicho.Game
{
    using Flags = Cities::District.Flags;

    class District : IGameObject<Cities::District, Flags>
    {
        public District()
        {

        }

        public void Dispose() {}

        static bool HasFlags(Cities::District obj, Flags flags)
        {
            return false;
        }
        bool IGameObject<Cities::District, Flags>.HasFlags(Cities::District obj, Flags flags)
        {
            return HasFlags(obj, flags);
        }
    }
}

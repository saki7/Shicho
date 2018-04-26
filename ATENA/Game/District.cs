extern alias CitiesL;

namespace ATENA.Game
{
    using Flags = CitiesL.District.Flags;

    class District : IGameObject<CitiesL.District, Flags>
    {
        public District()
        {

        }

        public void Dispose() {}

        static bool HasFlags(CitiesL.District obj, Flags flags)
        {
            return false;
        }
        bool IGameObject<CitiesL.District, Flags>.HasFlags(CitiesL.District obj, Flags flags)
        {
            return HasFlags(obj, flags);
        }
    }
}

extern alias CitiesL;
using ColossalFramework;
using System;

namespace ATENA.Game
{
    class CitizenID
    {
        public uint value;
        public override string ToString() => value.ToString();
    }

    class Citizen : IEquatable<Citizen>
    {
        public Citizen(uint age, uint family, CitiesL.Citizen.Gender? gender = null)
        {
            var success = false;

            if (gender.HasValue) {
                success = mgr.CreateCitizen(out id_.value, (int)age, (int)family, ref Atena.Instance.R, gender.Value);
            } else {
                success = mgr.CreateCitizen(out id_.value, (int)age, (int)family, ref Atena.Instance.R);
            }

            if (!success) {
                throw new GameError(typeof(Citizen), "failed: CreateCitizen()");
            }

            //var info = new CitiesL.CitizenInfo();
        }

        public override string ToString()
        {
            return $"Citizen({ID})";
        }

        public bool Equals(Citizen rhs) => ID == rhs.ID;

        private static CitiesL.CitizenManager mgr = Singleton<CitiesL.CitizenManager>.instance;
        private CitizenID id_ = new CitizenID();
        public CitizenID ID { get => id_; }
    }
}

extern alias Cities;
using ColossalFramework;
using System;

namespace Shicho.Game
{
    class CitizenID
    {
        public uint value;
        public override string ToString() => value.ToString();
    }

    class Citizen : IEquatable<Citizen>
    {
        public Citizen(uint age, uint family, Cities::Citizen.Gender? gender = null)
        {
            var success = false;

            if (gender.HasValue) {
                success = mgr.CreateCitizen(out id_.value, (int)age, (int)family, ref App.Instance.R, gender.Value);
            } else {
                success = mgr.CreateCitizen(out id_.value, (int)age, (int)family, ref App.Instance.R);
            }

            if (!success) {
                throw new GameError(typeof(Citizen), "failed: CreateCitizen()");
            }

            //var info = new Cities::CitizenInfo();
        }

        public override string ToString()
        {
            return $"Citizen({ID})";
        }

        public bool Equals(Citizen rhs) => ID == rhs.ID;

        private static Cities::CitizenManager mgr = Singleton<Cities::CitizenManager>.instance;
        private CitizenID id_ = new CitizenID();
        public CitizenID ID { get => id_; }
    }
}

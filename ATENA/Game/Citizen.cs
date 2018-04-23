extern alias CitiesL;
using ColossalFramework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ATENA.Game
{
    class Citizen
    {
        public Citizen(int age, int family, CitiesL.Citizen.Gender? gender)
        {
            var success = false;

            if (gender.HasValue) {
                success = mgr.CreateCitizen(out id_, age, family, ref Atena.Instance.R, gender.Value);
            } else {
                success = mgr.CreateCitizen(out id_, age, family, ref Atena.Instance.R);
            }

            if (!success) {
                throw new GameError(typeof(Citizen), "failed: CreateCitizen()");
            }
        }

        private static CitiesL.CitizenManager mgr = Singleton<CitiesL.CitizenManager>.instance;
        private uint id_;
        public uint ID {
            get => id_;
        }
    }
}

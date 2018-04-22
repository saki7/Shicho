using ICities;
using ColossalFramework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATENA.Game
{
    class Citizen
    {
        private uint id_;
        public uint ID {
            get => id_;
        }

        public Citizen(int age, int family, global::Citizen.Gender? gender)
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

        private static CitizenManager mgr = Singleton<CitizenManager>.instance;
    }
}

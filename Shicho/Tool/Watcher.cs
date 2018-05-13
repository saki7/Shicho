extern alias Cities;

using Shicho.Core;

using UnityEngine;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;


namespace Shicho.Tool
{
    using PatchPair = KeyValuePair<MethodBase, MethodInfo>;
    using UpdateSpec = SortedList<float, UpdateHandler>;

    delegate void UpdateHandler();

    public class Watcher : MonoBehaviour
    {
        public void Awake()
        {
            elapsed_ = 0;
            lastFiredAt_ = 0;
            lastFiredAtMax_ = 0;

            r_ = new System.Random(App.GetDeviceSeedI());

            updaters_ = new UpdateSpec() {
                {3, UnpatchHostiles},
                {1, UpdateCitizen},
            };
            maxInterval_ = updaters_.Keys.Max();
        }

        public void Update()
        {
            elapsed_ += Time.deltaTime;
            var firedDelta = elapsed_ - lastFiredAt_;

            try {
                foreach (var us in updaters_) {
                    // skip all remaining timers w/ greater interval
                    if (us.Key > firedDelta) break;

                    // skip current timer if it's been fired in current window
                    if (us.Key <= lastFiredAtMax_) continue;

                    // engage the handler IFF certain period has given
                    if (firedDelta > us.Key) {
                        // cache current interval as *max*
                        lastFiredAtMax_ = us.Key;
                        // Log.Debug($"invoking timer for interval [{lastFiredAtMax_} sec]");

                        try {
                            us.Value.Invoke();
                        } catch (Exception e) {
                            Log.Error($"timer failed: {e}");
                        }
                    }
                }

            } finally {
                if (firedDelta > maxInterval_) {
                    lastFiredAtMax_ = 0;
                    lastFiredAt_ = elapsed_;
                }
            }
        }

        private void UpdateCitizen()
        {
            const byte MinHealAmount = (byte)(Game.Citizen.MaxHealth * 0.15f);
            const byte MaxHealAmount = (byte)(Game.Citizen.MaxHealth * 0.75f);
            const float ChanceToMiracleHeal = 0.01f;

            lock (App.Config.AILock) {
                if (App.Config.AI.regenChance.Enabled) {
                    var mgr = Cities.CitizenManager.instance;

                    DataQuery.Citizens((ref Cities::Citizen c, uint id) => {
                        if (!c.Sick) return true;

                        var sampleChance = r_.NextDouble();
                        if (sampleChance > App.Config.AI.regenChance.Value) return true;

                        // Log.Debug($"healing sick: {c}");
                        var info = c.GetCitizenInfo(id);
                        var healAmount = Math.Max(MinHealAmount, (byte)(r_.NextDouble() * MaxHealAmount));

                        // extra heal for healthy citizen
                        if (c.m_health >= Game.Citizen.MaxHealth * 0.5) {
                            healAmount += (byte)(Game.Citizen.MaxHealth * 0.1); // 10% boost
                            healAmount *= 2;
                        }

                        if (sampleChance < ChanceToMiracleHeal) {
                            //Log.Debug($"healing: \"{info.name}\": 100% HP (miracle)");
                            c.m_health = Game.Citizen.MaxHealth;

                        } else {
                            //Log.Debug($"healing: \"{info.name}\": {healAmount} HP ({(float)healAmount / Game.Citizen.MaxHealth:P})");
                            c.m_health = (byte)Math.Min(Game.Citizen.MaxHealth, c.m_health + healAmount);
                        }

                        c.Sick = Cities::Citizen.GetHealthLevel(c.m_health) <= Cities::Citizen.Health.Sick;
                        return true;
                    });

                    // Log.Debug($"sick: {sickCount}");
                }
            }
        }

        public static void UnpatchHostiles()
        {
            //Log.Debug($"UnpatchHostiles()");

            // TODO: unpatch based on config options

            var harmony = Bootstrapper.Instance.Harmony;

            var hostiles = new List<KeyValuePair<MethodBase, MethodInfo>>();
            foreach (var target in harmony.GetPatchedMethods()) {
                //Log.Debug($"found patched method: {target} [{target.Attributes}]");
                Harmony.Patches info = null;

                try {
                    info = harmony.GetPatchInfo(target);

                } catch (InvalidCastException e) {
                    Log.Error($"incompatible Harmony patch detected; this might cause serious conflicts (or visual glitch) in-game\nError: {e}");
                    continue;
                }

                foreach (var p in info.Prefixes) {
                    // TODO: if we have prefix patches, remove others
                    // Log.Warn($"found patch: {p} (ignoring)");
                }

                foreach (var p in info.Postfixes) {
                    if (p.owner != Mod.ModInfo.COMIdentifier) {
                        Log.Warn($"found patch: {p.GetMethod(target)} [by {p.owner}]");
                        Log.Warn($"Unknown Harmony patcher `{p.owner}` found! This will lead to undesired behavior; please report.");
                        hostiles.Add(new KeyValuePair<MethodBase, MethodInfo>(target, p.GetMethod(target)));
                    }

                }
            }

            foreach (var kv in hostiles) {
                Log.Warn($"unpatching: {kv.Value} for {kv.Key}");
                harmony.RemovePatch(kv.Key, kv.Value);
            }

        }

        private System.Random r_;

        private float elapsed_;
        private float lastFiredAt_, lastFiredAtMax_;

        private UpdateSpec updaters_;
        private float maxInterval_;
    }
}

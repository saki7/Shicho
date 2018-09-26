using Shicho.Core;

using Harmony;

using System;
using System.Collections.Generic;
using System.Reflection;


namespace Shicho.Patcher
{
    public static class Util
    {
        public delegate bool PatchPredicate(Harmony.Patch p);

        public static void UnpatchTarget(PatchPredicate pred)
        {
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
                    if (pred.Invoke(p)) {
                        {
                            var msg = $"found target patch: {p.GetMethod(target)} [by {p.owner}]";
                            if (p.owner == Mod.ModInfo.COMIdentifier) {
                                Log.Debug(msg);
                            } else {
                                Log.Warn(msg);
                            }
                        }
                        //Log.Warn($"Unknown Harmony patcher from owner \"{p.owner}\" found! This will lead to undesired behavior; please report.");
                        hostiles.Add(new KeyValuePair<MethodBase, MethodInfo>(target, p.GetMethod(target)));
                    }

                }
            }

            foreach (var kv in hostiles) {
                Log.Warn($"unpatching: {kv.Value} for {kv.Key}");
                harmony.Unpatch(kv.Key, kv.Value);
            }
        }
    }
}

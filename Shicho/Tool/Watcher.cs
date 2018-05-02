using Shicho.Core;

using UnityEngine;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Shicho.Tool
{
    using PatchPair = KeyValuePair<MethodBase, MethodInfo>;

    public class Watcher : MonoBehaviour
    {
        public void Awake()
        {
            elapsed_ = 0;
            lastFiredAt_ = 0;
        }

        public void Update()
        {
            elapsed_ += Time.deltaTime;

            if (elapsed_ - lastFiredAt_ > 1.5f) {
                UnpatchHostiles();
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
                var info = harmony.GetPatchInfo(target);

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

        private float elapsed_;
        private float lastFiredAt_;
    }
}

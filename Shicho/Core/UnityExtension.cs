using UnityEngine;
using System;


namespace Shicho.Core.UnityExtension
{
    public static class UnityExtensionImpl
    {
        public static T AppendChildComponent<T>(
            this MonoBehaviour self,
            string name
        )
            where T: Component
        {
            var obj = new GameObject(name);
            obj.transform.parent = self.gameObject.transform;

            return obj.AddComponent<T>();
        }
    }
}

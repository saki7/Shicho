using ColossalFramework.UI;
using UnityEngine;

using System;

namespace Shicho.GUI
{
    public class Panel
        : UIPanel
    {
        public override void Awake()
        {
            base.Awake();

            // autoLayout = true;
            isInteractive = false;
        }

        public override void Start()
        {
            base.Start();
        }

        public new T AddUIComponent<T>()
            where T : UIComponent
        {
            return Helper.AddDefaultComponent<T>(this);
        }
    }
}

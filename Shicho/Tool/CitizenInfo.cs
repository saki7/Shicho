extern alias Cities;

using Shicho.Core;

using ColossalFramework.UI;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Shicho.Tool
{
    using Shicho.GUI;
    using Shicho.GUI.Extension;

    class CitizenInfo : UIPanel
    {
        const int RowHeight = 18;

        public override void Awake()
        {
            base.Awake();
            ResetCount();

            mgr_ = Cities::CitizenManager.instance;
            updateInterval_ = 0.5f;
            elapsed_ = lastUpdatedAt_ = 0;

            datas_ = new Dictionary<string, DataPrinter>() {
                {"Population",   () => new [] {$"{counts_.total - counts_.dead}"}},
                {"(Dead)",       () => PartialToTotal(counts_.dead)},
                {"(Sick)",       () => PartialToTotal(counts_.sick)},
                {"(Criminal)",   () => PartialToTotal(counts_.criminal)},
                {"(Arrested)",   () => PartialToTotal(counts_.arrested)},
            };
        }

        public override void Start()
        {
            base.Start();


            width = parent.width;
            autoSize = true;
            this.SetAutoLayout(LayoutDirection.Horizontal, Helper.ZeroOffset);

            keyCol_ = AddUIComponent<UIPanel>();
            keyCol_.autoSize = false;
            keyCol_.height = RowHeight;
            UIFont keyFont = null, valueFont = null;

            keyCol_.width = width * 0.32f;
            keyCol_.SetAutoLayout(LayoutDirection.Vertical);

            valueCol_ = AddUIComponent<UIPanel>();
            valueCol_.width = width - keyCol_.width;
            valueCol_.SetAutoLayout(LayoutDirection.Vertical);

            height = keyCol_.height * datas_.Keys.Count;

            foreach (var key in datas_.Keys) {
                var keyLabel = keyCol_.AddUIComponent<UILabel>();
                if (!keyFont) {
                    keyFont = Instantiate(keyLabel.font);
                    keyFont.size = RowHeight - 7;
                }
                keyLabel.textColor = Helper.RGB(180, 180, 180);
                keyLabel.font = keyFont;
                keyLabel.text = key;

                var valuePanel = valueCol_.AddUIComponent<UIPanel>();
                valuePanel.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
                valuePanel.height = keyLabel.height;
                valuePanel.SetAutoLayout(LayoutDirection.Horizontal);

                for (int i = 0; i < 2; ++i) {
                    var valueLabel = valuePanel.AddUIComponent<UILabel>();
                    valueLabel.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;

                    valueLabel.textAlignment = UIHorizontalAlignment.Right;
                    valueLabel.width = valueLabel.parent.width / 2;
                    valueLabel.height = keyLabel.height;

                    if (i >= 1) {
                        valueLabel.padding.left = 4;
                        valueLabel.textColor = Helper.RGB(230, 230, 230);
                    }

                    // Log.Debug($"label '{key}': {valueLabel.position}, {valueLabel.size}");

                    if (!valueFont) {
                        valueFont = Instantiate(valueLabel.font);
                        valueFont.size = 12;
                    }
                    valueLabel.font = valueFont;
                }
            }
        }

        public override void Update()
        {
            base.Update();

            elapsed_ += Time.deltaTime;
            if (elapsed_ - lastUpdatedAt_ > updateInterval_) {
                ResetCount();

                DataQuery.Citizens((ref Citizen c, uint id) => {
                    ++counts_.total;

                    if ((c.m_flags & Citizen.Flags.DummyTraffic) != Citizen.Flags.None) {
                        --counts_.total;
                    }

                    if (c.Dead) {
                        ++counts_.dead;
                        return true;
                    } // no else-if here

                    if (c.Arrested) {
                        ++counts_.arrested;
                    }
                    if (c.Criminal) {
                        ++counts_.criminal;
                    }

                    var healthLevel = Citizen.GetHealthLevel(c.m_health);
                    if (healthLevel == Citizen.Health.Sick) {
                        ++counts_.sick;
                    }
                    return true;
                });

                float maxWidth = 0;
                foreach (var e in valueCol_.components.Select((c, id) => new { c, id })) {
                    var panel = e.c as UIPanel;
                    var values = datas_[(keyCol_.components[e.id] as UILabel).text].Invoke();

                    for (int i = 0; i < values.Length; ++i) {
                        var label = panel.components[i] as UILabel;
                        label.text = values[i];
                        maxWidth = Math.Max(maxWidth, label.width);
                    }
                }

                foreach (var e in valueCol_.components) {
                    var label = e.components[0] as UILabel;
                    label.autoSize = false;
                    label.width = maxWidth;
                }

                lastUpdatedAt_ = elapsed_;
            }
        }

        private void ResetCount()
        {
            counts_ = new CountData();
        }

        private string[] PartialToTotal(uint count)
        {
            return new[] { count.ToString(), $"({(float)count / counts_.total:P1})" };
        }

        private delegate string[] DataPrinter();
        Dictionary<string, DataPrinter> datas_;
        private UIPanel keyCol_, valueCol_;

        float updateInterval_, elapsed_, lastUpdatedAt_;

        public static Cities::CitizenManager mgr_;

        struct CountData
        {
            public uint total, sick, dead, arrested, criminal;
        };
        private CountData counts_;
    }
}

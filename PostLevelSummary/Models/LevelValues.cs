using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Newtonsoft.Json;
using Sirenix.Serialization.Utilities;
using Unity.VisualScripting;

namespace PostLevelSummary.Models
{
    public class LevelValues
    {
        public int TotalItems = 0;
        public float TotalValue = 0f;
        public List<ValuableValue> ValuableValues = new();

        public int ItemsHit = 0;
        public float TotalValueLost = 0f;
        public int ItemsBroken = 0;
        public float ExtractedValue = 0f;
        public int ExtractedItems = 0;
        public bool Extracting = false;

        public LevelValues() {
            run();
        }

        public void Clear()
        {
            PostLevelSummary.Logger.LogDebug("Clearing level values!");

            TotalItems = 0;
            TotalValue = 0f;
            ValuableValues.Clear();

            ItemsHit = 0;
            TotalValueLost = 0f;
            ItemsBroken = 0;
            Extracting = false;
        }

        public async Task AddValuable(ValuableObject val)
        {
            while (!val.dollarValueSet)
            {
                await Task.Delay(50);
            }

            TotalItems += 1;
            TotalValue += val.dollarValueOriginal;
            ValuableValues.Add(new ValuableValue
            {
                Object = val,
                InstanceId = val.GetInstanceID(),
                Value = val.dollarValueOriginal
            });

            PostLevelSummary.Logger.LogDebug($"Created Valuable Object! {val.name} Val: {val.dollarValueOriginal}");
        }

        private async Task run()
        {
            while (true)
            {
                await Task.Delay(100);

                if (PostLevelSummary.InGame)
                {
                    PostLevelSummary.Logger.LogWarning(ValuableValues.Count);
                    List<int> toBeRemoved = new();

                    ValuableValues.ForEach(val =>
                    {
                        if (!val.Object)
                        {
                            ItemsHit += 1;
                            TotalValueLost += val.Value;
                            ItemsBroken += 1;

                            toBeRemoved.Add(val.InstanceId);

                            PostLevelSummary.Logger.LogDebug("An item has been destroyed");
                        } else if (val.Value != val.Object.dollarValueCurrent)
                        {
                            PostLevelSummary.Logger.LogDebug($"{val.Object.name} ({val.Object.GetInstanceID()}) {val.Object.IsDestroyed()} {val.Object.dollarValueCurrent} {val.Object.dollarValueOriginal} {val.Value}");
                            var diff = val.Value - val.Object.dollarValueCurrent;

                            ItemsHit += 1;
                            TotalValueLost += diff;
                            val.Value -= diff;
                            UI.Update();
                        }
                    });

                    if (toBeRemoved.Count > 0)
                    {
                        ValuableValues = ValuableValues.FindAll(v => !toBeRemoved.Contains(v.InstanceId));
                        UI.Update();
                    }
                }
            }
        }
    }
}

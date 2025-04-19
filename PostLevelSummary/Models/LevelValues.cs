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
        public List<int> DollarHaulList = new();

        public int ItemsHit = 0;
        public float TotalValueLost = 0f;
        public int ItemsBroken = 0;
        public float ExtractedValue = 0f;
        public int ExtractedItems = 0;

        public bool Extracting = false;
        public int ExtractedChecksLeft = 0;

        private bool _run = false;
        private readonly object _locker = new();
        private Task? running;

        public void Clear()
        {
            PostLevelSummary.Logger.LogDebug("Clearing level values!");

            TotalItems = 0;
            TotalValue = 0f;
            ValuableValues.Clear();
            DollarHaulList.Clear();

            ItemsHit = 0;
            TotalValueLost = 0f;
            ItemsBroken = 0;
            ExtractedValue = 0f;
            ExtractedItems = 0;

            Extracting = false;
            ExtractedChecksLeft = 0;
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
            runTask();
        }

        private void runTask()
        {
            lock (_locker)
            {
                if (running != null && 
                    running.Status != TaskStatus.Canceled &&
                    running.Status != TaskStatus.Faulted &&
                    running.Status != TaskStatus.RanToCompletion
                )
                {
                    return;
                }

                if (running != null)
                {
                    PostLevelSummary.Logger.LogWarning($"Lost track of our runner / checker: {running} ({running.Status})");
                }

                _run = true;
                running = run();
            }
        }

        public void StopTask()
        {
            _run = false;
        }

        private async Task run()
        {
            while (_run)
            {
                await Task.Delay(100);

                if (PostLevelSummary.InGame && !PostLevelSummary.InShop && !PostLevelSummary.InLobby)
                {
                    PostLevelSummary.Logger.LogWarning(ValuableValues.Count);
                    List<int> toBeRemoved = new();

                    ValuableValues.ForEach(val =>
                    {
                        if (!val.Object)
                        {
                            if (DollarHaulList.Contains(val.InstanceId) && (ExtractedChecksLeft > 0 || Extracting))
                            {
                                ExtractedValue += val.Value;
                                ExtractedItems += 1;

                                if (!Extracting)
                                    ExtractedChecksLeft -= 1;
                            }
                            else
                            {
                                ItemsHit += 1;
                                TotalValueLost += val.Value;
                                ItemsBroken += 1;
                            }

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

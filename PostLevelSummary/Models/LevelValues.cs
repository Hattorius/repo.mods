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
        public List<ValuableObject> Valuables = new();
        public List<ValuableValue> ValuableValues = new();
        public Dictionary<int, ValuableObject> ValuableRegistry = new();

        public int ItemsHit = 0;
        public float TotalValueLost = 0f;
        public int ItemsBroken = 0;
        public float ExtractedValue = 0f;
        public int ExtractedItems = 0;

        public int ItemCount { get {  return Valuables.Count; } }

        public void Clear()
        {
            PostLevelSummary.Logger.LogDebug("Clearing level values!");

            TotalItems = 0;
            TotalValue = 0f;
            Valuables.Clear();
            ValuableValues.Clear();
            ValuableRegistry.Clear();

            ItemsHit = 0;
            TotalValueLost = 0f;
            ItemsBroken = 0;
        }

        public async Task AddValuable(ValuableObject val)
        {
            while (!val.dollarValueSet)
            {
                await Task.Delay(50);
            }

            TotalItems += 1;
            TotalValue += val.dollarValueOriginal;
            Valuables.Add(val);
            ValuableValues.Add(new ValuableValue
            {
                InstanceId = val.GetInstanceID(),
                Value = val.dollarValueOriginal
            });
            ValuableRegistry[val.GetInstanceID()] = val;

            PostLevelSummary.Logger.LogDebug($"Created Valuable Object! {val.name} Val: {val.dollarValueOriginal}");
        }

        public void CheckValueChange(ValuableObject val)
        {

            ValuableValue vv = ValuableValues.Find(v => v.InstanceId == val.GetInstanceID());

            if (vv.Value != val.dollarValueCurrent)
            {
                var lostValue = vv.Value - val.dollarValueCurrent;
                PostLevelSummary.Logger.LogDebug($"{val.name} lost {lostValue} value!");

                ItemsHit += 1;
                TotalValueLost += lostValue;
                vv.Value = val.dollarValueCurrent;

                if (val.dollarValueCurrent == 0f)
                {
                    ValuableRegistry.Keys.Where(k => !ValuableValues.Select(v => v.InstanceId).Contains(k)).ForEach(k => ValuableRegistry.Remove(k));
                    ValuableValues.RemoveAll(v => ValuableRegistry[v.InstanceId].IsDestroyed());
                    ItemsBroken += 1;

                    PostLevelSummary.Logger.LogDebug($"1 item destroyed!");
                }
            }

            if (val.dollarValueCurrent == 0f)
            {
                ItemBroken();
            }
        }

        public async Task ItemBroken()
        {
            var destroyed = ValuableRegistry.Keys.Where(k => !ValuableValues.Select(v => v.InstanceId).Contains(k)).Select(v => ValuableRegistry[v]);
            var totalDestroyed = destroyed.Count();
            var totalValueDestroyed = destroyed.Select(v => v.dollarValueCurrent).Sum();

            ItemsHit += totalDestroyed;
            TotalValueLost += totalValueDestroyed;
            ItemsBroken += totalDestroyed;

            PostLevelSummary.Logger.LogDebug($"{totalDestroyed} item(s) destroyed! Lost ${totalValueDestroyed} value");

            ValuableRegistry.Keys.Where(k => !ValuableValues.Select(v => v.InstanceId).Contains(k)).ForEach(k => ValuableRegistry.Remove(k));
            ValuableValues.RemoveAll(v => ValuableRegistry[v.InstanceId].IsDestroyed());
        }

        public void Extracted()
        {
            if (Valuables.Any(v => v.IsDestroyed()))
            {
                var existing = Valuables.FindAll(v => v.GetInstanceID() != 0).Select(v => v.GetInstanceID());
                var extracted = ValuableValues.FindAll(v => !existing.Any(id => id == v.InstanceId));

                ExtractedValue += extracted.Select(v => v.Value).Sum();
                ExtractedItems += extracted.Count;

                Valuables.RemoveAll(v => v.GetInstanceID() == 0);
                ValuableValues.RemoveAll(v => !existing.Any(id => id == v.InstanceId));
            }
        }
    }
}

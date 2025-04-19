using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Unity.VisualScripting;

namespace PostLevelSummary.Patches
{
    [HarmonyPatch(typeof(ValuableObject))]
    class ValuableObjectPatches
    {
        [HarmonyPatch("DollarValueSet")]
        [HarmonyPostfix]
        static void DollarValueSetPostfix(ValuableObject __instance)
        {
            if (__instance.name.ToLower().Contains("surplus"))
                return;

            PostLevelSummary.AddValuable(__instance);
        }

        [HarmonyPatch("AddToDollarHaulList")]
        [HarmonyPostfix]
        static void AddToDollarHaulListPostix(ValuableObject __instance)
        {
            if (__instance.name.ToLower().Contains("surplus"))
                return;

            PostLevelSummary.Logger.LogDebug($"Added to dollar haul list: {__instance.name}");
            AddToDollarHaulList(__instance.GetInstanceID());
        }

        [HarmonyPatch("AddToDollarHaulListRPC")]
        [HarmonyPostfix]
        static void AddToDollarHaulListRPCPostix(ValuableObject __instance)
        {
            if (__instance.name.ToLower().Contains("surplus"))
                return;

            PostLevelSummary.Logger.LogDebug($"[RPC] Added to dollar haul list: {__instance.name}");
            AddToDollarHaulList(__instance.GetInstanceID());
        }

        [HarmonyPatch("RemoveFromDollarHaulList")]
        [HarmonyPostfix]
        static void RemoveFromDollarHaulListPostix(ValuableObject __instance)
        {
            if (__instance.name.ToLower().Contains("surplus"))
                return;

            PostLevelSummary.Logger.LogDebug($"Removed from dollar haul list: {__instance.name}");
            RemoveFromDollarHaulList(__instance.GetInstanceID());
        }

        [HarmonyPatch("RemoveFromDollarHaulListRPC")]
        [HarmonyPostfix]
        static void RemoveFromDollarHaulListRPCPostix(ValuableObject __instance)
        {
            if (__instance.name.ToLower().Contains("surplus"))
                return;

            PostLevelSummary.Logger.LogDebug($"[RPC] Removed from dollar haul list: {__instance.name}");
            RemoveFromDollarHaulList(__instance.GetInstanceID());
        }

        private static void AddToDollarHaulList(int id)
        {
            if (PostLevelSummary.Level.DollarHaulList.Contains(id)) return;
            PostLevelSummary.Level.DollarHaulList.Add(id);
        }

        private static void RemoveFromDollarHaulList(int id)
        {
            PostLevelSummary.Level.DollarHaulList.Remove(id);
        }
    }
}

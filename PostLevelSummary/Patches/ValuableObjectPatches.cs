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
        }

        [HarmonyPatch("AddToDollarHaulListRPC")]
        [HarmonyPostfix]
        static void AddToDollarHaulListRPCPostix(ValuableObject __instance)
        {
            if (__instance.name.ToLower().Contains("surplus"))
                return;

            PostLevelSummary.Logger.LogDebug($"Added to dollar haul list: {__instance.name}");
        }
    }
}

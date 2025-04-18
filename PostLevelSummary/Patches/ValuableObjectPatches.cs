using System;
using System.Collections.Generic;
using System.Text;
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
    }
}

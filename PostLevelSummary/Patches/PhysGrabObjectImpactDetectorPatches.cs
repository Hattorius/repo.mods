using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine.UIElements.UIR;

namespace PostLevelSummary.Patches
{
    [HarmonyPatch(typeof(PhysGrabObjectImpactDetector))]
    class PhysGrabObjectImpactDetectorPatches
    {
        [HarmonyPatch("BreakRPC")]
        [HarmonyPostfix]
        static void BreakPostfix(PhysGrabObjectImpactDetector? __instance)
        {
            ValuableObject? vo = __instance?.GetComponent<ValuableObject>();
            if (vo == null) return;

            PostLevelSummary.Logger.LogInfo("Break RPC");

            PostLevelSummary.Level.CheckValueChange(vo);
        }

        [HarmonyPatch("DestroyObjectRPC")]
        [HarmonyPostfix]
        static void DestroyObjectRPCPostfix()
        {
            PostLevelSummary.Logger.LogInfo("DestroyObjectRPC");
            PostLevelSummary.Level.ItemBroken();
        }
    }
}

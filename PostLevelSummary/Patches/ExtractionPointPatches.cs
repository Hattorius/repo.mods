using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace PostLevelSummary.Patches
{
    [HarmonyPatch(typeof(ExtractionPoint))]
    class ExtractionPointPatches
    {
        [HarmonyPatch("StateComplete")]
        [HarmonyPostfix]
        public static void StateCompletePostfix()
        {
            if (PostLevelSummary.InGame)
            {
                PostLevelSummary.Level.Extracted();
            }
        }

        [HarmonyPatch("StateSetRPC")]
        [HarmonyPrefix]
        public static void StateSetRPCPrefix(ExtractionPoint.State state)
        {
            if (state.Equals(ExtractionPoint.State.Complete))
            {
                if (PostLevelSummary.InGame)
                {
                    PostLevelSummary.Level.Extracted();
                }
            }
        }
    }
}

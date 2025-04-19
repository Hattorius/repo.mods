using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using PostLevelSummary.Models;

namespace PostLevelSummary.Patches
{
    [HarmonyPatch(typeof(ExtractionPoint))]
    class ExtractionPointPatches
    {
        [HarmonyPatch("StateSet")]
        [HarmonyPrefix]
        public static void StateSetPrefix(ExtractionPoint.State newState)
        {
            if (PostLevelSummary.InGame)
            {
                StateChangedPrefix(newState);
            }
        }

        [HarmonyPatch("StateSetRPC")]
        [HarmonyPrefix]
        public static void StateSetRPCPrefix(ExtractionPoint.State state)
        {
            if (PostLevelSummary.InGame)
            {
                StateChangedPrefix(state);
            }
        }

        [HarmonyPatch("StateSet")]
        [HarmonyPostfix]
        public static void StateSetPostfix(ExtractionPoint.State newState)
        {
            if (PostLevelSummary.InGame)
            {
                StateChangedPostfix(newState);
            }
        }

        [HarmonyPatch("StateSetRPC")]
        [HarmonyPostfix]
        public static void StateSetRPCPostfix(ExtractionPoint.State state)
        {
            if (PostLevelSummary.InGame)
            {
                StateChangedPostfix(state);
            }
        }

        private static void StateChangedPrefix(ExtractionPoint.State state)
        {
            PostLevelSummary.Logger.LogDebug($"New state: {state}");

            if (state == ExtractionPoint.State.Extracting)
            {
                PostLevelSummary.Level.Extracting = true;
                PostLevelSummary.Level.ExtractedChecksLeft = 20; // 20 * 100 ms = 2 seconds
            }
        }

        private static void StateChangedPostfix(ExtractionPoint.State state)
        {
            PostLevelSummary.Logger.LogDebug($"New state: {state}");

            if (state == ExtractionPoint.State.Complete)
            {
                PostLevelSummary.Level.Extracting = false;
                PostLevelSummary.Level.ExtractedChecksLeft = 20; // 20 * 100 ms = 2 seconds
            }
        }
    }
}

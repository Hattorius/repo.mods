using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;

namespace PostLevelSummary.Patches
{
    [HarmonyPatch(typeof(RunManager))]
    class SceneManagerPatches
    {
        [HarmonyPatch("ChangeLevel")]
        [HarmonyPrefix]
        public static void ChangeLevelPrefix()
        {
            PostLevelSummary.Level.StopTask();
        }

        [HarmonyPatch("UpdateLevel")]
        [HarmonyPrefix]
        public static void UpdateLevelPrefix()
        {
            PostLevelSummary.Level.StopTask();
        }
    }
}

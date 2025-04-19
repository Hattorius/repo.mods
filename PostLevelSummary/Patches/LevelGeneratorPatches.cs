using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace PostLevelSummary.Patches
{
    [HarmonyPatch(typeof(LevelGenerator))]
    class LevelGeneratorPatches
    {
        public static LevelGenerator? Instance;

        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPostfix]
        public static void CaptureInstance(LevelGenerator __instance)
        {
            Instance = __instance;
            PostLevelSummary.Logger.LogDebug("Captured LevelGenerator instance.");
        }

        [HarmonyPatch("GenerateDone")]
        [HarmonyPrefix]
        public static void GenerateDonePrefix()
        {
            if (Instance == null) return;
            PostLevelSummary.Logger.LogDebug($"Done generating new level: {Instance.Level.name} ({Instance.Level.NarrativeName})");

            if (Instance.Level.name.ToLower().Contains("menu"))
            {
                PostLevelSummary.InMenu = true;
            }
            else if (Instance.Level.name.ToLower().Contains("lobby"))
            {
                PostLevelSummary.InLobby = true;
            }
            else if (Instance.Level.name.ToLower().Contains("shop"))
            {
                PostLevelSummary.InShop = true;
            }
            else if (Instance.Level.HasEnemies && !PostLevelSummary.InGame)
            {
                PostLevelSummary.InGame = true;
            }

            if (!Instance.Level.name.ToLower().Contains("menu"))
            {
                PostLevelSummary.InMenu = false;
            }

            if (!Instance.Level.name.ToLower().Contains("lobby"))
            {
                PostLevelSummary.InLobby = false;
            }

            if (!Instance.Level.name.ToLower().Contains("shop"))
            {
                PostLevelSummary.InShop = false;
            }

            if (!Instance.Level.HasEnemies)
            {
                PostLevelSummary.InGame = false;
            }
        }
    }
}

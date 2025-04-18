using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using PostLevelSummary.Models;
using PostLevelSummary.Patches;
using System.Threading.Tasks;

namespace PostLevelSummary;

[BepInPlugin("Hattorius.PostLevelSummary", "PostLevelSummary", "1.0")]
public class PostLevelSummary : BaseUnityPlugin
{
    internal static PostLevelSummary Instance { get; private set; } = null!;
    public static new ManualLogSource Logger;
    private readonly Harmony harmony = new("Hattorius.PostLevelSummary");

    public static LevelValues Level = new();

    private static bool _ingame = false;
    public static bool InGame {
        get => _ingame;
        set
        {
            if (value)
            {
                ResetValues();
            }

            _ingame = value;
        }
    }
    private static bool _inshop = false;
    public static bool InShop {
        get => _inshop;
        set {
            Logger.LogDebug($"In shop: {value}");
            if (value)
            {
                UI.Update();
                TextInstance.SetActive(true);
            }
            else
            {
                TextInstance.SetActive(true);
            }

            _inshop = value;
        }
    }

    public static bool InMenu = false;
    public static GameObject TextInstance;
    public static TextMeshProUGUI ValueText;

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;
        
        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        harmony.PatchAll(typeof(ValuableObjectPatches));
        harmony.PatchAll(typeof(LevelGeneratorPatches));
        harmony.PatchAll(typeof(PhysGrabObjectImpactDetectorPatches));
        harmony.PatchAll(typeof(RoundDirectorPatches));
        harmony.PatchAll(typeof(ExtractionPointPatches));

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    internal void Unpatch()
    {
        harmony?.UnpatchSelf();
    }

    public static void ResetValues()
    {
        Level.Clear();
    }

    public static void AddValuable(ValuableObject val)
    {
        Level.AddValuable(val);
    }
}
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

[BepInPlugin("Hattorius.PostLevelSummary", "PostLevelSummary", "2.0.0")]
public class PostLevelSummary : BaseUnityPlugin
{
    internal static PostLevelSummary Instance { get; private set; } = null!;
    public static new ManualLogSource Logger;
    private readonly Harmony harmony = new("Hattorius.PostLevelSummary");

    public static LevelValues Level;

    private static bool _inshop = false;
    public static bool InShop {
        get => _inshop;
        set {
            Logger.LogDebug($"In shop: {value}");
            if (value)
            {
                UI.Update();
            }

            TextInstance.SetActive(value || _inlobby);
            _inshop = value;
        }
    }
    private static bool _inlobby = false;
    public static bool InLobby
    {
        get => _inlobby;
        set
        {
            Logger.LogDebug($"In lobby: {value}");
            if (value)
            {
                ResetValues();
            }

            _inlobby = value;
        }
    }
    private static bool _inmenu = false;
    public static bool InMenu
    {
        get => _inmenu;
        set
        {
            Logger.LogDebug($"In menu: {value}");
            if (value)
            {
                ResetValues();
            }

            _inmenu = value;
        }
    }

    public static bool InGame = false;
    public static GameObject TextInstance;
    public static TextMeshProUGUI ValueText;

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;
        Level = new();

        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        harmony.PatchAll(typeof(SceneManagerPatches));
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
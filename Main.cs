using System.Collections.Generic;
using CompleteDarkness;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.Map;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(CompleteDarkness.Main), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace CompleteDarkness;

[HarmonyPatch]
public class Main : BloonsTD6Mod
{
    internal static MelonLogger.Instance Logger;
    private static List<string> Maps = new();
    public override void OnMainMenu()
    {
        Maps = GameData._instance.mapSet.maps.Where(x => Game.instance.GetBtd6Player().IsMapUnlocked(x.id))
            .Select(x => x.id).ToList();
    }

    public override void OnInitialize()
    {
        Logger = LoggerInstance;
    }
    
    [HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.Initialise))] 
    [HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.Show))]
    [HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.SelectionChanged))]
    //[HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.UpgradeTower))]
    [HarmonyPostfix]
    static void TowerSelectionMenu_Stuff(TowerSelectionMenu __instance)
    {
        __instance.scalar.gameObject.SetActive(false);
        
        /*__instance.themeManager?.currentTheme?.transform.FindChild("MonkeyImage").gameObject.SetActive(false);
        foreach (var graphic in __instance.gameObject.GetComponentsInChildren<Graphic>().Where(graphic => graphic is not null))
        {
            graphic.color = new Color(0, 0, 0, 1);
        }
        foreach (var nkTextMeshProUGUI in __instance.gameObject.GetComponentsInChildren<NK_TextMeshProUGUI>().Where(nkTextMeshProUGUI => nkTextMeshProUGUI is not null))
        {
            nkTextMeshProUGUI.enabled = false;
        }
        foreach (var image in __instance.gameObject.GetComponentsInChildren<Image>().Where(image => image is not null))
        {
            image.enabled = false;
        }*/

    }

    [HarmonyPatch(typeof(UnityToSimulation), nameof(UnityToSimulation.InitMap))]
    [HarmonyPrefix]
    internal static void UnityToSimulation_InitMap(UnityToSimulation __instance, ref MapModel map)
    {
        SceneManager.GetSceneByName(map.mapName).GetRootGameObjects().ForEach(x => x.SetActive(false));
    }

    [HarmonyPatch(typeof(Factory.__c__DisplayClass21_0), nameof(Factory.__c__DisplayClass21_0._CreateAsync_b__0))]
    [HarmonyPrefix]
    private static bool Factory_CreateAsync_Prefix(Factory.__c__DisplayClass21_0 __instance,
        ref UnityDisplayNode prototype)
    {
        return false;
    }
    
    [HarmonyPatch(typeof(MapLoader), nameof(MapLoader.LoadScene))]
    [HarmonyPrefix]
    private static void MapLoader_LoadScene(ref MapLoader __instance)
    {
        __instance.currentMapName = Maps[Random.Range(0, Maps.Count)];
    }
}
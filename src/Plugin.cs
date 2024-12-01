using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MyBox;
using UnityEngine.Localization;

namespace SS.src;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll(typeof(Patches));

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }


    class Patches
    {

        [HarmonyPatch(typeof(ProductSO), nameof(ProductSO.ComplexName))]
        [HarmonyPostfix]
        static void OnProductSOComplexName(ref ProductSO __instance, ref string __result)
        {

            var displayType = Singleton<IDManager>.Instance.ProductSO(__instance.ID).ProductDisplayType;

            if (displayType == DisplayType.SHELF)
            {
                return;
            }

            var localizationEntry = displayType.LocalizationEntry();

            __result = string.Format("{0} [{1}]",
                __result,
                new LocalizedString(localizationEntry.TableCollection, localizationEntry.TableEntry).GetLocalizedString());

        }

    }
}

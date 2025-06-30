using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModTemplate.Patches
{
    internal class ExampleSortByUraPatch
    {
        [HarmonyPatch(typeof(SongFilterSetting))]
        [HarmonyPatch(nameof(SongFilterSetting.SelectNextItem))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool SongFilterSetting_SelectNextItem_Prefix(SongFilterSetting __instance)
        {
            __instance.itemValues[__instance.selectedItem] = (__instance.itemValues[__instance.selectedItem] + 1) % __instance.itemMax[__instance.selectedItem];
            __instance.isRequestedViewUpdate = true;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);

            return false;
        }

        [HarmonyPatch(typeof(SongFilterSetting))]
        [HarmonyPatch(nameof(SongFilterSetting.SelectPrevItem))]
        [HarmonyPrefix]
        private static bool SongFilterSetting_SelectPrevItem_Prefix(SongFilterSetting __instance)
        {
            __instance.itemValues[__instance.selectedItem] = (__instance.itemValues[__instance.selectedItem] - 1 + __instance.itemMax[__instance.selectedItem]) % __instance.itemMax[__instance.selectedItem];
            __instance.isRequestedViewUpdate = true;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);

            return false;
        }
    }
}

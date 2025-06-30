using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdjustUraFlipSpeed.Patches
{
    class AdjustUraFlipSpeedPatch
    {
        [HarmonyPatch(typeof(CourseSelect))]
        [HarmonyPatch(nameof(CourseSelect.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool CourseSelect_Start_Prefix(CourseSelect __instance)
        {
            var multiplier = Plugin.Instance.ConfigAdjustMultiplier.Value;
            if (multiplier == 0)
            {
                multiplier = 0.01f;
                ModLogger.Log("AdjustMultiplier cannot be 0. Setting to 0.01");
            }
            __instance.DiffCourseRootAnim.speed = multiplier;
            return true;
        }
    }
}

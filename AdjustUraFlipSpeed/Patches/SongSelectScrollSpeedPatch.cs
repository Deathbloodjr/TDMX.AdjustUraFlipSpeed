using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdjustUraFlipSpeed.Patches
{
    internal class SongSelectScrollSpeedPatch
    {
        /// 
        /// This file adjusts how quickly you can scroll through the song select menu
        /// To do that, we adjust the speed of the animation for the kanban's movement
        /// 
        /// The (probably) best way to do this, is detect when we are within the PlayKanbanMoveAnim function
        /// This function assigns the kanban movement speed, so we'll patch the speed setter function
        /// and adjust the speed to whatever we want
        /// 


        static bool changeSpeed = false;

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.PlayKanbanMoveAnim))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static void SongSelectManager_PlayKanbanMoveAnim_Prefix(SongSelectManager __instance)
        {
            ModLogger.Log("SongSelectManager_PlayKanbanMoveAnim_Prefix");
            changeSpeed = true;
        }


        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.PlayKanbanMoveAnim))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        private static void SongSelectManager_PlayKanbanMoveAnim_Postfix(SongSelectManager __instance)
        {
            ModLogger.Log("SongSelectManager_PlayKanbanMoveAnim_Postfix");
            changeSpeed = false;
        }

        static bool skipNext = false;
        [HarmonyPatch(typeof(Animator))]
        [HarmonyPatch(nameof(Animator.speed))]
        [HarmonyPatch(MethodType.Setter)]
        [HarmonyPostfix]
        private static void Animator_speed_setter_Postfix(Animator __instance)
        {
            ModLogger.Log("Animator_speed_setter_Postfix");
            if (changeSpeed && !skipNext)
            {
                skipNext = true;
                var newSpeed = 1f;
                if (__instance.speed == 1)
                {
                    newSpeed = Plugin.Instance.ConfigSongSelectScrollNormalSpeed.Value;
                }
                else if (__instance.speed == 2)
                {
                    newSpeed = Plugin.Instance.ConfigSongSelectScrollFastSpeed.Value;
                }

                if (newSpeed == 0)
                {
                    newSpeed = 0.01f;
                    ModLogger.Log("SongSelectScrollSpeed cannot be 0. Setting to 0.01");
                }

                __instance.speed = newSpeed;
            }
            skipNext = false;
        }
    }
}

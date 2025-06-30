using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModTemplate.Patches
{
    internal class ExampleSingleHitBigNotesPatch
    {
		[HarmonyPatch(typeof(EnsoInput))]
		[HarmonyPatch(nameof(EnsoInput.GetLastInputForCore))]
		[HarmonyPatch(MethodType.Normal)]
		[HarmonyPrefix]
		public static bool EnsoInput_GetLastInputForCore_Prefix(EnsoInput __instance, ref TaikoCoreTypes.UserInputType __result, int player)
		{
			TaikoCoreTypes.UserInputType NewGetLastInputForCore()
			{
				EnsoInput.EnsoInputFlag flag = __instance.GetLastInput(player);
				if (flag == EnsoInput.EnsoInputFlag.DonR ||
					flag == EnsoInput.EnsoInputFlag.DonL ||
					flag == EnsoInput.EnsoInputFlag.DaiDon)
				{
					flag = EnsoInput.EnsoInputFlag.DaiDon;
				}
				else if (flag == EnsoInput.EnsoInputFlag.KatsuR ||
						 flag == EnsoInput.EnsoInputFlag.KatsuL ||
						 flag == EnsoInput.EnsoInputFlag.DaiKatsu)
				{
					flag = EnsoInput.EnsoInputFlag.DaiKatsu;
				}
				if (__instance.ensoParam.EnsoEndType == EnsoPlayingParameter.EnsoEndTypes.OptionPerfect || __instance.ensoParam.EnsoEndType == EnsoPlayingParameter.EnsoEndTypes.OptionTraining)
				{
					flag = EnsoInput.EnsoInputFlag.None;
				}
				return __instance.ToUserInputType(flag);
			}

			__result = NewGetLastInputForCore();

			return false;
		}
	}
}

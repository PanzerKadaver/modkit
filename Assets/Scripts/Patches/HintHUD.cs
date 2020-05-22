using HarmonyLib;

namespace Assets.Scripts.Patches
{
	[HarmonyPatch(typeof(HintHUD), "ParseValue")]
	public static class HintHUD_ParseValue
	{
		public static bool Prefix(HintHUD __instance, string t, ref string __result)
		{
			Character character = CharacterSelectorHUD.GetCurrent().Character;

			__result = t.Replace("$SPEC_LVLP$", Utils.GetTalentPoints(character, Game.World.GetGameDifficult()).ToString());
			
			return false;
		}
	}
}

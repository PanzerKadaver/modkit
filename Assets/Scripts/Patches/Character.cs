using HarmonyLib;

namespace Assets.Scripts.Patches
{
	[HarmonyPatch(typeof(Character), "Amnesia")]
	public static class Character_Amnesia
	{
		public static void Postfix(Character __instance)
		{
			CharacterProtoStats stats = __instance.CharProto.Stats;
			stats.SpecLevel = 0;
			stats.FreeSpecPoints = (stats.Level - 1) * Utils.GetTalentPoints(__instance, Game.World.GetGameDifficult());
		}
	}
}

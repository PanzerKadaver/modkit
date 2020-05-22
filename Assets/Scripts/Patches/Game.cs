using HarmonyLib;

namespace Assets.Scripts.Patches
{
	[HarmonyPatch(typeof(Game), "NextXPLevel")]
	public static class Game_NextXPLevel
	{
		public static bool Prefix(Game __instance)
		{
			if (__instance.HasLevelUp())
			{
				__instance.Player.CharacterComponent.Character.CharProto.Stats.Level++;
				__instance.Player.CharacterComponent.Character.CharProto.Stats.FreeSkillPoints += __instance.Player.CharacterComponent.Character.Stats.SkillRate;
				__instance.Player.CharacterComponent.Character.CharProto.Stats.FreeSpecPoints += Utils.GetTalentPoints(__instance.Player.CharacterComponent.Character, Game.World.GetGameDifficult());
				if (Game.World.GetGameDifficult() == Game.GameDifficult.Expert && __instance.Player.CharacterComponent.Character.CharProto.Stats.Level >= 30)
				{
					Game.World.Services.ApplyAchievement("EXPERT_1");
				}
			}

			return false;
		}
	}

	[HarmonyPatch(typeof(Game), "CalcUpLevelXP")]
	public static class Game_CalcUpLevelXP
	{
		public static bool Prefix(Game __instance, int level, bool childprodigy, ref int __result)
		{
			int num = level + 1;
			__result = num * (num - 1) / 2 * (childprodigy ? 625 : 500);

			return false;
		}
	}
}

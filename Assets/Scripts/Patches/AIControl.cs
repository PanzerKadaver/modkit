using HarmonyLib;

namespace Assets.Scripts.Patches
{
	[HarmonyPatch(typeof(AIControl), "CheckNewLevel")]
	public static class AIControl_CheckNewLevel
	{
		public static bool Prefix(AIControl __instance)
		{
			Character character = __instance.CharacterComponent.Character;
			int num = Game.World.CalcUpLevelXP(character.CharProto.Stats.Level, childprodigy: false);
			while (character.CharProto.Stats.ExperiencePoints >= num)
			{
				character.CharProto.Stats.Level++;
				character.CharProto.Stats.FreeSkillPoints += character.Stats.SkillRate;
				character.CharProto.Stats.FreeSpecPoints += Utils.GetTalentPoints(character, Game.World.GetGameDifficult());
				if (__instance.CharacterComponent.IsMale())
				{
					Game.World.Msg(notification: false, "notification.teammate.newlevel", __instance.CharacterComponent.GetShortName(), character.CharProto.Stats.FreeSkillPoints);
				}
				else
				{
					Game.World.Msg(notification: false, "notification.teammate.newlevel_fm", __instance.CharacterComponent.GetShortName(), character.CharProto.Stats.FreeSkillPoints);
				}
				num = Game.World.CalcUpLevelXP(character.CharProto.Stats.Level, childprodigy: false);
			}
			return false;
		}
	}
}

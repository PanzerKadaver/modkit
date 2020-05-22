using HarmonyLib;

namespace Assets.Scripts.Patches
{
	[HarmonyPatch(typeof(CharacterAddSpecPointsNode), "Start")]
	public static class CharacterAddSpecPointsNode_Start
	{
		public static bool Prefix(CharacterAddSpecPointsNode __instance)
		{
			CharacterComponent component = __instance.GetObject().GetComponent<CharacterComponent>();
			component.Character.CharProto.Stats.FreeSpecPoints += __instance.value * Utils.GetTalentPoints(component.Character, Game.World.GetGameDifficult());

			return false;
		}
	}
}

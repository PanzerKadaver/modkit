using System;
using System.Reflection;

namespace Assets.Scripts
{
	public static class Utils
	{
		public static string ModName => Assembly.GetExecutingAssembly().GetName().Name;
		public static string DT => DateTime.Now.ToString("G");
		
		public static void Log(object msg)
		{
			UnityEngine.Debug.Log($"[{DT}]" + $"[{ModName}] : " + msg);
		}

		public static void LogWarning(object msg)
		{
			UnityEngine.Debug.LogWarning($"[{DT}]" + $"[{ModName}] : " + msg);
		}

		public static void LogError(object msg)
		{
			UnityEngine.Debug.LogError($"[{DT}]" + $"[{ModName}] : " + msg);
		}

		public static int GetTalentPoints(Character character, Game.GameDifficult difficulty)
		{
			int tp = 3;

			if (character.IsTeammate)
				if (character.CharProto.name.ToLower() == "Wolfter".ToLower())
					return 4;
				else
					return 3;

			switch (difficulty)
			{
				case Game.GameDifficult.Casual:
					tp += 1;
					break;
				case Game.GameDifficult.Survive:
					tp -= 1;
					break;
				default:
					break;
			}

			return tp - (character.CharProto.Stats.HasPerk(CharacterStats.Perk.ChildProdigy) ? (1) : (0));
		}
	}
}

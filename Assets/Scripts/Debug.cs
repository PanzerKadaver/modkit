using System;
using System.Reflection;

namespace Assets.Scripts
{
	public static class Debug
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
	}
}

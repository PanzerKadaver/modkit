using HarmonyLib;

namespace Assets.Scripts.Patches
{
	[HarmonyPatch(typeof(StatInfoHUD), "Show")]
	public static class StatInfoHUD_Show
	{
		public static bool Prefix(StatInfoHUD __instance)
		{
			__instance.gameObject.transform.SetAsLastSibling();

			return true; // fallback to original method
		}
	}
}

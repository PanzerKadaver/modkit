using HarmonyLib;

namespace Assets.Scripts.Patches
{
	[HarmonyPatch(typeof(PlayerHUD), "Start")]
	public static class PlayerHUD_Start
	{
		private static UnityEngine.GameObject _currentInventory = null;
		private static UnityEngine.GameObject _inventoryPrefab = null;

		public static bool Prefix(PlayerHUD __instance)
		{
			if (_inventoryPrefab == null)
			{
				Debug.Log("Retrieve injected Inventory.prefab");
				_inventoryPrefab = ResourceManager.Load<UnityEngine.GameObject>("ui/prefabs/inventory", ".prefab");
			}
			if (_currentInventory != null)
			{
				Debug.Log("Deleting old injected Inventory");
				UnityEngine.GameObject.DestroyImmediate(_currentInventory);
				_currentInventory = null;
			}

			int index = __instance.gameObject.transform.Find("UI").Find("Inventory").GetSiblingIndex();
			Debug.Log($"Inventory index : {index}");
			UnityEngine.GameObject.DestroyImmediate(__instance.gameObject.transform.Find("UI").Find("Inventory").gameObject);
			_currentInventory = UnityEngine.GameObject.Instantiate(_inventoryPrefab, __instance.gameObject.transform.Find("UI"), false);
			_currentInventory.name = "Inventory";
			_currentInventory.transform.SetSiblingIndex(index);

			__instance.Inventory = _currentInventory.GetComponent<InventoryHUD>();

			return true;
		}
	}
}

using UnityEngine;
using Harmony;
using JSon;
using AutoMapper;

namespace Assets.Scripts.Patches_Weapon
{
	[HarmonyPriority(Priority.Normal)]
	[HarmonyPatch(typeof(Weapon), "Deserialize")]
	public static class Weapon_Deserialize
	{
		public static void Postfix(Weapon __instance, JNode node)
		{
			if (__instance.Prototype is Extended.WeaponProtoExtended)
			{
				Debug.Log($"[{nameof(Weapon_Deserialize)}|{nameof(Postfix)}] : {__instance.Prototype.name} is already prototyped as WeaponProtoExtended");
			}
			else if (__instance.Prototype.GetMode(ShotMode.Reload).Dispersion == -1)
			{
				Debug.Log($"[{nameof(Weapon_Deserialize)}|{nameof(Postfix)}] : {__instance.Prototype.name} have the Reload.Dispersion flag. Overriding proto.");

				MapperConfiguration configuration = new MapperConfiguration(cfg =>
				{
					cfg.CreateMap<WeaponProto, Extended.WeaponProtoExtended>();
				});
				IMapper mapper = configuration.CreateMapper();

				Extended.WeaponProtoExtended newProto = mapper.Map<Extended.WeaponProtoExtended>(__instance.Prototype);
				newProto.Instantiate(__instance.Prototype);

				Debug.Log($"[{nameof(Weapon_Deserialize)}|{nameof(Postfix)}] : Base proto Icon -> {__instance.Prototype.Icon}");
				Debug.Log($"[{nameof(Weapon_Deserialize)}|{nameof(Postfix)}] : New proto Icon -> {newProto.Icon}");

				__instance.Prototype = newProto as WeaponProto;

				Debug.Log($"[{nameof(Weapon_Deserialize)}|{nameof(Postfix)}] : : {__instance.Prototype.name} prototype overrided.");


			}
		}
	}
}

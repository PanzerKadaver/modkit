using HarmonyLib;

namespace Assets.Scripts.Patches
{
	[HarmonyPatch(typeof(SpecHUD), "ShowInfo")]
	public static class SpecHUD_ShowInfo
	{
		/**
		 * Display Perks tooltips when player mouse hover perk's icon.
		 * This override will display the new perk cost.
		 * If #DEBUG constant is set, will also display the internal name of perk (ex : [SP_MasterMelee] )
		 **/

		public static bool Prefix(SpecHUD __instance, CharacterStats.Perk ____perkType, CharacterStats.Perk ____parentPerkType, bool ____active, bool ____activeParent, StatInfoHUD ____info)
		{
			UnityEngine.Vector2 min = __instance.GetComponent<UnityEngine.RectTransform>().rect.min;
			UnityEngine.Vector3 position = __instance.transform.position;
			string str = ____perkType.ToString().ToLower();
			Character character = CharacterSelectorHUD.GetCurrent().Character;
			bool isChildProdigy = character.CharProto.Stats.HasPerk(CharacterStats.Perk.ChildProdigy);
			int perkCost = SpecHUD_Utils.GetPerkCost(____perkType);

//#if DEBUG
			//string text = "[" + ____perkType.ToString() + "] ";
//#else
			string text = "";
//#endif
			text += Localization.TagBeginBold + Localization.Localize("perks." + str) + Localization.TagEndBold + $" <i><color=#EAD94EFF>[{perkCost} {Localization.Localize("perks.tp_abrev")}]</color></i>" + " - <color=#C8C8C8FF>" + Localization.Localize("perks." + str + "_info") + "</color>" + "\n\n";

			if (____perkType == CharacterStats.Perk.SP_AddExp && isChildProdigy)
			{
				text += string.Format(Localization.Localize("spec.status"), Localization.Localize("spec.status_child_prodigy"));
			}
			else if (!____activeParent)
			{
				text += string.Format(Localization.Localize("spec.status"), string.Format(Localization.Localize("spec.status_block"), Localization.Localize("perks." + ____parentPerkType)));
			}
			else if (____active)
			{
				text += string.Format(Localization.Localize("spec.status"), Localization.Localize("spec.status_active"));
			}
			else
			{
				int freeSpecPoints = character.CharProto.Stats.FreeSpecPoints;
				
				text += ((freeSpecPoints < perkCost) ? (string.Format(Localization.Localize("spec.status"), Localization.Localize("spec.status_not_enough"))) : (string.Format(Localization.Localize("spec.status"), Localization.Localize("spec.status_open"))));
			}
			____info.Show(position, new UnityEngine.Vector2(0f, 0f - min.y), text, "");

			return false; // Skip the original method
		}
	}

	[HarmonyPatch(typeof(SpecHUD), "OnClickSpec")]
	public static class SpecHUD_OnClickSpec
	{
		public static bool Prefix(
			SpecHUD __instance,
			bool ____aware,
			bool ____activeParent,
			ref bool ____active,
			CharacterStats.Perk ____perkType,
			SpecTreeView ____treeView,
			UnityEngine.AudioClip ____soundBuy,
			UnityEngine.AudioClip ____soundSell,
			UnityEngine.AudioClip ____soundBlock)
		{
			Character character = CharacterSelectorHUD.GetCurrent().Character;
			bool isChildProdigy = character.CharProto.Stats.HasPerk(CharacterStats.Perk.ChildProdigy);
			int perkCost = SpecHUD_Utils.GetPerkCost(____perkType);

			if (____aware)
			{
				return false;
			}
			if (!____activeParent || (____perkType == CharacterStats.Perk.SP_AddExp && isChildProdigy))
			{
				Game.World.HUD.PlayUISound(____soundBlock);
				return false;
			}
			if (____active)
			{
				if (!MyHasHookup(____treeView, ____perkType))
				{
					character.CharProto.Stats.RemovePerk(____perkType);
					character.CharProto.Stats.SpecLevel -= perkCost;
					character.CharProto.Stats.FreeSpecPoints += perkCost;
					____active = false;
					Game.World.HUD.PlayUISound(____soundSell);
				}
			}
			else if (character.CharProto.Stats.FreeSpecPoints >= perkCost)
			{
				character.CharProto.Stats.AddPerk(____perkType);
				character.CharProto.Stats.FreeSpecPoints -= perkCost;
				character.CharProto.Stats.SpecLevel += perkCost;
				____active = true;
				Game.World.HUD.PlayUISound(____soundBuy);
			}
			else
			{
				Game.World.HUD.PlayUISound(____soundBlock);
			}
			return false;
		}

		private static bool MyHasHookup(SpecTreeView _treeView, CharacterStats.Perk _perkType)
		{
			SpecTreeView.SpecItem[] specs = _treeView.specs;
			foreach (SpecTreeView.SpecItem specItem in specs)
			{
				if (specItem.parentPerk == _perkType && CharacterSelectorHUD.GetCurrent().HasPerk(specItem.perk))
				{
					return true;
				}
			}
			return false;
		}
	}

	[HarmonyPatch(typeof(SpecHUD), "Invalidate")]
	public static class SpecHUD_Invalidate
	{
		private static readonly UnityEngine.Color colorBlock = UnityEngine.Color.black;
		private static readonly UnityEngine.Color colorDisable = new UnityEngine.Color(1f, 1f, 1f, 1f);

		public static bool Prefix(SpecHUD __instance, CharacterStats.Perk ____perkType, UnityEngine.UI.Image ____lineDot, UnityEngine.UI.Image ____lineImage, UnityEngine.GameObject ____blocker)
		{
			Character character = CharacterSelectorHUD.GetCurrent().Character;
			bool isChildProdigy = character.CharProto.Stats.HasPerk(CharacterStats.Perk.ChildProdigy);

			if (____perkType == CharacterStats.Perk.SP_AddExp && isChildProdigy)
			{
				MySetLineColor(colorBlock, ____lineDot, ____lineImage);
				____blocker.SetActive(value: true);

				UnityEngine.UI.Button component = __instance.GetComponent<UnityEngine.UI.Button>();
				component.interactable = false;

				UnityEngine.UI.ColorBlock colors = component.colors;
				colors.normalColor = colorDisable;
				colors.disabledColor = colorDisable;
				component.colors = colors;

				return false;
			}

			return true;
		}

		private static void MySetLineColor(UnityEngine.Color color, UnityEngine.UI.Image _lineDot, UnityEngine.UI.Image _lineImage)
		{
			_lineDot.color = color;
			_lineImage.color = color;
		}
	}

	public static class SpecHUD_Utils
	{
		public static int GetPerkCost(CharacterStats.Perk perk)
		{
			switch (perk)
			{
				// Human perks
				// Melee Branch
				case CharacterStats.Perk.SP_MasterMelee:
					return 1;
				case CharacterStats.Perk.SP_MeleeAC:
					return 2;
				case CharacterStats.Perk.SP_MeleeHeal:
					return 3;
				case CharacterStats.Perk.SP_Lockpick:
					return 3;
				case CharacterStats.Perk.SP_MeleeStun:
					return 3;
				case CharacterStats.Perk.SP_MeleeAddDamge:
					return 4;
				case CharacterStats.Perk.SP_MeleeAP:
					return 4;
				case CharacterStats.Perk.SP_MeleeAssasin:
					return 3;
				case CharacterStats.Perk.SP_Sneak:
					return 4;
				case CharacterStats.Perk.SP_IA:
					return 4;
				case CharacterStats.Perk.SP_MeleeCrit:
					return 4;
				case CharacterStats.Perk.SP_Sequence:
					return 5;

				// Automatic Weapons Branch
				case CharacterStats.Perk.SP_MasterAuto:
					return 1;
				case CharacterStats.Perk.SP_IgnoreCloseRange:
					return 2;
				case CharacterStats.Perk.SP_Shot:
					return 3;
				case CharacterStats.Perk.SP_NoST:
					return 4;

				// Pistols/SMGs Branch
				case CharacterStats.Perk.SP_MasterPisol:
					return 1;
				case CharacterStats.Perk.SP_Duel:
					return 2;
				case CharacterStats.Perk.SP_FastPistol:
					return 3;
				case CharacterStats.Perk.SP_Reload:
					return 4;

				// Rifles/Shotguns Branch
				case CharacterStats.Perk.SP_MasterRifle:
					return 1;
				case CharacterStats.Perk.SP_Alertness:
					return 2;
				case CharacterStats.Perk.SP_NightVision:
					return 3;
				case CharacterStats.Perk.SP_Monster:
					return 3;
				case CharacterStats.Perk.SP_Sniper:
					return 3;
				case CharacterStats.Perk.SP_Damage2x:
					return 4;

				// Survival Branch
				case CharacterStats.Perk.SP_Rest:
					return 1;
				case CharacterStats.Perk.SP_Hunger:
					return 2;
				case CharacterStats.Perk.SP_Fishman:
					return 3;
				case CharacterStats.Perk.SP_Hunting:
					return 3;
				case CharacterStats.Perk.SP_Wanderer:
					return 2;
				case CharacterStats.Perk.SP_RustWeapon:
					return 3;
				case CharacterStats.Perk.SP_Craft:
					return 4;
				case CharacterStats.Perk.SP_CraftWeaponDamage:
					return 4;
				case CharacterStats.Perk.SP_Throw:
					return 2;
				case CharacterStats.Perk.SP_Dynamit:
					return 3;
				case CharacterStats.Perk.SP_Fire:
					return 4;

				// Knowledge Branch
				case CharacterStats.Perk.SP_BarterDiscount:
					return 1;
				case CharacterStats.Perk.SP_AddSkill:
					return 2;
				case CharacterStats.Perk.SP_Bookman:
					return 3;
				case CharacterStats.Perk.SP_AddExp:
					return 4;

				// Health Branch
				case CharacterStats.Perk.SP_Doc:
					return 1;
				case CharacterStats.Perk.SP_HP:
					return 2;
				case CharacterStats.Perk.SP_ToxicRadResistance:
					return 2;
				case CharacterStats.Perk.SP_NoDrugAddiction:
					return 3;
				case CharacterStats.Perk.SP_NoAlcoAddiction:
					return 3;
				case CharacterStats.Perk.SP_ToxicHeal:
					return 3;

				// Armor Branch
				case CharacterStats.Perk.SP_StunBlock:
					return 1;
				case CharacterStats.Perk.SP_MeleeArmor:
					return 2;
				case CharacterStats.Perk.SP_CarryWeight:
					return 2;
				case CharacterStats.Perk.SP_MeleeShieldArmor:
					return 3;
				case CharacterStats.Perk.SP_MeleeShieldBless:
					return 4;
				case CharacterStats.Perk.SP_MistAC:
					return 4;
				case CharacterStats.Perk.SP_Praetorian:
					return 5;

				// Wulf Perks
				// Health Branch
				case CharacterStats.Perk.SP_DOG_Meal:
					return 1;
				case CharacterStats.Perk.SP_DOG_HP:
					return 2;
				case CharacterStats.Perk.SP_DOG_Heal:
					return 3;
				case CharacterStats.Perk.SP_DOG_AP:
					return 4;
				case CharacterStats.Perk.SP_DOG_Rest:
					return 4;

				// Armor Branch
				case CharacterStats.Perk.SP_DOG_Defence:
					return 1;
				case CharacterStats.Perk.SP_DOG_ArmorClass:
					return 2;
				case CharacterStats.Perk.SP_DOG_Bulletproof:
					return 3;
				case CharacterStats.Perk.SP_DOG_Armor2x:
					return 3;
				case CharacterStats.Perk.SP_DOG_ArmorFit:
					return 3;
				case CharacterStats.Perk.SP_DOG_MPC:
					return 4;
				case CharacterStats.Perk.SP_DOG_NoPanic:
					return 4;

				// Misc Branch
				case CharacterStats.Perk.SP_DOG_Dodge:
					return 1;
				case CharacterStats.Perk.SP_DOG_StunBlock:
					return 2;
				case CharacterStats.Perk.SP_DOG_CarryWeight:
					return 3;
				case CharacterStats.Perk.SP_DOG_Sneak:
					return 3;
				case CharacterStats.Perk.SP_DOG_Watchdog:
					return 4;
				case CharacterStats.Perk.SP_DOG_Frisbie:
					return 4;

				// Attack branch
				case CharacterStats.Perk.SP_DOG_PanicAttack:
					return 1;
				case CharacterStats.Perk.SP_DOG_StunAttack:
					return 2;
				case CharacterStats.Perk.SP_DOG_Damage:
					return 3;
				case CharacterStats.Perk.SP_DOG_Crit:
					return 4;
				case CharacterStats.Perk.SP_DOG_Sequence:
					return 3;
				case CharacterStats.Perk.SP_DOG_FastAttack:
					return 4;
				case CharacterStats.Perk.SP_DOG_Fatality:
					return 4;
				case CharacterStats.Perk.SP_DOG_AI:
					return 3;
				case CharacterStats.Perk.SP_DOG_FirstAttack:
					return 4;
				case CharacterStats.Perk.SP_DOG_FAC:
					return 5;

				// Fallback
				default:
					return -1;
			}
		}
	}
}

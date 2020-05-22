using HarmonyLib;

namespace Assets.Scripts.Patches
{
	[HarmonyPatch(typeof(PerkEffect), "CollectModifiers")]
	public static class PerkEffect_CollectModifiers
	{
		public struct StateStruct
		{
			public bool _leaderState { get; set; }
			public int _sexAppealState { get; set; }
		}

		public static bool Prefix(out StateStruct __state, Modifiers m, ref bool ____leader, ref int ____sexappeal)
		{
			__state = new StateStruct();

			__state._leaderState = ____leader; // Saving _leader state
			if (____leader) // line 122
			{
				int count = Game.World.GetAllTeamMates().Count;
				if (count > 0)
				{
					m.Add(Modifier.ModifyType.Speech, 7 * count);
					m.Add(Modifier.ModifyType.FirstAid, 5 * count);
					m.Add(Modifier.ModifyType.Barter, 5 * count);
					//m.Add(Modifier.ModifyType.Perception, -1 * count); Removing attributes malus
					//m.Add(Modifier.ModifyType.Agility, -1 * count);
				}
			}
			____leader = false; // Disabling the original if block

			__state._sexAppealState = ____sexappeal; // Saving _sexappeal state
			if (____sexappeal != 0) // line 174
			{
				if (____sexappeal > 0)
				{
					m.Add(Modifier.ModifyType.Charisma, 2);
					m.Add(Modifier.ModifyType.Barter, 15);
				}
				// Reducing same sex malus
				else
				{
					m.Add(Modifier.ModifyType.Charisma, -1);
					// m.Add(Modifier.ModifyType.Charisma, -2);
					// m.Add(Modifier.ModifyType.Barter, -15);
				}
			}
			____sexappeal = 0; // Disabling the original if block

			return true; // Resuming the original function
		}

		public static void Postfix(StateStruct __state, ref bool ____leader, ref int ____sexappeal)
		{
			____leader = __state._leaderState; // Restoring _leader state
			____sexappeal = __state._sexAppealState; // Restoring _sexAppeal state
			
		}
	}
}

using System;

namespace Assets.Scripts.Extended
{
	[Serializable]
	public class WeaponExtended : Weapon
	{
		public Weapon _baseInstance;

		/// <summary>
		/// CTOR
		/// </summary>
		public WeaponExtended() { }

		public void Instantiate(Weapon wp)
		{
			_baseInstance = wp;
		}
	}
}

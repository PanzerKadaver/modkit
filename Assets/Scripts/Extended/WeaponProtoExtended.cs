using JSon;
using System;

namespace Assets.Scripts.Extended
{
	[Serializable]
	public class WeaponProtoExtended : WeaponProto
	{
		public WeaponProto _baseInstance;

		/// <summary>
		/// CTOR
		/// </summary>
		public WeaponProtoExtended() : base() {
			
		}

		public void Instantiate(WeaponProto wp)
		{
			_baseInstance = wp;
		}

		public void DeserializeProto(JNode node)
		{

		}
	}
}

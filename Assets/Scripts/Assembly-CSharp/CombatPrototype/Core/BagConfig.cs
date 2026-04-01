using System;

namespace CombatPrototype.Core
{
	[Serializable]
	public class BagConfig
	{
		public int SlashCount = 4;

		public int PierceCount = 3;

		public int BashCount = 3;

		public int EmotionCount = 4;

		public int SkillCount = 3;

		public int PassionCount = 3;

		public int HeroTotal => SlashCount + PierceCount + BashCount;

		public int SaintTotal => EmotionCount + SkillCount + PassionCount;

		public bool IsValid
		{
			get
			{
				if (HeroTotal == 10)
				{
					return SaintTotal == 10;
				}
				return false;
			}
		}

		public static BagConfig Default => new BagConfig();
	}
}

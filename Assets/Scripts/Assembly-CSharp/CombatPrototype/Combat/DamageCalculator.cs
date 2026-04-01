using System.Collections.Generic;
using CombatPrototype.Core;
using UnityEngine;

namespace CombatPrototype.Combat
{
	public static class DamageCalculator
	{
		public static int Calculate(List<Token> tokens, SynergyResult synergy, float enemyDefense, TurnEffects fx)
		{
			float num = 0f;
			foreach (Token token in tokens)
			{
				float num2 = Token.GetBaseDamage(token.Type);
				bool flag = token.Type == TokenType.Pierce || token.Type == TokenType.Ultimate;
				if (fx.IgnoreAllArmor || flag)
				{
					num += num2;
				}
				else if (fx.ArmorIgnorePercent > 0f)
				{
					float num3 = enemyDefense * (1f - fx.ArmorIgnorePercent);
					num += num2 * (1f - num3);
				}
				else
				{
					num += num2 * (1f - enemyDefense);
				}
			}
			if (fx.DrumSoloActive)
			{
				foreach (Token token2 in tokens)
				{
					if (token2.Type == TokenType.Bash)
					{
						float num4 = Token.GetBaseDamage(TokenType.Bash);
						float num5;
						if (fx.IgnoreAllArmor)
						{
							num5 = num4 * 0.4f;
						}
						else if (fx.ArmorIgnorePercent > 0f)
						{
							float num6 = enemyDefense * (1f - fx.ArmorIgnorePercent);
							num5 = num4 * (1f - num6) * 0.4f;
						}
						else
						{
							num5 = num4 * (1f - enemyDefense) * 0.4f;
						}
						num += num5;
					}
				}
			}
			num *= fx.DamageMultiplier;
			return Mathf.Max(0, Mathf.RoundToInt(num));
		}
	}
}

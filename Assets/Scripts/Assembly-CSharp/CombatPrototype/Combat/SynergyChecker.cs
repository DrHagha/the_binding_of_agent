using System.Collections.Generic;
using CombatPrototype.Core;

namespace CombatPrototype.Combat
{
	public static class SynergyChecker
	{
		public static SynergyResult Check(List<Token> tokens)
		{
			SynergyResult synergyResult = new SynergyResult();
			if (tokens == null || tokens.Count == 0)
			{
				return synergyResult;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			foreach (Token token in tokens)
			{
				switch (token.Type)
				{
				case TokenType.Slash:
					num++;
					break;
				case TokenType.Pierce:
					num2++;
					break;
				case TokenType.Bash:
					num3++;
					break;
				case TokenType.Ultimate:
					num4++;
					break;
				}
			}
			if (num >= 3)
			{
				synergyResult.Active.Add(SynergyType.Slash);
				synergyResult.DamageMultiplier *= 1.5f;
				synergyResult.GaugeBonus += 20f;
			}
			if (num2 >= 3)
			{
				synergyResult.Active.Add(SynergyType.Penetrate);
				synergyResult.DamageMultiplier *= 1.5f;
				synergyResult.IgnoreAllArmor = true;
				synergyResult.GaugeBonus += 20f;
			}
			if (num3 >= 3)
			{
				synergyResult.Active.Add(SynergyType.Bash);
				synergyResult.StunMultiplier *= 2f;
				synergyResult.GaugeBonus += 20f;
			}
			if (tokens.Count >= 2)
			{
				TokenType type = tokens[0].Type;
				if (type == TokenType.Slash && num < 3)
				{
					synergyResult.Active.Add(SynergyType.LeadSlash);
					synergyResult.DamageMultiplier *= 1.1f;
					synergyResult.GaugeBonus += 10f;
				}
				else if (type == TokenType.Pierce && num2 < 3)
				{
					synergyResult.Active.Add(SynergyType.LeadPenetrate);
					synergyResult.ArmorIgnoreBonus += 0.1f;
					synergyResult.GaugeBonus += 10f;
				}
				else if (type == TokenType.Bash && num3 < 3)
				{
					synergyResult.Active.Add(SynergyType.LeadBash);
					synergyResult.StunChanceBonus += 0.1f;
					synergyResult.GaugeBonus += 10f;
				}
			}
			int num5 = 0;
			foreach (Token token2 in tokens)
			{
				if (token2.Type == TokenType.Skill)
				{
					num5++;
				}
			}
			if (num5 >= 1 && num3 >= 1)
			{
				synergyResult.Active.Add(SynergyType.DrumSolo);
				synergyResult.DrumSoloActive = true;
				synergyResult.GaugeBonus += 15f;
			}
			if (num4 >= 3)
			{
				synergyResult.Active.Add(SynergyType.Signature);
				synergyResult.GlobalMultiplier *= 2f;
				synergyResult.GaugeBonus += 30f;
			}
			else if (num4 >= 2)
			{
				synergyResult.Active.Add(SynergyType.Special);
				synergyResult.GlobalMultiplier *= 1.5f;
				synergyResult.GaugeBonus += 20f;
			}
			return synergyResult;
		}
	}
}

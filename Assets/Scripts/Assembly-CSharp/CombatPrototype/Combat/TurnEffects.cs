using System.Collections.Generic;
using CombatPrototype.Core;
using UnityEngine;

namespace CombatPrototype.Combat
{
	public class TurnEffects
	{
		public float DamageMultiplier = 1f;

		public bool IgnoreAllArmor;

		public float ArmorIgnorePercent;

		public float StunChance;

		public float DodgeChance;

		public bool HasReflect;

		public float ReflectRatio = 0.5f;

		public float IncomingDamageMultiplier = 1f;

		public bool DrumSoloActive;

		public float DrumSoloBashStunChance;

		public bool ShouldStun()
		{
			if (StunChance <= 0f)
			{
				return false;
			}
			return Random.value < StunChance;
		}

		public bool ShouldDodge()
		{
			if (DodgeChance <= 0f)
			{
				return false;
			}
			return Random.value < DodgeChance;
		}

		public static TurnEffects Compute(List<Token> tokens, SynergyResult synergy, CharacterData hero, CharacterData saint)
		{
			TurnEffects turnEffects = new TurnEffects();
			foreach (Token token in tokens)
			{
				switch (token.Type)
				{
				case TokenType.Bash:
					turnEffects.StunChance += 0.2f;
					break;
				case TokenType.Emotion:
					turnEffects.HasReflect = true;
					break;
				case TokenType.Passion:
					turnEffects.DamageMultiplier *= 2f;
					turnEffects.IncomingDamageMultiplier *= 1.5f;
					break;
				case TokenType.Ultimate:
					turnEffects.StunChance += 0.6f;
					break;
				}
			}
			if (saint != null && saint.Effects.Has(ActiveEffectType.DodgeBoost))
			{
				turnEffects.DodgeChance = saint.Effects.GetValue(ActiveEffectType.DodgeBoost);
			}
			else if (hero != null && hero.Effects.Has(ActiveEffectType.DodgeBoost))
			{
				turnEffects.DodgeChance = hero.Effects.GetValue(ActiveEffectType.DodgeBoost);
			}
			turnEffects.DamageMultiplier *= synergy.DamageMultiplier;
			turnEffects.DamageMultiplier *= synergy.GlobalMultiplier;
			if (synergy.IgnoreAllArmor)
			{
				turnEffects.IgnoreAllArmor = true;
			}
			turnEffects.ArmorIgnorePercent = synergy.ArmorIgnoreBonus;
			turnEffects.StunChance *= synergy.StunMultiplier;
			turnEffects.StunChance += synergy.StunChanceBonus;
			turnEffects.StunChance *= synergy.GlobalMultiplier;
			if (synergy.DrumSoloActive)
			{
				turnEffects.DrumSoloActive = true;
				turnEffects.DrumSoloBashStunChance = 0.2f;
			}
			return turnEffects;
		}
	}
}

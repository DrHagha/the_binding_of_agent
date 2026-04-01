using UnityEngine;

namespace CombatPrototype.Core
{
	public class ActiveEffect
	{
		public ActiveEffectType Type { get; private set; }

		public int RemainingTurns { get; private set; }

		public float Value { get; private set; }

		public bool IsExpired => RemainingTurns <= 0;

		public ActiveEffect(ActiveEffectType type, int turns, float value = 0f)
		{
			Type = type;
			RemainingTurns = turns;
			Value = value;
		}

		public void Tick()
		{
			RemainingTurns = Mathf.Max(0, RemainingTurns - 1);
		}

		public void Refresh(int turns)
		{
			RemainingTurns = Mathf.Max(RemainingTurns, turns);
		}
	}
}

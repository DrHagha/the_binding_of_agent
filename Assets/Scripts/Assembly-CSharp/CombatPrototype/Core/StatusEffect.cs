namespace CombatPrototype.Core
{
	public class StatusEffect
	{
		public StatusEffectType Type { get; private set; }

		public bool IsStunned => Type == StatusEffectType.Stun;

		public void ApplyStun()
		{
			Type = StatusEffectType.Stun;
		}

		public void Tick()
		{
			Type = StatusEffectType.None;
		}

		public void Clear()
		{
			Type = StatusEffectType.None;
		}
	}
}

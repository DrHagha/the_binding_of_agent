using System.Collections.Generic;

namespace CombatPrototype.Core
{
	public class ActiveEffects
	{
		private readonly List<ActiveEffect> _effects = new List<ActiveEffect>();

		public IReadOnlyList<ActiveEffect> All => _effects;

		public void Add(ActiveEffectType type, int turns, float value = 0f)
		{
			ActiveEffect activeEffect = _effects.Find((ActiveEffect e) => e.Type == type);
			if (activeEffect != null)
			{
				activeEffect.Refresh(turns);
			}
			else
			{
				_effects.Add(new ActiveEffect(type, turns, value));
			}
		}

		public void Clear()
		{
			_effects.Clear();
		}

		public bool Has(ActiveEffectType type)
		{
			return _effects.Exists((ActiveEffect e) => e.Type == type && !e.IsExpired);
		}

		public float GetValue(ActiveEffectType type)
		{
			return _effects.Find((ActiveEffect x) => x.Type == type && !x.IsExpired)?.Value ?? 0f;
		}

		public void Tick()
		{
			foreach (ActiveEffect effect in _effects)
			{
				effect.Tick();
			}
			_effects.RemoveAll((ActiveEffect e) => e.IsExpired);
		}
	}
}

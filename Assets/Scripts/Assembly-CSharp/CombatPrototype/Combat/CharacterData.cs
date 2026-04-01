using CombatPrototype.Core;
using UnityEngine;

namespace CombatPrototype.Combat
{
	public class CharacterData
	{
		public string Name { get; private set; }

		public int MaxLP { get; private set; }

		public int CurrentLP { get; private set; }

		public float Gauge { get; private set; }

		public bool IsAlive => CurrentLP > 0;

		public float GaugePercent => Mathf.Clamp(Gauge, 0f, 100f);

		public TokenBag Bag { get; private set; }

		public ActiveEffects Effects { get; private set; } = new ActiveEffects();

		public CharacterData(string name, int maxLP, BagOwner owner)
		{
			Name = name;
			MaxLP = maxLP;
			Bag = new TokenBag(owner);
		}

		public void Initialize(BagConfig config)
		{
			CurrentLP = MaxLP;
			Gauge = 0f;
			Bag.Initialize(config);
			Effects.Clear();
		}

		public bool TakeDamage(int damage)
		{
			if (damage <= 0 || !IsAlive)
			{
				return false;
			}
			CurrentLP = Mathf.Max(0, CurrentLP - damage);
			Gauge = Mathf.Min(100f, Gauge + (float)damage);
			return true;
		}

		public bool AddGauge(float amount)
		{
			Gauge = Mathf.Min(100f, Gauge + amount);
			if (Gauge >= 100f)
			{
				Gauge -= 100f;
				Bag.AddToken(TokenType.Ultimate);
				return true;
			}
			return false;
		}

		public void AddEffect(ActiveEffectType type, int turns, float value = 0f)
		{
			Effects.Add(type, turns, value);
		}

		public void TickEffects()
		{
			Effects.Tick();
		}
	}
}

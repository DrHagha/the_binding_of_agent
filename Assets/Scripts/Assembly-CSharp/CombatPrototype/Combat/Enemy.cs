using CombatPrototype.Core;
using UnityEngine;

namespace CombatPrototype.Combat
{
	public abstract class Enemy : MonoBehaviour
	{
		[Header("기본 스탯")]
		public string EnemyName = "적";

		public int MaxHP = 60;

		public float Defense = 0.4f;

		protected int TurnCount;

		private StatusEffect _statusEffect = new StatusEffect();

		public int CurrentHP { get; protected set; }

		public bool IsDead => CurrentHP <= 0;

		public bool IsStunned => _statusEffect.IsStunned;

		public virtual bool ActsFirst => false;

		public virtual void Initialize()
		{
			CurrentHP = MaxHP;
			TurnCount = 0;
			_statusEffect.Clear();
		}

		public void TakeDamage(int damage)
		{
			CurrentHP = Mathf.Max(0, CurrentHP - damage);
		}

		public void ApplyStun()
		{
			_statusEffect.ApplyStun();
		}

		public EnemyTurnResult ExecuteTurn(CharacterData hero, CharacterData saint, TurnEffects fx)
		{
			if (_statusEffect.IsStunned)
			{
				_statusEffect.Tick();
				return new EnemyTurnResult
				{
					DamageDealt = 0,
					ActionDescription = EnemyName + " 기절! 행동 불가.",
					TargetName = "-"
				};
			}
			_statusEffect.Tick();
			TurnCount++;
			return DoTurn(hero, saint, fx);
		}

		protected abstract EnemyTurnResult DoTurn(CharacterData hero, CharacterData saint, TurnEffects fx);

		protected CharacterData PickTarget(CharacterData hero, CharacterData saint)
		{
			bool isAlive = hero.IsAlive;
			bool isAlive2 = saint.IsAlive;
			if (isAlive && isAlive2)
			{
				if (!(Random.value < 0.5f))
				{
					return saint;
				}
				return hero;
			}
			if (isAlive)
			{
				return hero;
			}
			return saint;
		}

		protected int ApplyAttack(CharacterData target, int rawDamage, TurnEffects fx, out bool dodged, out int reflected)
		{
			dodged = false;
			reflected = 0;
			if (fx.ShouldDodge())
			{
				dodged = true;
				return 0;
			}
			int num = Mathf.RoundToInt((float)rawDamage * fx.IncomingDamageMultiplier);
			target.TakeDamage(num);
			if (fx.HasReflect)
			{
				reflected = Mathf.RoundToInt((float)num * fx.ReflectRatio);
				TakeDamage(reflected);
			}
			return num;
		}
	}
}

using CombatPrototype.Core;

namespace CombatPrototype.Combat
{
	public class BagDestroyerEnemy : Enemy
	{
		private void Awake()
		{
			EnemyName = "Bag 파괴형";
			MaxHP = 60;
			Defense = 0.2f;
		}

		protected override EnemyTurnResult DoTurn(CharacterData hero, CharacterData saint, TurnEffects fx)
		{
			if (TurnCount % 2 == 1)
			{
				int num = 4 + 2 * TurnCount;
				CharacterData characterData = PickTarget(hero, saint);
				ApplyAttack(characterData, num, fx, out var dodged, out var reflected);
				string actionDescription = (dodged ? ("Bag 파괴형 공격! → " + characterData.Name + " 회피!") : ($"Bag 파괴형 공격! {characterData.Name}에게 {num} 데미지" + ((reflected > 0) ? $" / 반사 {reflected}" : "")));
				return new EnemyTurnResult
				{
					DamageDealt = ((!dodged) ? num : 0),
					ActionDescription = actionDescription,
					TargetName = characterData.Name
				};
			}
			if (hero.IsAlive)
			{
				hero.Bag.AddToken(TokenType.Junk);
			}
			saint.Bag.AddToken(TokenType.Junk);
			return new EnemyTurnResult
			{
				DamageDealt = 0,
				ActionDescription = "Bag 파괴형: 잉어킹 토큰 투입! (두 백에 1개씩)",
				AddedJunkToken = true,
				TargetName = "백"
			};
		}
	}
}

namespace CombatPrototype.Combat
{
	public class GrowthEnemy : Enemy
	{
		private void Awake()
		{
			EnemyName = "성장형";
			MaxHP = 60;
			Defense = 0.4f;
		}

		protected override EnemyTurnResult DoTurn(CharacterData hero, CharacterData saint, TurnEffects fx)
		{
			int num = 6 + 2 * TurnCount;
			CharacterData characterData = PickTarget(hero, saint);
			ApplyAttack(characterData, num, fx, out var dodged, out var reflected);
			string actionDescription = (dodged ? ("성장형 공격! → " + characterData.Name + " 회피!") : ($"성장형 공격! {characterData.Name}에게 {num} 데미지" + ((reflected > 0) ? $" / 반사 {reflected}" : "")));
			return new EnemyTurnResult
			{
				DamageDealt = ((!dodged) ? num : 0),
				ActionDescription = actionDescription,
				TargetName = characterData.Name
			};
		}
	}
}

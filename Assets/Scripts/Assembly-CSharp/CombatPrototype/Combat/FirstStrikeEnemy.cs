namespace CombatPrototype.Combat
{
	public class FirstStrikeEnemy : Enemy
	{
		public override bool ActsFirst => true;

		private void Awake()
		{
			EnemyName = "선공형";
			MaxHP = 60;
			Defense = 0.2f;
		}

		protected override EnemyTurnResult DoTurn(CharacterData hero, CharacterData saint, TurnEffects fx)
		{
			int num = 5 + 2 * TurnCount;
			CharacterData characterData = PickTarget(hero, saint);
			ApplyAttack(characterData, num, fx, out var dodged, out var reflected);
			string actionDescription = (dodged ? ("선공형 선제 공격! → " + characterData.Name + " 회피!") : ($"선공형 선제 공격! {characterData.Name}에게 {num} 데미지" + ((reflected > 0) ? $" / 반사 {reflected}" : "")));
			return new EnemyTurnResult
			{
				DamageDealt = ((!dodged) ? num : 0),
				ActionDescription = actionDescription,
				TargetName = characterData.Name
			};
		}
	}
}

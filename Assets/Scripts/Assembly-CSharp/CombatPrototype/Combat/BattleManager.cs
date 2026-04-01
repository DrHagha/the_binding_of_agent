using System;
using System.Collections;
using System.Collections.Generic;
using CombatPrototype.Core;
using UnityEngine;

namespace CombatPrototype.Combat
{
	public class BattleManager : MonoBehaviour
	{
		private BagOwner _freeBagChoice;

		private Enemy[] _allEnemies;

		private int[] _enemyOrder;

		private int _battleIndex;

		private IBattleObserver _observer;

		public BagConfig BagConfig { get; private set; } = BagConfig.Default;

		public CharacterData Hero { get; private set; }

		public CharacterData Saint { get; private set; }

		public Enemy CurrentEnemy { get; private set; }

		public BattleState CurrentState { get; private set; }

		public List<Token> DrawnPair { get; private set; } = new List<Token>();

		public List<Token> SelectedTokens { get; private set; } = new List<Token>();

		public int BattleIndex => _battleIndex;

		public int TotalBattles => 3;

		public void RegisterObserver(IBattleObserver obs)
		{
			_observer = obs;
		}

		public void RegisterEnemies(Enemy[] enemies)
		{
			_allEnemies = enemies;
		}

		private void Start()
		{
			Hero = new CharacterData("용사", 50, BagOwner.Hero);
			Saint = new CharacterData("성녀 얀", 50, BagOwner.Saint);
			ChangeState(BattleState.BagBuilder);
		}

		public void StartGame(BagConfig config)
		{
			BagConfig = config;
			_battleIndex = 0;
			_enemyOrder = ShuffledEnemyOrder();
			StartBattle();
		}

		private void StartBattle()
		{
			Enemy[] allEnemies = _allEnemies;
			for (int i = 0; i < allEnemies.Length; i++)
			{
				allEnemies[i].gameObject.SetActive(value: false);
			}
			CurrentEnemy = _allEnemies[_enemyOrder[_battleIndex]];
			CurrentEnemy.gameObject.SetActive(value: true);
			Hero.Initialize(BagConfig);
			Saint.Initialize(BagConfig);
			CurrentEnemy.Initialize();
			Log($"=== 전투 {_battleIndex + 1}/{TotalBattles} 시작! 상대: {CurrentEnemy.EnemyName} ===");
			_observer?.OnBattleProgress(_battleIndex + 1, TotalBattles);
			StartTurn();
		}

		private void StartTurn()
		{
			SelectedTokens.Clear();
			DrawnPair.Clear();
			if (CurrentEnemy.ActsFirst)
			{
				ChangeState(BattleState.EnemyFirstStrike);
				StartCoroutine(EnemyFirstStrikePhase());
			}
			else
			{
				StartCoroutine(DraftHeroPhase());
			}
		}

		private IEnumerator DraftHeroPhase()
		{
			if (!Hero.IsAlive)
			{
				StartCoroutine(DraftSaintPhase());
				yield break;
			}
			if (Hero.Bag.IsEmpty)
			{
				Log("용사 백 소진. 용사 드래프트 생략.");
				StartCoroutine(DraftSaintPhase());
				yield break;
			}
			DrawnPair = Hero.Bag.Draw(2);
			if (DrawnPair.Count == 1)
			{
				Log($"용사 백 1개 남아 자동 선택: [{DrawnPair[0]}]");
				SelectedTokens.Add(DrawnPair[0]);
				DrawnPair.Clear();
				StartCoroutine(DraftSaintPhase());
			}
			else
			{
				Log("[드래프트 1/3] 용사 백에서 2장 뽑기");
				ChangeState(BattleState.HeroSelect);
				_observer?.OnDraftChoice(BattleState.HeroSelect, DrawnPair);
			}
		}

		public void OnHeroTokenSelected(int index)
		{
			SelectToken(index, Hero.Bag, DraftSaintPhase);
		}

		private IEnumerator DraftSaintPhase()
		{
			if (Saint.Bag.IsEmpty)
			{
				Log("성녀 백 소진. 성녀 드래프트 생략.");
				StartCoroutine(DraftFreePhase());
				yield break;
			}
			DrawnPair = Saint.Bag.Draw(2);
			if (DrawnPair.Count == 1)
			{
				Log($"성녀 백 1개 남아 자동 선택: [{DrawnPair[0]}]");
				SelectedTokens.Add(DrawnPair[0]);
				DrawnPair.Clear();
				StartCoroutine(DraftFreePhase());
			}
			else if (Saint.Effects.Has(ActiveEffectType.RandomSaintSelect))
			{
				int index = UnityEngine.Random.Range(0, DrawnPair.Count);
				Log($"기교 효과: 성녀 토큰 자동 선택! [{DrawnPair[index]}]");
				SelectToken(index, Saint.Bag, DraftFreePhase);
			}
			else
			{
				Log("[드래프트 2/3] 성녀 백에서 2장 뽑기");
				ChangeState(BattleState.SaintSelect);
				_observer?.OnDraftChoice(BattleState.SaintSelect, DrawnPair);
			}
		}

		public void OnSaintTokenSelected(int index)
		{
			SelectToken(index, Saint.Bag, DraftFreePhase);
		}

		private IEnumerator DraftFreePhase()
		{
			bool flag = Hero.IsAlive && !Hero.Bag.IsEmpty;
			bool flag2 = !Saint.Bag.IsEmpty;
			if (!flag && !flag2)
			{
				StartCoroutine(CheckBagDefeat());
			}
			else if (!flag)
			{
				_freeBagChoice = BagOwner.Saint;
				StartCoroutine(DraftFreeDrawPhase());
			}
			else if (!flag2)
			{
				_freeBagChoice = BagOwner.Hero;
				StartCoroutine(DraftFreeDrawPhase());
			}
			else
			{
				Log("[드래프트 3/3] 어느 백에서 뽑을지 선택하세요.");
				ChangeState(BattleState.BagChoiceRequired);
				_observer?.OnBagChoiceRequired();
			}
			yield break;
		}

		public void OnBagChosen(BagOwner owner)
		{
			if (CurrentState == BattleState.BagChoiceRequired)
			{
				_freeBagChoice = owner;
				StartCoroutine(DraftFreeDrawPhase());
			}
		}

		private IEnumerator DraftFreeDrawPhase()
		{
			TokenBag tokenBag = ((_freeBagChoice == BagOwner.Hero) ? Hero.Bag : Saint.Bag);
			DrawnPair = tokenBag.Draw(2);
			if (DrawnPair.Count == 0)
			{
				StartCoroutine(CheckBagDefeat());
			}
			else if (DrawnPair.Count == 1)
			{
				Log($"[드래프트 3/3] 1개 남아 자동 선택: [{DrawnPair[0]}]");
				SelectedTokens.Add(DrawnPair[0]);
				DrawnPair.Clear();
				StartCoroutine(SynergyPhase());
			}
			else
			{
				Log("[드래프트 3/3] " + ((_freeBagChoice == BagOwner.Hero) ? "용사" : "성녀") + " 백에서 2장 뽑기");
				ChangeState(BattleState.FreeSelect);
				_observer?.OnDraftChoice(BattleState.FreeSelect, DrawnPair);
			}
			yield break;
		}

		public void OnFreeTokenSelected(int index)
		{
			TokenBag sourceBag = ((_freeBagChoice == BagOwner.Hero) ? Hero.Bag : Saint.Bag);
			SelectToken(index, sourceBag, SynergyPhase);
		}

		private void SelectToken(int index, TokenBag sourceBag, Func<IEnumerator> next)
		{
			if (index < 0 || index >= DrawnPair.Count)
			{
				return;
			}
			for (int i = 0; i < DrawnPair.Count; i++)
			{
				if (i != index)
				{
					sourceBag.ReturnToken(DrawnPair[i]);
				}
			}
			Log($"선택: [{DrawnPair[index]}]");
			SelectedTokens.Add(DrawnPair[index]);
			DrawnPair.Clear();
			_observer?.OnRefresh();
			StartCoroutine(next());
		}

		private IEnumerator SynergyPhase()
		{
			ChangeState(BattleState.SynergyCheck);
			yield return new WaitForSeconds(0.4f);
			SynergyResult synergyResult = SynergyChecker.Check(SelectedTokens);
			TurnEffects fx = TurnEffects.Compute(SelectedTokens, synergyResult, Hero, Saint);
			int damage = DamageCalculator.Calculate(SelectedTokens, synergyResult, CurrentEnemy.Defense, fx);
			if (synergyResult.HasAnySynergy)
			{
				Log("✦ " + synergyResult.BuildDescription());
				bool num = Hero.IsAlive && Hero.AddGauge(synergyResult.GaugeBonus);
				bool flag = Saint.IsAlive && Saint.AddGauge(synergyResult.GaugeBonus / 2f);
				if (num || flag)
				{
					Log("필살기 게이지 100%! 거장의 혼 토큰 추가!");
				}
			}
			_observer?.OnSynergyResult(synergyResult, damage);
			yield return new WaitForSeconds(0.8f);
			StartCoroutine(AllyTurnPhase(fx, damage));
		}

		private IEnumerator AllyTurnPhase(TurnEffects fx, int damage)
		{
			ChangeState(BattleState.AllyTurn);
			yield return new WaitForSeconds(0.3f);
			CurrentEnemy.TakeDamage(damage);
			if (fx.ShouldStun())
			{
				CurrentEnemy.ApplyStun();
				Log("기절! " + CurrentEnemy.EnemyName + "이(가) 다음 행동 불가.");
			}
			if (fx.DrumSoloActive && fx.DrumSoloBashStunChance > 0f && UnityEngine.Random.value < fx.DrumSoloBashStunChance)
			{
				CurrentEnemy.ApplyStun();
				Log("드럼 솔로 추가 기절! " + CurrentEnemy.EnemyName + " 다음 행동 불가.");
			}
			foreach (Token selectedToken in SelectedTokens)
			{
				if (selectedToken.Type == TokenType.Skill)
				{
					Saint.AddEffect(ActiveEffectType.DodgeBoost, 2, 0.7f);
					Saint.AddEffect(ActiveEffectType.RandomSaintSelect, 2);
					Log("기교 발동! 성녀 2턴간 회피 +70% + 다음 성녀 드래프트 자동 선택.");
					break;
				}
			}
			if (Hero.IsAlive)
			{
				Hero.AddGauge(4f);
			}
			if (Saint.IsAlive)
			{
				Saint.AddGauge(2f);
			}
			Log($"아군 공격! 데미지: {damage} (적 HP: {CurrentEnemy.CurrentHP}/{CurrentEnemy.MaxHP})");
			_observer?.OnRefresh();
			if (CurrentEnemy.IsDead)
			{
				yield return new WaitForSeconds(0.5f);
				yield return StartCoroutine(HandleBattleVictory());
				yield break;
			}
			yield return new WaitForSeconds(0.5f);
			if (CurrentEnemy.ActsFirst)
			{
				StartCoroutine(TurnEndPhase());
			}
			else
			{
				StartCoroutine(EnemyTurnPhase(fx));
			}
		}

		private IEnumerator EnemyFirstStrikePhase()
		{
			yield return new WaitForSeconds(0.5f);
			TurnEffects fx = new TurnEffects();
			EnemyTurnResult enemyTurnResult = CurrentEnemy.ExecuteTurn(Hero, Saint, fx);
			Log(enemyTurnResult.ActionDescription);
			_observer?.OnRefresh();
			if (!Saint.IsAlive)
			{
				yield return new WaitForSeconds(0.5f);
				ChangeState(BattleState.GameOver);
				Log("성녀 얀 HP 0. 전투 패배!");
				_observer?.OnGameOver(cleared: false);
			}
			else
			{
				yield return new WaitForSeconds(0.5f);
				StartCoroutine(DraftHeroPhase());
			}
		}

		private IEnumerator EnemyTurnPhase(TurnEffects fx)
		{
			ChangeState(BattleState.EnemyTurn);
			yield return new WaitForSeconds(0.3f);
			EnemyTurnResult enemyTurnResult = CurrentEnemy.ExecuteTurn(Hero, Saint, fx);
			Log(enemyTurnResult.ActionDescription);
			if (enemyTurnResult.AddedJunkToken)
			{
				Log($"잉어킹 토큰 투입! (용사 백: {Hero.Bag.Count} / 성녀 백: {Saint.Bag.Count})");
			}
			_observer?.OnRefresh();
			yield return new WaitForSeconds(0.5f);
			if (!Saint.IsAlive)
			{
				ChangeState(BattleState.GameOver);
				Log("성녀 얀 HP 0. 전투 패배!");
				_observer?.OnGameOver(cleared: false);
			}
			else
			{
				StartCoroutine(TurnEndPhase());
			}
		}

		private IEnumerator TurnEndPhase()
		{
			ChangeState(BattleState.TurnEnd);
			yield return new WaitForSeconds(0.2f);
			Hero.TickEffects();
			Saint.TickEffects();
			bool num = !Hero.IsAlive || Hero.Bag.IsEmpty;
			bool isEmpty = Saint.Bag.IsEmpty;
			if (num && isEmpty)
			{
				ChangeState(BattleState.GameOver);
				Log("두 백 모두 소진. 전투 패배!");
				_observer?.OnGameOver(cleared: false);
			}
			else
			{
				Log("─── 다음 턴 ───");
				StartTurn();
			}
		}

		private IEnumerator CheckBagDefeat()
		{
			ChangeState(BattleState.GameOver);
			Log("뽑을 수 있는 토큰이 없습니다. 전투 패배!");
			_observer?.OnGameOver(cleared: false);
			yield break;
		}

		private IEnumerator HandleBattleVictory()
		{
			_battleIndex++;
			if (_battleIndex >= TotalBattles)
			{
				ChangeState(BattleState.GameClear);
				Log("=== 3전 모두 승리! 최종 클리어! ===");
				_observer?.OnGameOver(cleared: true);
			}
			else
			{
				ChangeState(BattleState.BattleVictory);
				Log($"=== {_battleIndex}/{TotalBattles}전 승리! 잠시 후 다음 전투 시작 ===");
				_observer?.OnBattleProgress(_battleIndex + 1, TotalBattles);
				yield return new WaitForSeconds(2f);
				StartBattle();
			}
		}

		private int[] ShuffledEnemyOrder()
		{
			int[] array = new int[3] { 0, 1, 2 };
			for (int num = array.Length - 1; num > 0; num--)
			{
				int num2 = UnityEngine.Random.Range(0, num + 1);
				ref int reference = ref array[num];
				ref int reference2 = ref array[num2];
				int num3 = array[num2];
				int num4 = array[num];
				reference = num3;
				reference2 = num4;
			}
			return array;
		}

		private void ChangeState(BattleState s)
		{
			CurrentState = s;
			_observer?.OnStateChanged(s);
		}

		public void Log(string msg)
		{
			Debug.Log("[Battle] " + msg);
			_observer?.OnLog(msg);
		}

		public void RestartGame()
		{
			StopAllCoroutines();
			Enemy[] allEnemies = _allEnemies;
			for (int i = 0; i < allEnemies.Length; i++)
			{
				allEnemies[i].gameObject.SetActive(value: false);
			}
			ChangeState(BattleState.BagBuilder);
		}
	}
}

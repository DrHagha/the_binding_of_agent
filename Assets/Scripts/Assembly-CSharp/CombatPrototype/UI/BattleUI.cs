using System.Collections;
using System.Collections.Generic;
using System.Text;
using CombatPrototype.Combat;
using CombatPrototype.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CombatPrototype.UI
{
	public class BattleUI : MonoBehaviour, IBattleObserver
	{
		[Header("참조")]
		public BattleManager Battle;

		public GameObject BagBuilderPanel;

		public Slider HeroSlashSlider;

		public Slider HeroPierceSlider;

		public Slider HeroBashSlider;

		public TextMeshProUGUI HeroSlashVal;

		public TextMeshProUGUI HeroPierceVal;

		public TextMeshProUGUI HeroBashVal;

		public TextMeshProUGUI HeroTotalText;

		public Slider SaintEmotionSlider;

		public Slider SaintSkillSlider;

		public Slider SaintPassionSlider;

		public TextMeshProUGUI SaintEmotionVal;

		public TextMeshProUGUI SaintSkillVal;

		public TextMeshProUGUI SaintPassionVal;

		public TextMeshProUGUI SaintTotalText;

		public Button StartGameButton;

		public TextMeshProUGUI BagValidationText;

		public GameObject BattlePanel;

		public Slider EnemyHPSlider;

		public TextMeshProUGUI EnemyHPText;

		public TextMeshProUGUI EnemyNameText;

		public Slider HeroHPSlider;

		public TextMeshProUGUI HeroHPText;

		public TextMeshProUGUI HeroBagCountText;

		public TextMeshProUGUI HeroStatusText;

		public Slider SaintHPSlider;

		public TextMeshProUGUI SaintHPText;

		public TextMeshProUGUI SaintBagCountText;

		public GaugeBar UltimateGaugeBar;

		public GameObject TokenDrawPanel;

		public TokenButton TokenChoiceA;

		public TokenButton TokenChoiceB;

		public GameObject BagChoicePanel;

		public Button ChooseHeroBagButton;

		public Button ChooseSaintBagButton;

		public TextMeshProUGUI BagChoiceHeroCount;

		public TextMeshProUGUI BagChoiceSaintCount;

		public TextMeshProUGUI SelectedTokensText;

		public TextMeshProUGUI TurnPhaseText;

		public TextMeshProUGUI SynergyText;

		public ScrollRect LogScrollRect;

		public TextMeshProUGUI LogText;

		private StringBuilder _log = new StringBuilder();

		public TextMeshProUGUI BattleProgressText;

		public GameObject GameOverPanel;

		public TextMeshProUGUI GameOverText;

		public Button RestartButton;

		public ScrollRect GameOverLogScroll;

		public TextMeshProUGUI GameOverLogText;

		public void Initialize()
		{
			HeroSlashSlider.onValueChanged.AddListener(delegate
			{
				RefreshBagBuilder();
			});
			HeroPierceSlider.onValueChanged.AddListener(delegate
			{
				RefreshBagBuilder();
			});
			HeroBashSlider.onValueChanged.AddListener(delegate
			{
				RefreshBagBuilder();
			});
			SaintEmotionSlider.onValueChanged.AddListener(delegate
			{
				RefreshBagBuilder();
			});
			SaintSkillSlider.onValueChanged.AddListener(delegate
			{
				RefreshBagBuilder();
			});
			SaintPassionSlider.onValueChanged.AddListener(delegate
			{
				RefreshBagBuilder();
			});
			StartGameButton.onClick.AddListener(OnStartGameClicked);
			RestartButton.onClick.AddListener(Battle.RestartGame);
			ChooseHeroBagButton.onClick.AddListener(delegate
			{
				Battle.OnBagChosen(BagOwner.Hero);
			});
			ChooseSaintBagButton.onClick.AddListener(delegate
			{
				Battle.OnBagChosen(BagOwner.Saint);
			});
			RefreshBagBuilder();
			ShowBagBuilder(show: true);
		}

		private void RefreshBagBuilder()
		{
			int num = (int)HeroSlashSlider.value;
			int num2 = (int)HeroPierceSlider.value;
			int num3 = (int)HeroBashSlider.value;
			int num4 = num + num2 + num3;
			HeroSlashVal.text = num.ToString();
			HeroPierceVal.text = num2.ToString();
			HeroBashVal.text = num3.ToString();
			HeroTotalText.text = $"용사 합계: {num4}/10";
			HeroTotalText.color = ((num4 == 10) ? Color.green : Color.red);
			int num5 = (int)SaintEmotionSlider.value;
			int num6 = (int)SaintSkillSlider.value;
			int num7 = (int)SaintPassionSlider.value;
			int num8 = num5 + num6 + num7;
			SaintEmotionVal.text = num5.ToString();
			SaintSkillVal.text = num6.ToString();
			SaintPassionVal.text = num7.ToString();
			SaintTotalText.text = $"성녀 합계: {num8}/10";
			SaintTotalText.color = ((num8 == 10) ? Color.green : Color.red);
			bool flag = num4 == 10 && num8 == 10;
			StartGameButton.interactable = flag;
			BagValidationText.text = (flag ? "전투 준비 완료!" : "각 백은 정확히 10개여야 합니다.");
			BagValidationText.color = (flag ? Color.green : Color.yellow);
		}

		private void OnStartGameClicked()
		{
			BagConfig config = new BagConfig
			{
				SlashCount = (int)HeroSlashSlider.value,
				PierceCount = (int)HeroPierceSlider.value,
				BashCount = (int)HeroBashSlider.value,
				EmotionCount = (int)SaintEmotionSlider.value,
				SkillCount = (int)SaintSkillSlider.value,
				PassionCount = (int)SaintPassionSlider.value
			};
			Battle.StartGame(config);
		}

		public void OnStateChanged(BattleState state)
		{
			bool flag = state != BattleState.BagBuilder;
			ShowBagBuilder(!flag);
			BattlePanel.SetActive(flag && state != BattleState.GameOver && state != BattleState.GameClear);
			GameOverPanel.SetActive(state == BattleState.GameOver || state == BattleState.GameClear);
			TokenDrawPanel.SetActive(state == BattleState.HeroSelect || state == BattleState.SaintSelect || state == BattleState.FreeSelect);
			BagChoicePanel.SetActive(state == BattleState.BagChoiceRequired);
			TextMeshProUGUI turnPhaseText = TurnPhaseText;
			turnPhaseText.text = state switch
			{
				BattleState.HeroSelect => "[드래프트 1/3] 용사 토큰 선택", 
				BattleState.SaintSelect => "[드래프트 2/3] 성녀 토큰 선택", 
				BattleState.BagChoiceRequired => "[드래프트 3/3] 어느 백에서 뽑을까요?", 
				BattleState.FreeSelect => "[드래프트 3/3] 토큰 선택", 
				BattleState.SynergyCheck => "시너지 판정 중...", 
				BattleState.EnemyFirstStrike => "선공형 선제 공격!", 
				BattleState.AllyTurn => "아군 공격!", 
				BattleState.EnemyTurn => "적 행동 중...", 
				BattleState.TurnEnd => "턴 종료", 
				BattleState.BattleVictory => "전투 승리! 다음 전투 준비 중...", 
				BattleState.GameClear => "★ 최종 클리어 ★", 
				BattleState.GameOver => "전투 패배...", 
				_ => "", 
			};
			if (state == BattleState.BagChoiceRequired)
			{
				BagChoiceHeroCount.text = $"용사 백 ({Battle.Hero.Bag.Count})";
				BagChoiceSaintCount.text = $"성녀 백 ({Battle.Saint.Bag.Count})";
			}
			OnRefresh();
		}

		public void OnDraftChoice(BattleState step, List<Token> drawn)
		{
			SetupTokenButton(TokenChoiceA, (drawn.Count > 0) ? drawn[0] : null, 0, step);
			SetupTokenButton(TokenChoiceB, (drawn.Count > 1) ? drawn[1] : null, 1, step);
		}

		private void SetupTokenButton(TokenButton btn, Token token, int index, BattleState step)
		{
			bool flag = token != null;
			btn.gameObject.SetActive(flag);
			if (flag)
			{
				btn.Setup(Battle, token, index, step);
			}
		}

		public void OnBagChoiceRequired()
		{
		}

		public void OnSynergyResult(SynergyResult synergy, int damage)
		{
			SynergyText.text = (synergy.HasAnySynergy ? $"{synergy.BuildDescription()}\n예상 데미지: {damage}" : $"시너지 없음\n예상 데미지: {damage}");
		}

		public void OnRefresh()
		{
			if (Battle == null)
			{
				return;
			}
			CharacterData hero = Battle.Hero;
			CharacterData saint = Battle.Saint;
			Enemy currentEnemy = Battle.CurrentEnemy;
			if (hero != null)
			{
				HeroHPSlider.value = (float)hero.CurrentLP / (float)hero.MaxLP;
				HeroHPText.text = $"용사 LP: {hero.CurrentLP}/{hero.MaxLP}";
				HeroStatusText.text = (hero.IsAlive ? "" : "전사");
				UltimateGaugeBar?.SetValue(hero.GaugePercent);
				Dictionary<TokenType, int> composition = hero.Bag.GetComposition();
				int value;
				int num = (composition.TryGetValue(TokenType.Slash, out value) ? value : 0);
				int value2;
				int num2 = (composition.TryGetValue(TokenType.Pierce, out value2) ? value2 : 0);
				int value3;
				int num3 = (composition.TryGetValue(TokenType.Bash, out value3) ? value3 : 0);
				int value4;
				int num4 = (composition.TryGetValue(TokenType.Ultimate, out value4) ? value4 : 0);
				string text = ((num4 > 0) ? $" 필{num4}" : "");
				HeroBagCountText.text = $"용사 백: {hero.Bag.Count}\n참{num} / 관{num2} / 타{num3}{text}";
			}
			if (saint != null)
			{
				SaintHPSlider.value = (float)saint.CurrentLP / (float)saint.MaxLP;
				SaintHPText.text = $"성녀 얀 LP: {saint.CurrentLP}/{saint.MaxLP}";
				Dictionary<TokenType, int> composition2 = saint.Bag.GetComposition();
				int value5;
				int num5 = (composition2.TryGetValue(TokenType.Emotion, out value5) ? value5 : 0);
				int value6;
				int num6 = (composition2.TryGetValue(TokenType.Skill, out value6) ? value6 : 0);
				int value7;
				int num7 = (composition2.TryGetValue(TokenType.Passion, out value7) ? value7 : 0);
				SaintBagCountText.text = $"성녀 백: {saint.Bag.Count}\n감{num5} / 기{num6} / 격{num7}";
			}
			if (currentEnemy != null)
			{
				EnemyNameText.text = currentEnemy.EnemyName;
				EnemyHPSlider.value = ((currentEnemy.MaxHP > 0) ? ((float)currentEnemy.CurrentHP / (float)currentEnemy.MaxHP) : 0f);
				EnemyHPText.text = $"{currentEnemy.EnemyName} HP: {currentEnemy.CurrentHP}/{currentEnemy.MaxHP}";
			}
			if (Battle.SelectedTokens.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder("선택: ");
				foreach (Token selectedToken in Battle.SelectedTokens)
				{
					stringBuilder.Append($"[{selectedToken}] ");
				}
				SelectedTokensText.text = stringBuilder.ToString();
			}
			else
			{
				SelectedTokensText.text = "선택: -";
			}
			if (Battle.SelectedTokens.Count == 0)
			{
				SynergyText.text = "";
			}
			bool active = Battle.CurrentState == BattleState.HeroSelect || Battle.CurrentState == BattleState.SaintSelect || Battle.CurrentState == BattleState.FreeSelect;
			TokenDrawPanel.SetActive(active);
		}

		public void OnLog(string message)
		{
			AddLog(message);
		}

		public void OnBattleProgress(int current, int total)
		{
			BattleProgressText.text = $"전투 {current}/{total}";
		}

		public void OnGameOver(bool cleared)
		{
			GameOverPanel.SetActive(value: true);
			GameOverText.text = (cleared ? "★ 클리어! ★" : "전투 패배...");
			GameOverText.color = (cleared ? Color.yellow : Color.red);
			if (GameOverLogText != null)
			{
				GameOverLogText.text = _log.ToString();
				if (GameOverLogScroll != null)
				{
					StartCoroutine(ScrollToBottom(GameOverLogScroll));
				}
			}
		}

		private IEnumerator ScrollToBottom(ScrollRect sr)
		{
			yield return null;
			Canvas.ForceUpdateCanvases();
			sr.verticalNormalizedPosition = 0f;
		}

		private void ShowBagBuilder(bool show)
		{
			BagBuilderPanel.SetActive(show);
			if (show)
			{
				BattlePanel.SetActive(value: false);
				GameOverPanel.SetActive(value: false);
			}
		}

		private void AddLog(string msg)
		{
			_log.AppendLine(msg);
			LogText.text = _log.ToString();
			Canvas.ForceUpdateCanvases();
			LogScrollRect.verticalNormalizedPosition = 0f;
		}
	}
}

using CombatPrototype.Combat;
using CombatPrototype.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(BattleManager))]
public class BattleSceneBootstrap : MonoBehaviour
{
	private BattleManager _battle;

	private BattleUI _ui;

	private Canvas _canvas;

	private TMP_FontAsset _font;

	private static readonly Color BG_DARK = new Color(0.08f, 0.08f, 0.12f);

	private static readonly Color BG_MID = new Color(0.13f, 0.13f, 0.2f);

	private static readonly Color BG_PANEL = new Color(0.17f, 0.17f, 0.27f, 0.97f);

	private static readonly Color ACCENT = new Color(0.9f, 0.7f, 0.2f);

	private static readonly Color HP_GREEN = new Color(0.2f, 0.78f, 0.3f);

	private static readonly Color HP_RED = new Color(0.8f, 0.2f, 0.2f);

	private static readonly Color GAUGE_COL = new Color(0.75f, 0.45f, 1f);

	private static readonly Color HERO_COL = new Color(0.4f, 0.7f, 1f);

	private static readonly Color SAINT_COL = new Color(1f, 0.55f, 0.7f);

	private void Awake()
	{
		_battle = GetComponent<BattleManager>();
		_font = Resources.Load<TMP_FontAsset>("Fonts & Materials/DNFBitBitv2 SDF");
		if (_font == null)
		{
			Debug.LogWarning("[Bootstrap] DNFBitBitv2 SDF 폰트 없음 – 기본 폰트 사용");
		}
		SetupCamera();
		SetupCanvas();
		_ui = BuildUI();
		Enemy[] enemies = CreateEnemies();
		_battle.RegisterEnemies(enemies);
		_battle.RegisterObserver(_ui);
		_ui.Battle = _battle;
		_ui.Initialize();
	}

	private void SetupCamera()
	{
		Camera.main.backgroundColor = BG_DARK;
		Camera.main.clearFlags = CameraClearFlags.Color;
	}

	private void SetupCanvas()
	{
		if (Object.FindFirstObjectByType<EventSystem>() == null)
		{
			GameObject obj = new GameObject("EventSystem");
			obj.AddComponent<EventSystem>();
			obj.AddComponent<StandaloneInputModule>();
		}
		GameObject gameObject = new GameObject("Canvas");
		_canvas = gameObject.AddComponent<Canvas>();
		_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
		canvasScaler.matchWidthOrHeight = 0.5f;
		gameObject.AddComponent<GraphicRaycaster>();
	}

	private Enemy[] CreateEnemies()
	{
		GameObject gameObject = new GameObject("Enemy_Growth");
		GrowthEnemy growthEnemy = gameObject.AddComponent<GrowthEnemy>();
		GameObject gameObject2 = new GameObject("Enemy_BagDestroy");
		BagDestroyerEnemy bagDestroyerEnemy = gameObject2.AddComponent<BagDestroyerEnemy>();
		GameObject obj = new GameObject("Enemy_FirstStrike");
		FirstStrikeEnemy firstStrikeEnemy = obj.AddComponent<FirstStrikeEnemy>();
		gameObject.SetActive(value: false);
		gameObject2.SetActive(value: false);
		obj.SetActive(value: false);
		return new Enemy[3] { growthEnemy, bagDestroyerEnemy, firstStrikeEnemy };
	}

	private BattleUI BuildUI()
	{
		BattleUI battleUI = new GameObject("BattleUI").AddComponent<BattleUI>();
		BuildBagBuilderPanel(battleUI);
		BuildBattlePanel(battleUI);
		BuildGameOverPanel(battleUI);
		return battleUI;
	}

	private void BuildBagBuilderPanel(BattleUI ui)
	{
		GameObject gameObject = MakePanel("BagBuilderPanel", _canvas.transform, V2(0f, 0f), V2(1f, 1f));
		SetColor(gameObject, BG_DARK);
		ui.BagBuilderPanel = gameObject;
		Transform transform = gameObject.transform;
		MakeText("Title", transform, "용사 + 성녀 얀 – 전투 시작", V2(0f, 0.88f), V2(1f, 1f), 48f, ACCENT, TextAlignmentOptions.Center);
		MakeText("Sub", transform, "각 백을 정확히 10개로 구성하세요", V2(0f, 0.82f), V2(1f, 0.9f), 26f, Color.white, TextAlignmentOptions.Center);
		BuildBagSection(transform, ui, "★ 용사 백 (참격 / 관통 / 타격)", V2(0.03f, 0.58f), V2(0.48f, 0.65f), "참격", V2(0.03f, 0.49f), V2(0.48f, 0.58f), "관통", V2(0.03f, 0.39f), V2(0.48f, 0.48f), "타격", V2(0.03f, 0.29f), V2(0.48f, 0.38f), V2(0.03f, 0.22f), V2(0.48f, 0.29f), isHero: true);
		BuildBagSection(transform, ui, "♪ 성녀 백 (감성 / 기교 / 격정)", V2(0.52f, 0.58f), V2(0.97f, 0.65f), "감성", V2(0.52f, 0.49f), V2(0.97f, 0.58f), "기교", V2(0.52f, 0.39f), V2(0.97f, 0.48f), "격정", V2(0.52f, 0.29f), V2(0.97f, 0.38f), V2(0.52f, 0.22f), V2(0.97f, 0.29f), isHero: false);
		ui.BagValidationText = MakeText("Validation", transform, "", V2(0f, 0.15f), V2(1f, 0.22f), 26f, Color.yellow, TextAlignmentOptions.Center);
		GameObject gameObject2 = MakeButton("전투 시작!", transform, V2(0.35f, 0.05f), V2(0.65f, 0.14f), ACCENT, 36f);
		ui.StartGameButton = gameObject2.GetComponent<Button>();
		MakeButton("게임 종료", transform, V2(0.8f, 0.02f), V2(0.98f, 0.08f), new Color(0.4f, 0.4f, 0.4f), 22f).GetComponent<Button>().onClick.AddListener(QuitGame);
	}

	private void BuildBagSection(Transform pt, BattleUI ui, string label, Vector2 headerMin, Vector2 headerMax, string row1Label, Vector2 row1Min, Vector2 row1Max, string row2Label, Vector2 row2Min, Vector2 row2Max, string row3Label, Vector2 row3Min, Vector2 row3Max, Vector2 totalMin, Vector2 totalMax, bool isHero)
	{
		Color color = (isHero ? HERO_COL : SAINT_COL);
		MakeText(label, pt, label, headerMin, headerMax, 28f, color, TextAlignmentOptions.Center);
		if (isHero)
		{
			ui.HeroSlashSlider = MakeSliderWithLabel(pt, row1Label, row1Min, row1Max, color, out ui.HeroSlashVal, 0, 10, 4);
			ui.HeroPierceSlider = MakeSliderWithLabel(pt, row2Label, row2Min, row2Max, color, out ui.HeroPierceVal, 0, 10, 3);
			ui.HeroBashSlider = MakeSliderWithLabel(pt, row3Label, row3Min, row3Max, color, out ui.HeroBashVal, 0, 10, 3);
			ui.HeroTotalText = MakeText("HeroTotal", pt, "용사 합계: 10/10", totalMin, totalMax, 24f, color, TextAlignmentOptions.Center);
		}
		else
		{
			ui.SaintEmotionSlider = MakeSliderWithLabel(pt, row1Label, row1Min, row1Max, color, out ui.SaintEmotionVal, 0, 10, 4);
			ui.SaintSkillSlider = MakeSliderWithLabel(pt, row2Label, row2Min, row2Max, color, out ui.SaintSkillVal, 0, 10, 3);
			ui.SaintPassionSlider = MakeSliderWithLabel(pt, row3Label, row3Min, row3Max, color, out ui.SaintPassionVal, 0, 10, 3);
			ui.SaintTotalText = MakeText("SaintTotal", pt, "성녀 합계: 10/10", totalMin, totalMax, 24f, color, TextAlignmentOptions.Center);
		}
	}

	private Slider MakeSliderWithLabel(Transform parent, string label, Vector2 aMin, Vector2 aMax, Color col, out TextMeshProUGUI valText, int min, int max, int defaultVal)
	{
		GameObject gameObject = MakePanel("Row_" + label, parent, aMin, aMax);
		SetColor(gameObject, new Color(0f, 0f, 0f, 0f));
		Transform parent2 = gameObject.transform;
		MakeText("LabelText", parent2, label, V2(0f, 0f), V2(0.2f, 1f), 24f, col, TextAlignmentOptions.MidlineRight);
		Slider result = MakeInteractiveSlider("Slider_" + label, parent2, V2(0.22f, 0.1f), V2(0.78f, 0.9f), col, min, max, defaultVal);
		valText = MakeText("ValText", parent2, defaultVal.ToString(), V2(0.8f, 0f), V2(1f, 1f), 24f, Color.white, TextAlignmentOptions.MidlineLeft);
		return result;
	}

	private void BuildBattlePanel(BattleUI ui)
	{
		GameObject gameObject = MakePanel("BattlePanel", _canvas.transform, V2(0f, 0f), V2(1f, 1f));
		SetColor(gameObject, BG_DARK);
		ui.BattlePanel = gameObject;
		Transform transform = gameObject.transform;
		ui.BattleProgressText = MakeText("BattleProgress", transform, "전투 1/3", V2(0f, 0.94f), V2(0.3f, 1f), 26f, ACCENT, TextAlignmentOptions.Center);
		ui.TurnPhaseText = MakeText("TurnPhase", transform, "드래프트 중", V2(0.3f, 0.94f), V2(1f, 1f), 30f, Color.white, TextAlignmentOptions.Center);
		BuildEnemyBar(transform, ui);
		BuildHeroPanel(transform, ui);
		BuildSaintPanel(transform, ui);
		BuildCenterArea(transform, ui);
		BuildLogArea(transform, ui);
	}

	private void BuildEnemyBar(Transform pt, BattleUI ui)
	{
		GameObject gameObject = MakePanel("EnemyBar", pt, V2(0f, 0.85f), V2(1f, 0.94f));
		SetColor(gameObject, BG_PANEL);
		ui.EnemyNameText = MakeText("EnemyName", gameObject.transform, "적", V2(0f, 0f), V2(0.25f, 1f), 26f, HP_RED, TextAlignmentOptions.Center);
		ui.EnemyHPText = MakeText("EnemyHP", gameObject.transform, "HP: -/-", V2(0.25f, 0f), V2(0.55f, 1f), 22f, Color.white, TextAlignmentOptions.Center);
		ui.EnemyHPSlider = MakeSlider("EnemyHPSlider", gameObject.transform, V2(0.55f, 0.15f), V2(0.99f, 0.85f), HP_RED);
	}

	private void BuildHeroPanel(Transform pt, BattleUI ui)
	{
		GameObject gameObject = MakePanel("HeroPanel", pt, V2(0f, 0.18f), V2(0.2f, 0.85f));
		SetColor(gameObject, BG_PANEL);
		Transform parent = gameObject.transform;
		MakeText("HeroTitle", parent, "용사", V2(0f, 0.9f), V2(1f, 1f), 28f, HERO_COL, TextAlignmentOptions.Center);
		ui.HeroHPText = MakeText("HeroHP", parent, "LP: 50/50", V2(0f, 0.78f), V2(1f, 0.9f), 20f, Color.white, TextAlignmentOptions.Center);
		ui.HeroHPSlider = MakeSlider("HeroHPSlider", parent, V2(0.05f, 0.7f), V2(0.95f, 0.78f), HP_GREEN);
		ui.HeroBagCountText = MakeText("HeroBag", parent, "용사 백: 10", V2(0f, 0.55f), V2(1f, 0.7f), 18f, Color.white, TextAlignmentOptions.Center);
		ui.HeroStatusText = MakeText("HeroStatus", parent, "", V2(0f, 0.47f), V2(1f, 0.55f), 20f, HP_RED, TextAlignmentOptions.Center);
		MakeText("GaugeLabel", parent, "필살기 게이지", V2(0f, 0.36f), V2(1f, 0.47f), 16f, GAUGE_COL, TextAlignmentOptions.Center);
		GameObject obj = new GameObject("GaugeBar");
		obj.transform.SetParent(parent, worldPositionStays: false);
		GaugeBar gaugeBar = obj.AddComponent<GaugeBar>();
		gaugeBar.Slider = MakeSlider("GaugeSlider", parent, V2(0.05f, 0.26f), V2(0.95f, 0.36f), GAUGE_COL);
		gaugeBar.PercentText = MakeText("GaugePct", parent, "0%", V2(0f, 0.16f), V2(1f, 0.26f), 16f, GAUGE_COL, TextAlignmentOptions.Center);
		ui.UltimateGaugeBar = gaugeBar;
	}

	private void BuildSaintPanel(Transform pt, BattleUI ui)
	{
		GameObject gameObject = MakePanel("SaintPanel", pt, V2(0.8f, 0.18f), V2(1f, 0.85f));
		SetColor(gameObject, BG_PANEL);
		Transform parent = gameObject.transform;
		MakeText("SaintTitle", parent, "성녀 얀", V2(0f, 0.9f), V2(1f, 1f), 26f, SAINT_COL, TextAlignmentOptions.Center);
		ui.SaintHPText = MakeText("SaintHP", parent, "LP: 50/50", V2(0f, 0.78f), V2(1f, 0.9f), 20f, Color.white, TextAlignmentOptions.Center);
		ui.SaintHPSlider = MakeSlider("SaintHPSlider", parent, V2(0.05f, 0.7f), V2(0.95f, 0.78f), SAINT_COL);
		ui.SaintBagCountText = MakeText("SaintBag", parent, "성녀 백: 10", V2(0f, 0.55f), V2(1f, 0.7f), 18f, Color.white, TextAlignmentOptions.Center);
	}

	private void BuildCenterArea(Transform pt, BattleUI ui)
	{
		GameObject gameObject = MakePanel("CenterArea", pt, V2(0.2f, 0.18f), V2(0.8f, 0.85f));
		SetColor(gameObject, BG_MID);
		Transform transform = gameObject.transform;
		ui.SelectedTokensText = MakeText("SelectedTokens", transform, "선택: -", V2(0f, 0.88f), V2(1f, 1f), 26f, Color.white, TextAlignmentOptions.Center);
		ui.SynergyText = MakeText("SynergyText", transform, "", V2(0f, 0.75f), V2(1f, 0.88f), 22f, ACCENT, TextAlignmentOptions.Center);
		BuildTokenDrawPanel(transform, ui);
		BuildBagChoicePanel(transform, ui);
	}

	private void BuildTokenDrawPanel(Transform ct, BattleUI ui)
	{
		GameObject gameObject = MakePanel("TokenDrawPanel", ct, V2(0.03f, 0.2f), V2(0.97f, 0.73f));
		SetColor(gameObject, new Color(0f, 0f, 0f, 0f));
		ui.TokenDrawPanel = gameObject;
		ui.TokenChoiceA = BuildTokenButton("TokenA", gameObject.transform, V2(0.02f, 0.02f), V2(0.48f, 0.98f));
		ui.TokenChoiceB = BuildTokenButton("TokenB", gameObject.transform, V2(0.52f, 0.02f), V2(0.98f, 0.98f));
		gameObject.SetActive(value: false);
	}

	private TokenButton BuildTokenButton(string name, Transform parent, Vector2 aMin, Vector2 aMax)
	{
		GameObject gameObject = MakePanel(name, parent, aMin, aMax);
		SetColor(gameObject, new Color(0.25f, 0.25f, 0.35f));
		Button button = gameObject.AddComponent<Button>();
		TokenButton tokenButton = gameObject.AddComponent<TokenButton>();
		tokenButton.Button = button;
		GameObject obj = new GameObject("CardImage");
		obj.transform.SetParent(gameObject.transform, worldPositionStays: false);
		Image image = obj.AddComponent<Image>();
		image.preserveAspect = true;
		RectTransform component = obj.GetComponent<RectTransform>();
		component.anchorMin = V2(0.05f, 0.28f);
		component.anchorMax = V2(0.95f, 0.98f);
		component.offsetMin = Vector2.zero;
		component.offsetMax = Vector2.zero;
		tokenButton.CardImage = image;
		GameObject obj2 = new GameObject("NameLabel");
		obj2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		TextMeshProUGUI textMeshProUGUI = obj2.AddComponent<TextMeshProUGUI>();
		if (_font != null)
		{
			textMeshProUGUI.font = _font;
		}
		textMeshProUGUI.enableAutoSizing = true;
		textMeshProUGUI.fontSizeMin = 12f;
		textMeshProUGUI.fontSizeMax = 22f;
		textMeshProUGUI.color = Color.white;
		textMeshProUGUI.alignment = TextAlignmentOptions.Center;
		RectTransform component2 = textMeshProUGUI.GetComponent<RectTransform>();
		component2.anchorMin = V2(0f, 0.13f);
		component2.anchorMax = V2(1f, 0.28f);
		component2.offsetMin = Vector2.zero;
		component2.offsetMax = Vector2.zero;
		tokenButton.NameLabel = textMeshProUGUI;
		GameObject obj3 = new GameObject("EffectLabel");
		obj3.transform.SetParent(gameObject.transform, worldPositionStays: false);
		TextMeshProUGUI textMeshProUGUI2 = obj3.AddComponent<TextMeshProUGUI>();
		if (_font != null)
		{
			textMeshProUGUI2.font = _font;
		}
		textMeshProUGUI2.fontSize = 16f;
		textMeshProUGUI2.color = new Color(0.8f, 0.9f, 1f);
		textMeshProUGUI2.alignment = TextAlignmentOptions.Center;
		textMeshProUGUI2.textWrappingMode = TextWrappingModes.Normal;
		RectTransform component3 = textMeshProUGUI2.GetComponent<RectTransform>();
		component3.anchorMin = V2(0f, 0f);
		component3.anchorMax = V2(1f, 0.13f);
		component3.offsetMin = Vector2.zero;
		component3.offsetMax = Vector2.zero;
		tokenButton.EffectLabel = textMeshProUGUI2;
		return tokenButton;
	}

	private void BuildBagChoicePanel(Transform ct, BattleUI ui)
	{
		GameObject gameObject = MakePanel("BagChoicePanel", ct, V2(0.05f, 0.25f), V2(0.95f, 0.72f));
		SetColor(gameObject, new Color(0f, 0f, 0f, 0f));
		ui.BagChoicePanel = gameObject;
		MakeText("BagChoiceTitle", gameObject.transform, "어느 백에서 뽑을까요?", V2(0f, 0.75f), V2(1f, 1f), 30f, Color.white, TextAlignmentOptions.Center);
		GameObject gameObject2 = MakeButton("용사 백", gameObject.transform, V2(0.05f, 0.1f), V2(0.45f, 0.72f), HERO_COL, 32f);
		GameObject gameObject3 = MakeButton("성녀 백", gameObject.transform, V2(0.55f, 0.1f), V2(0.95f, 0.72f), SAINT_COL, 32f);
		ui.ChooseHeroBagButton = gameObject2.GetComponent<Button>();
		ui.ChooseSaintBagButton = gameObject3.GetComponent<Button>();
		ui.BagChoiceHeroCount = MakeText("HeroCount", gameObject2.transform, "", V2(0f, 0f), V2(1f, 0.3f), 20f, Color.white, TextAlignmentOptions.Center);
		ui.BagChoiceSaintCount = MakeText("SaintCount", gameObject3.transform, "", V2(0f, 0f), V2(1f, 0.3f), 20f, Color.white, TextAlignmentOptions.Center);
		gameObject.SetActive(value: false);
	}

	private void BuildLogArea(Transform pt, BattleUI ui)
	{
		GameObject gameObject = MakePanel("LogPanel", pt, V2(0f, 0f), V2(1f, 0.18f));
		SetColor(gameObject, BG_PANEL);
		GameObject gameObject2 = new GameObject("LogScroll");
		gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		ScrollRect scrollRect = gameObject2.AddComponent<ScrollRect>();
		RectTransform component = gameObject2.GetComponent<RectTransform>();
		component.anchorMin = V2(0f, 0f);
		component.anchorMax = V2(1f, 1f);
		Vector2 offsetMin = (component.offsetMax = Vector2.zero);
		component.offsetMin = offsetMin;
		GameObject gameObject3 = new GameObject("Viewport");
		gameObject3.transform.SetParent(gameObject2.transform, worldPositionStays: false);
		gameObject3.AddComponent<RectMask2D>();
		RectTransform component2 = gameObject3.GetComponent<RectTransform>();
		component2.anchorMin = Vector2.zero;
		component2.anchorMax = Vector2.one;
		offsetMin = (component2.offsetMax = Vector2.zero);
		component2.offsetMin = offsetMin;
		GameObject gameObject4 = new GameObject("Content");
		gameObject4.transform.SetParent(gameObject3.transform, worldPositionStays: false);
		gameObject4.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		VerticalLayoutGroup verticalLayoutGroup = gameObject4.AddComponent<VerticalLayoutGroup>();
		verticalLayoutGroup.childForceExpandWidth = true;
		verticalLayoutGroup.childForceExpandHeight = false;
		RectTransform component3 = gameObject4.GetComponent<RectTransform>();
		component3.anchorMin = V2(0f, 1f);
		component3.anchorMax = V2(1f, 1f);
		component3.pivot = V2(0.5f, 1f);
		offsetMin = (component3.offsetMax = Vector2.zero);
		component3.offsetMin = offsetMin;
		GameObject obj = new GameObject("LogText");
		obj.transform.SetParent(gameObject4.transform, worldPositionStays: false);
		TextMeshProUGUI textMeshProUGUI = obj.AddComponent<TextMeshProUGUI>();
		if (_font != null)
		{
			textMeshProUGUI.font = _font;
		}
		textMeshProUGUI.fontSize = 16f;
		textMeshProUGUI.color = new Color(0.85f, 0.85f, 0.85f);
		textMeshProUGUI.alignment = TextAlignmentOptions.TopLeft;
		textMeshProUGUI.textWrappingMode = TextWrappingModes.Normal;
		scrollRect.content = component3;
		scrollRect.viewport = component2;
		scrollRect.horizontal = false;
		scrollRect.vertical = true;
		ui.LogScrollRect = scrollRect;
		ui.LogText = textMeshProUGUI;
	}

	private void BuildGameOverPanel(BattleUI ui)
	{
		GameObject gameObject = MakePanel("GameOverPanel", _canvas.transform, V2(0.15f, 0.1f), V2(0.85f, 0.9f));
		SetColor(gameObject, new Color(0.08f, 0.08f, 0.12f, 0.97f));
		gameObject.SetActive(value: false);
		ui.GameOverPanel = gameObject;
		ui.GameOverText = MakeText("GameOverText", gameObject.transform, "결과", V2(0f, 0.86f), V2(1f, 0.98f), 48f, Color.white, TextAlignmentOptions.Center);
		GameObject gameObject2 = MakeButton("다시 시작", gameObject.transform, V2(0.25f, 0.78f), V2(0.75f, 0.87f), ACCENT, 30f);
		ui.RestartButton = gameObject2.GetComponent<Button>();
		MakeButton("게임 종료", gameObject.transform, V2(0.25f, 0.72f), V2(0.75f, 0.78f), new Color(0.4f, 0.4f, 0.4f), 20f).GetComponent<Button>().onClick.AddListener(QuitGame);
		BuildGameOverLogArea(gameObject.transform, ui);
	}

	private void BuildGameOverLogArea(Transform pt, BattleUI ui)
	{
		GameObject gameObject = MakePanel("GameOverLogBG", pt, V2(0.02f, 0.01f), V2(0.98f, 0.71f));
		SetColor(gameObject, new Color(0.05f, 0.05f, 0.1f, 0.95f));
		MakeText("LogTitle", gameObject.transform, "전투 로그", V2(0f, 0.94f), V2(1f, 1f), 16f, ACCENT, TextAlignmentOptions.Center);
		GameObject gameObject2 = new GameObject("GOLogScroll");
		gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		ScrollRect scrollRect = gameObject2.AddComponent<ScrollRect>();
		RectTransform component = gameObject2.GetComponent<RectTransform>();
		component.anchorMin = V2(0f, 0f);
		component.anchorMax = V2(1f, 0.93f);
		Vector2 offsetMin = (component.offsetMax = Vector2.zero);
		component.offsetMin = offsetMin;
		GameObject gameObject3 = new GameObject("Viewport");
		gameObject3.transform.SetParent(gameObject2.transform, worldPositionStays: false);
		gameObject3.AddComponent<RectMask2D>();
		RectTransform component2 = gameObject3.GetComponent<RectTransform>();
		component2.anchorMin = Vector2.zero;
		component2.anchorMax = Vector2.one;
		offsetMin = (component2.offsetMax = Vector2.zero);
		component2.offsetMin = offsetMin;
		GameObject gameObject4 = new GameObject("Content");
		gameObject4.transform.SetParent(gameObject3.transform, worldPositionStays: false);
		gameObject4.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		VerticalLayoutGroup verticalLayoutGroup = gameObject4.AddComponent<VerticalLayoutGroup>();
		verticalLayoutGroup.childForceExpandWidth = true;
		verticalLayoutGroup.childForceExpandHeight = false;
		RectTransform component3 = gameObject4.GetComponent<RectTransform>();
		component3.anchorMin = V2(0f, 1f);
		component3.anchorMax = V2(1f, 1f);
		component3.pivot = V2(0.5f, 1f);
		offsetMin = (component3.offsetMax = Vector2.zero);
		component3.offsetMin = offsetMin;
		GameObject obj = new GameObject("GOLogText");
		obj.transform.SetParent(gameObject4.transform, worldPositionStays: false);
		TextMeshProUGUI textMeshProUGUI = obj.AddComponent<TextMeshProUGUI>();
		if (_font != null)
		{
			textMeshProUGUI.font = _font;
		}
		textMeshProUGUI.fontSize = 14f;
		textMeshProUGUI.color = new Color(0.8f, 0.8f, 0.8f);
		textMeshProUGUI.alignment = TextAlignmentOptions.TopLeft;
		textMeshProUGUI.textWrappingMode = TextWrappingModes.Normal;
		scrollRect.content = component3;
		scrollRect.viewport = component2;
		scrollRect.horizontal = false;
		scrollRect.vertical = true;
		ui.GameOverLogScroll = scrollRect;
		ui.GameOverLogText = textMeshProUGUI;
	}

	private static Vector2 V2(float x, float y)
	{
		return new Vector2(x, y);
	}

	private GameObject MakePanel(string name, Transform parent, Vector2 aMin, Vector2 aMax)
	{
		GameObject obj = new GameObject(name);
		obj.transform.SetParent(parent, worldPositionStays: false);
		obj.AddComponent<Image>();
		RectTransform component = obj.GetComponent<RectTransform>();
		component.anchorMin = aMin;
		component.anchorMax = aMax;
		Vector2 offsetMin = (component.offsetMax = Vector2.zero);
		component.offsetMin = offsetMin;
		return obj;
	}

	private TextMeshProUGUI MakeText(string name, Transform parent, string text, Vector2 aMin, Vector2 aMax, float size, Color color, TextAlignmentOptions align)
	{
		GameObject obj = new GameObject(name);
		obj.transform.SetParent(parent, worldPositionStays: false);
		TextMeshProUGUI textMeshProUGUI = obj.AddComponent<TextMeshProUGUI>();
		if (_font != null)
		{
			textMeshProUGUI.font = _font;
		}
		textMeshProUGUI.text = text;
		textMeshProUGUI.fontSize = size;
		textMeshProUGUI.color = color;
		textMeshProUGUI.alignment = align;
		textMeshProUGUI.textWrappingMode = TextWrappingModes.Normal;
		RectTransform component = textMeshProUGUI.GetComponent<RectTransform>();
		component.anchorMin = aMin;
		component.anchorMax = aMax;
		Vector2 offsetMin = (component.offsetMax = Vector2.zero);
		component.offsetMin = offsetMin;
		return textMeshProUGUI;
	}

	private Slider MakeSlider(string name, Transform parent, Vector2 aMin, Vector2 aMax, Color fill)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		Slider slider = gameObject.AddComponent<Slider>();
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = aMin;
		component.anchorMax = aMax;
		Vector2 offsetMin = (component.offsetMax = Vector2.zero);
		component.offsetMin = offsetMin;
		GameObject obj = new GameObject("BG");
		obj.transform.SetParent(gameObject.transform, worldPositionStays: false);
		Image image = obj.AddComponent<Image>();
		image.color = new Color(0.2f, 0.2f, 0.2f);
		StretchRT(image.GetComponent<RectTransform>());
		GameObject gameObject2 = new GameObject("FillArea");
		gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		StretchRT(gameObject2.AddComponent<RectTransform>());
		GameObject gameObject3 = new GameObject("Fill");
		gameObject3.transform.SetParent(gameObject2.transform, worldPositionStays: false);
		Image image2 = gameObject3.AddComponent<Image>();
		image2.color = fill;
		StretchRT(image2.GetComponent<RectTransform>());
		slider.fillRect = gameObject3.GetComponent<RectTransform>();
		slider.targetGraphic = image;
		slider.direction = Slider.Direction.LeftToRight;
		slider.minValue = 0f;
		slider.maxValue = 1f;
		slider.value = 1f;
		slider.interactable = false;
		return slider;
	}

	private Slider MakeInteractiveSlider(string name, Transform parent, Vector2 aMin, Vector2 aMax, Color fill, int min, int max, int def)
	{
		Slider slider = MakeSlider(name, parent, aMin, aMax, fill);
		slider.interactable = true;
		slider.wholeNumbers = true;
		slider.minValue = min;
		slider.maxValue = max;
		slider.value = def;
		GameObject gameObject = new GameObject("HandleSlideArea");
		gameObject.transform.SetParent(slider.transform, worldPositionStays: false);
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.offsetMin = new Vector2(8f, 0f);
		rectTransform.offsetMax = new Vector2(-8f, 0f);
		GameObject obj = new GameObject("Handle");
		obj.transform.SetParent(gameObject.transform, worldPositionStays: false);
		Image image = obj.AddComponent<Image>();
		image.color = Color.white;
		RectTransform component = obj.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(20f, 0f);
		Vector2 anchorMin = (component.anchorMax = V2(0.5f, 0.5f));
		component.anchorMin = anchorMin;
		slider.handleRect = component;
		slider.targetGraphic = image;
		return slider;
	}

	private GameObject MakeButton(string label, Transform parent, Vector2 aMin, Vector2 aMax, Color bg, float fontSize)
	{
		GameObject gameObject = new GameObject("Btn_" + label);
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		gameObject.AddComponent<Image>().color = bg;
		gameObject.AddComponent<Button>();
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = aMin;
		component.anchorMax = aMax;
		Vector2 offsetMin = (component.offsetMax = Vector2.zero);
		component.offsetMin = offsetMin;
		GameObject obj = new GameObject("Text");
		obj.transform.SetParent(gameObject.transform, worldPositionStays: false);
		TextMeshProUGUI textMeshProUGUI = obj.AddComponent<TextMeshProUGUI>();
		if (_font != null)
		{
			textMeshProUGUI.font = _font;
		}
		textMeshProUGUI.text = label;
		textMeshProUGUI.fontSize = fontSize;
		textMeshProUGUI.color = Color.white;
		textMeshProUGUI.alignment = TextAlignmentOptions.Center;
		textMeshProUGUI.textWrappingMode = TextWrappingModes.Normal;
		StretchRT(textMeshProUGUI.GetComponent<RectTransform>(), new Vector2(4f, 4f), new Vector2(-4f, -4f));
		return gameObject;
	}

	private void SetColor(GameObject go, Color c)
	{
		Image component = go.GetComponent<Image>();
		if ((bool)component)
		{
			component.color = c;
		}
	}

	private static void StretchRT(RectTransform rt, Vector2? offsetMin = null, Vector2? offsetMax = null)
	{
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.offsetMin = offsetMin ?? Vector2.zero;
		rt.offsetMax = offsetMax ?? Vector2.zero;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			QuitGame();
		}
	}

	private void QuitGame()
	{
		Application.Quit();
	}
}

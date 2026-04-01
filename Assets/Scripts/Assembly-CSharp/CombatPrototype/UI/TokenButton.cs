using CombatPrototype.Combat;
using CombatPrototype.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CombatPrototype.UI
{
	public class TokenButton : MonoBehaviour
	{
		public Button Button;

		public TextMeshProUGUI NameLabel;

		public TextMeshProUGUI EffectLabel;

		public Image CardImage;

		private BattleManager _battle;

		private int _index;

		private BattleState _step;

		public void Setup(BattleManager battle, Token token, int index, BattleState step)
		{
			_battle = battle;
			_index = index;
			_step = step;
			NameLabel.text = token.ToString();
			if (EffectLabel != null)
			{
				EffectLabel.text = Token.GetEffectDescription(token.Type);
			}
			if (CardImage != null)
			{
				string spriteName = Token.GetSpriteName(token.Type);
				Sprite sprite = null;
				if (!string.IsNullOrEmpty(spriteName))
				{
					sprite = Resources.Load<Sprite>("Tokens/" + spriteName);
				}
				if (sprite != null)
				{
					CardImage.sprite = sprite;
					CardImage.color = Color.white;
					CardImage.preserveAspect = true;
				}
				else
				{
					CardImage.sprite = null;
					CardImage.preserveAspect = false;
					Image cardImage = CardImage;
					cardImage.color = token.Type switch
					{
						TokenType.Slash => new Color(0.9f, 0.3f, 0.3f), 
						TokenType.Pierce => new Color(0.3f, 0.8f, 0.5f), 
						TokenType.Bash => new Color(0.7f, 0.3f, 0.2f), 
						TokenType.Emotion => new Color(0.2f, 0.7f, 0.8f), 
						TokenType.Skill => new Color(0.5f, 0.6f, 0.9f), 
						TokenType.Passion => new Color(0.9f, 0.4f, 0.2f), 
						TokenType.Ultimate => new Color(0.9f, 0.75f, 0.1f), 
						TokenType.Junk => new Color(0.5f, 0.5f, 0.5f), 
						_ => Color.white, 
					};
				}
			}
			Button.onClick.RemoveAllListeners();
			Button.onClick.AddListener(OnClick);
		}

		private void OnClick()
		{
			switch (_step)
			{
			case BattleState.HeroSelect:
				_battle.OnHeroTokenSelected(_index);
				break;
			case BattleState.SaintSelect:
				_battle.OnSaintTokenSelected(_index);
				break;
			case BattleState.FreeSelect:
				_battle.OnFreeTokenSelected(_index);
				break;
			case BattleState.BagChoiceRequired:
				break;
			}
		}
	}
}

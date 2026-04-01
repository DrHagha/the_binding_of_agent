using System;

namespace CombatPrototype.Core
{
	[Serializable]
	public class Token
	{
		public TokenType Type { get; private set; }

		public static int GetBaseDamage(TokenType type)
		{
			return type switch
			{
				TokenType.Slash => 8, 
				TokenType.Pierce => 6, 
				TokenType.Bash => 4, 
				TokenType.Passion => 5, 
				TokenType.Ultimate => 0, 
				_ => 0, 
			};
		}

		public static string GetDisplayName(TokenType type)
		{
			return type switch
			{
				TokenType.Slash => "참격", 
				TokenType.Pierce => "관통", 
				TokenType.Bash => "타격", 
				TokenType.Emotion => "세이렌의 노래", 
				TokenType.Skill => "솟아오르는 영감", 
				TokenType.Passion => "포르티시모", 
				TokenType.Ultimate => "거장의 혼", 
				TokenType.Junk => "잉어킹", 
				_ => "?", 
			};
		}

		public static string GetSpriteName(TokenType type)
		{
			return type switch
			{
				TokenType.Slash => "token_hero_slash", 
				TokenType.Pierce => "token_hero_pierce", 
				TokenType.Bash => "token_hero_bash", 
				TokenType.Emotion => "token_saint_serenade", 
				TokenType.Skill => "token_saint_inspiration", 
				TokenType.Passion => "token_saint_fortissimo", 
				TokenType.Ultimate => "token_ultimate", 
				TokenType.Junk => "token_junk", 
				_ => "", 
			};
		}

		public static string GetEffectDescription(TokenType type)
		{
			return type switch
			{
				TokenType.Slash => "데미지 8", 
				TokenType.Pierce => "데미지 6 · 방어무시", 
				TokenType.Bash => "데미지 4 · 기절 20%", 
				TokenType.Emotion => "피해 반사 50%", 
				TokenType.Skill => "2턴 회피 +70%", 
				TokenType.Passion => "피해 5 · 공격 ×2 · 피격 +50%", 
				TokenType.Ultimate => "기절 확률 60%", 
				TokenType.Junk => "효과 없음", 
				_ => "", 
			};
		}

		public static BagOwner GetOwner(TokenType type)
		{
			return type switch
			{
				TokenType.Slash => BagOwner.Hero, 
				TokenType.Pierce => BagOwner.Hero, 
				TokenType.Bash => BagOwner.Hero, 
				_ => BagOwner.Saint, 
			};
		}

		public Token(TokenType type)
		{
			Type = type;
		}

		public override string ToString()
		{
			return GetDisplayName(Type);
		}
	}
}

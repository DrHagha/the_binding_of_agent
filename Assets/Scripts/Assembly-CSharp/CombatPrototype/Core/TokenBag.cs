using System.Collections.Generic;
using UnityEngine;

namespace CombatPrototype.Core
{
	public class TokenBag
	{
		private readonly List<Token> _tokens = new List<Token>();

		private readonly BagOwner _owner;

		public int Count => _tokens.Count;

		public bool IsEmpty => _tokens.Count == 0;

		public BagOwner Owner => _owner;

		public TokenBag(BagOwner owner)
		{
			_owner = owner;
		}

		public void Initialize(BagConfig config)
		{
			_tokens.Clear();
			if (_owner == BagOwner.Hero)
			{
				Add(TokenType.Slash, config.SlashCount);
				Add(TokenType.Pierce, config.PierceCount);
				Add(TokenType.Bash, config.BashCount);
			}
			else
			{
				Add(TokenType.Emotion, config.EmotionCount);
				Add(TokenType.Skill, config.SkillCount);
				Add(TokenType.Passion, config.PassionCount);
			}
			Shuffle();
		}

		public List<Token> Draw(int count)
		{
			List<Token> list = new List<Token>();
			int num = Mathf.Min(count, _tokens.Count);
			for (int i = 0; i < num; i++)
			{
				int index = Random.Range(0, _tokens.Count);
				list.Add(_tokens[index]);
				_tokens.RemoveAt(index);
			}
			return list;
		}

		public void AddToken(TokenType type)
		{
			_tokens.Add(new Token(type));
		}

		public void ReturnToken(Token token)
		{
			_tokens.Add(token);
		}

		public Dictionary<TokenType, int> GetComposition()
		{
			Dictionary<TokenType, int> dictionary = new Dictionary<TokenType, int>();
			foreach (Token token in _tokens)
			{
				if (!dictionary.ContainsKey(token.Type))
				{
					dictionary[token.Type] = 0;
				}
				dictionary[token.Type]++;
			}
			return dictionary;
		}

		private void Add(TokenType type, int count)
		{
			for (int i = 0; i < count; i++)
			{
				_tokens.Add(new Token(type));
			}
		}

		private void Shuffle()
		{
			for (int num = _tokens.Count - 1; num > 0; num--)
			{
				int num2 = Random.Range(0, num + 1);
				List<Token> tokens = _tokens;
				int index = num;
				List<Token> tokens2 = _tokens;
				int index2 = num2;
				Token token = _tokens[num2];
				Token token2 = _tokens[num];
				Token token3 = (tokens[index] = token);
				token3 = (tokens2[index2] = token2);
			}
		}
	}
}

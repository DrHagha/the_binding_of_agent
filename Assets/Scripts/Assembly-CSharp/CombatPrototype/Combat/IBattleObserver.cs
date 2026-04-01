using System.Collections.Generic;
using CombatPrototype.Core;

namespace CombatPrototype.Combat
{
	public interface IBattleObserver
	{
		void OnStateChanged(BattleState state);

		void OnDraftChoice(BattleState step, List<Token> drawn);

		void OnBagChoiceRequired();

		void OnSynergyResult(SynergyResult synergy, int damage);

		void OnRefresh();

		void OnLog(string message);

		void OnBattleProgress(int currentBattle, int totalBattles);

		void OnGameOver(bool cleared);
	}
}

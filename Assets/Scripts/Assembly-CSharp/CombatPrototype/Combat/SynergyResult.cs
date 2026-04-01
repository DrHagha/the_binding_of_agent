using System.Collections.Generic;

namespace CombatPrototype.Combat
{
	public class SynergyResult
	{
		public List<SynergyType> Active = new List<SynergyType>();

		public float DamageMultiplier = 1f;

		public bool IgnoreAllArmor;

		public float ArmorIgnoreBonus;

		public float StunChanceBonus;

		public float StunMultiplier = 1f;

		public float GlobalMultiplier = 1f;

		public float GaugeBonus;

		public bool DrumSoloActive;

		public bool HasAnySynergy => Active.Count > 0;

		public string BuildDescription()
		{
			if (Active.Count == 0)
			{
				return "시너지 없음";
			}
			List<string> list = new List<string>();
			foreach (SynergyType item in Active)
			{
				List<string> list2 = list;
				list2.Add(item switch
				{
					SynergyType.Slash => "□ Slash – 데미지 ×1.5", 
					SynergyType.Penetrate => "□ Penetrate – 방어무시 + ×1.5", 
					SynergyType.Bash => "□ Bash – 기절 확률 ×2", 
					SynergyType.LeadSlash => "□ 선행참격 – 데미지 ×1.1", 
					SynergyType.LeadPenetrate => "□ 선행관통 – 방어무시 10%", 
					SynergyType.LeadBash => "□ 선행타격 – 기절 +10%", 
					SynergyType.Special => "□ Special – 모든 요소 ×1.5", 
					SynergyType.Signature => "□ Signature – 모든 요소 ×2", 
					SynergyType.DrumSolo => "□ 드럼 솔로 – 타격 40%추가+기절추가판정", 
					_ => "", 
				});
			}
			return string.Join("\n", list);
		}
	}
}

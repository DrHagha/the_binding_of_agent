using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CombatPrototype.UI
{
	public class GaugeBar : MonoBehaviour
	{
		public Slider Slider;

		public TextMeshProUGUI PercentText;

		public void SetValue(float value)
		{
			Slider.value = Mathf.Clamp01(value / 100f);
			PercentText.text = $"{value:F0}%";
		}
	}
}

using UnityEngine;
using UnityEngine.UI;

namespace FLFlight.UI {
	public class hudUI : MonoBehaviour {
		private Text statusReadoutText;
		private Text clipText;
		private Image healthBar;
		private Image shieldBar;
		private Image ammoBar;

		private void Awake() {
			statusReadoutText = transform.Find("StatusReadout").Find("Text").GetComponent<Text>();
			clipText = transform.Find("AmmoReadout").Find("ClipReadout").GetComponent<Text>();
			healthBar = transform.Find("HealthReadout").Find("HP").GetComponent<Image>();
			shieldBar = transform.Find("HealthReadout").Find("SP").GetComponent<Image>();
			ammoBar = transform.Find("AmmoReadout").Find("AmmoBelt").GetComponent<Image>();
		}
	}
}
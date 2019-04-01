using UnityEngine;
using UnityEngine.UI;

namespace FLFlight.UI {
	public class healthUI : MonoBehaviour {
		private Image healthFillBar;
		private Image shieldFillBar;
		int frameInterval = 5;

		private void Awake()
		{
			healthFillBar = transform.Find("HP").GetComponent<Image>();
		}

		void Update()
		{
			if(Time.frameCount % frameInterval == 0) {
				healthFillBar.fillAmount = Ship.PlayerShip.Health.health / Ship.PlayerShip.Health.healthCap;
			}
		}
	}
}
using UnityEngine;
using UnityEngine.UI;

namespace FLFlight.UI
{
	public class AmmoUI : MonoBehaviour {
		private Text text;
		private Image fillBar;
		int frameInterval = 5;

		private void Awake()
		{
			text = transform.Find("ClipReadout").GetComponent<Text>();
			fillBar = transform.Find("AmmoBelt").GetComponent<Image>();
		}

		void Update()
		{
			if(Time.frameCount % frameInterval == 0) {
				if (text != null && Ship.PlayerShip != null)
				{
					text.text = Ship.PlayerShip.Weapons.ClipNumber + " X";
					fillBar.fillAmount = Ship.PlayerShip.Weapons.ClipAmmo / Ship.PlayerShip.Weapons.ClipSize;
				}
			}
		}
	}
}
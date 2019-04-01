using UnityEngine;
using UnityEngine.UI;

namespace FLFlight.UI
{
    /// <summary>
    /// Shows throttle and speed of the player ship.
    /// </summary>
    public class SpeedUI : MonoBehaviour
    {
        private Text text;
		int frameInterval = 5;

        private void Awake()
        {
            text = GetComponent<Text>();
        }

        void Update()
        {
			if(Time.frameCount % frameInterval == 0) {
	            if (text != null && Ship.PlayerShip != null) {
					string THR = (Ship.PlayerShip.Input.Throttle * 100.0f).ToString("000");
					string SPD = Ship.PlayerShip.Velocity.magnitude.ToString("000");
					string PIT = (Ship.PlayerShip.Input.Pitch * 100.0f).ToString("000");
					string YAW = (Ship.PlayerShip.Input.Yaw * 100.0f).ToString("000");
					string ROL = (Ship.PlayerShip.Input.Roll * 100.0f).ToString("000");
					text.text = "THR: " + THR + "\t\tSPD: " + SPD + "\t\tPitch: " + PIT+ "\t  Yaw: " + YAW+ "\t  Roll: " + ROL;
	            }
			}
        }
    }
}


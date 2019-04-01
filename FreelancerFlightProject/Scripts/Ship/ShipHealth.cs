using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FLFlight {
	public class ShipHealth : MonoBehaviour {
		public Transform detEffect;
		public float health = 100;
		public float healthCap = 100;
		public bool isAI = false;

		void Start() {
			health = healthCap;
		}

		public void damageHealth(int amount) {
			health = health - amount;
			if(health <= 0) {
				die();
			}
			if(isAI) {
				GetComponent<AI_Ship>().setRandomEvasiveRoutine();
			}
		}

		void die() {
			GameObject explosion = Instantiate(detEffect.gameObject, transform.position, transform.rotation);
			explosion.transform.position = transform.position;
			explosion.SetActive(true);
			Destroy(gameObject);
		}
	}
}

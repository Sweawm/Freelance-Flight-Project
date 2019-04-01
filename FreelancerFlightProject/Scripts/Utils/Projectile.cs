using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FLFlight
{
	public class Projectile : MonoBehaviour
	{
		public Transform detonation;

		void OnCollisionEnter(Collision collision) {
			detonation.position = collision.GetContact(0).point;
			detonation.gameObject.SetActive(true);

			collision.transform.GetComponent<ShipHealth>().damageHealth(10);
			gameObject.SetActive(false);
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<TrailRenderer>().Clear();
		}
	}
}
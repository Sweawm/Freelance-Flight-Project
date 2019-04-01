using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FLFlight {
	public class WeaponSystem : MonoBehaviour {
		[Header("Prefabs")]
		[Tooltip("Prefab for Projectile Entity fired by Weapon System.")]
		public GameObject projectilePrefab;
		public GameObject soundPrefab;
		public GameObject hitPrefab;

		[Header("Weapon Properties")]
		[Range(0, 240)]
		[SerializeField] private int clipAmmo = 0;
		[Range(0, 240)]
		[SerializeField] private int clipSize = 30;
		[Range(0, 240)]
		[SerializeField] private int clipNumber = 6;
		[Range(0.0f, 10.0f)]
		[SerializeField] private float fireRate = 1.0f;
		[Range(0.0f, 10.0f)]
		[SerializeField] private float reloadTime = 2.0f;
		[Range(0, 100)]
		[SerializeField] private float projectileForce = 40;

		private float boresightDistance = 1000f;
		float fireRateCharge = 1.0f;

		bool readyToFire = true;
		bool leftFire = true;
		bool reloading = false;
		private List<Transform> ammoPool;
		private List<Transform> recyclePool;
		private List<Transform> hitPool;
		private List<Transform> recycleHitPool;
		private List<Transform> soundPool;
		private List<Transform> recycleSoundPool;
		Transform firepointLeft;
		Transform firepointRight;
		private bool playerWep = false;

		public float ClipAmmo { get { return clipAmmo; } }
		public float ClipSize { get { return clipSize; } }
		public float ClipNumber { get { return clipNumber; } }
		public bool SetAsPlayerWeapon { get { return playerWep; } set { playerWep = value; } }

	    void Start() {
			setClipSize();
			createAmmoPool();
		}

		void Update() {
			chargeGuns();
			if(playerWep) {
				getFireInput();
			}
		}

		void setClipSize() {
			Vector3 boresightPos = (transform.forward * boresightDistance) + transform.position;
			Vector3 screenPos = Camera.main.WorldToScreenPoint(boresightPos);
			screenPos.z = 0f;

			firepointLeft = transform.Find("FirePointL");
			firepointRight = transform.Find("FirePointR");
			firepointLeft.LookAt(boresightPos);
			firepointRight.LookAt(boresightPos);

			ammoPool = new List<Transform>();
			recyclePool = new List<Transform>();
			hitPool = new List<Transform>();
			recycleHitPool = new List<Transform>();
			soundPool = new List<Transform>();
			recycleSoundPool = new List<Transform>();
			clipAmmo = clipSize;
		}

		void createAmmoPool() {
			for(int i = 0; i < clipSize; i++) {
				GameObject ammo = Instantiate(projectilePrefab, transform.position, transform.rotation);
				ammo.SetActive(false);
				ammo.name = "AmmoRound_"+i;
				ammoPool.Add(ammo.transform);
			}
			for(int i = 0; i < clipSize; i++) {
				GameObject hit = Instantiate(hitPrefab, transform.position, transform.rotation);
				hit.SetActive(false);
				hit.name = "HitMarker_"+i;
				hitPool.Add(hit.transform);
			}
			for(int i = 0; i < clipSize / 2; i++) {
				GameObject sound = Instantiate(soundPrefab, transform.position, transform.rotation);
				sound.SetActive(false);
				sound.name = "SoundEffect_"+i;
				soundPool.Add(sound.transform);
			}
		}

		Transform getAmmoFromPool() {
			for(int i = 0; i < ammoPool.Count; i++) {
				if(!ammoPool[i].gameObject.activeSelf) {
					return ammoPool[i];
				}
			}
			return null;
		}

		Transform getHitFromPool() {
			for(int i = 0; i < hitPool.Count; i++) {
				if(!hitPool[i].gameObject.activeSelf) {
					return hitPool[i];
				}
			}
			return null;
		}

		Transform getSoundFromPool() {
			for(int i = 0; i < soundPool.Count; i++) {
				if(!soundPool[i].gameObject.activeSelf) {
					return soundPool[i];
				}
			}
			return null;
		}

		public void checkComputerTarget() {
			RaycastHit hit;
			if(Physics.SphereCast(transform.position, 2.5f, transform.forward, out hit, 600.0f)) {
				if(hit.transform.tag == "Player") {
					if(readyToFire) {
						if(clipAmmo > 0) {
							firePrimary();
							readyToFire = false;
							fireRateCharge = 0;
							clipAmmo--;
						} else if(!reloading && clipNumber > 0) {
							reloading = true;
							beginReload();
						}
					}
				}
			}
		}

		void chargeGuns() {
			if(reloading) {
				if(fireRateCharge > reloadTime) {
					endReload();
				} else {
					fireRateCharge += Time.deltaTime;
				}
			}
			if(!readyToFire) {
				if(fireRateCharge > fireRate) {
					readyToFire = true;
					popAmmo();
				} else {
					if(fireRateCharge > fireRate / 2) {
						firepointLeft.gameObject.SetActive(false);
						firepointRight.gameObject.SetActive(false);
					}
					fireRateCharge += Time.deltaTime;
				}
			}
		}

		void popAmmo() {
			if(recyclePool.Count > 20) {
				recyclePool[0].gameObject.SetActive(false);
				recyclePool[0].GetComponent<Rigidbody>().velocity = Vector3.zero;
				recyclePool[0].GetComponent<TrailRenderer>().Clear();
				recyclePool.RemoveAt(0);
			}
			if(recycleHitPool.Count > 10) {
				recycleHitPool[0].gameObject.SetActive(false);
				recycleHitPool[0].GetComponent<ParticleSystem>().Clear();
				recycleHitPool.RemoveAt(0);
			}
			if(recycleSoundPool.Count > 10) {
				recycleSoundPool[0].gameObject.SetActive(false);
				recycleSoundPool.RemoveAt(0);
			}
		}

		void getFireInput() {
			if(Input.GetMouseButton(0)) {
				if(readyToFire) {
					if(clipAmmo > 0) {
						firePrimary();
						readyToFire = false;
						fireRateCharge = 0;
						clipAmmo--;
					} else if(!reloading && clipNumber > 0) {
						reloading = true;
						beginReload();
					}
				}
			}
			if(Input.GetKeyDown("r")) {
				if(!reloading && clipNumber > 0) {
					reloading = true;
					beginReload();
				}
			}
		}

		void beginReload() {
			clipAmmo = 0;
			clipNumber--;
			fireRateCharge = 0;
			transform.Find("Visual").GetComponent<AudioSource>().Play();
		}

		void endReload() {
			transform.Find("Visual").GetComponent<AudioSource>().Play();
			clipAmmo = clipSize;
			fireRateCharge = 0;
			reloading = false;
		}

		void firePrimary() {
			if(leftFire) {
				Transform ammo = getAmmoFromPool();
				ammo.position = firepointLeft.position;
				ammo.rotation = firepointLeft.rotation;
				ammo.gameObject.SetActive(true);
				ammo.GetComponent<Rigidbody>().velocity = Ship.PlayerShip.Velocity;
				ammo.GetComponent<Rigidbody>().AddForce(transform.forward * projectileForce * 100);
				ammo.GetComponent<Projectile>().detonation = getHitFromPool();
				firepointLeft.gameObject.SetActive(true);
				recyclePool.Add(ammo);
				Transform soundEffect = getSoundFromPool();
				soundEffect.position = firepointLeft.position;
				soundEffect.gameObject.SetActive(true);
				recycleSoundPool.Add(soundEffect);

				leftFire = false;
			} else {
				Transform ammo = getAmmoFromPool();
				ammo.position = firepointRight.position;
				ammo.rotation = firepointRight.rotation;
				ammo.gameObject.SetActive(true);
				ammo.GetComponent<Rigidbody>().velocity = Ship.PlayerShip.Velocity;
				ammo.GetComponent<Rigidbody>().AddForce(transform.forward * projectileForce * 100);
				ammo.GetComponent<Projectile>().detonation = getHitFromPool();
				firepointRight.gameObject.SetActive(true);
				recyclePool.Add(ammo);
				Transform soundEffect = getSoundFromPool();
				soundEffect.position = firepointRight.position;
				soundEffect.gameObject.SetActive(true);
				recycleSoundPool.Add(soundEffect);

				leftFire = true;
			}
		}
	}
}

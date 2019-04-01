using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FLFlight {
	/// <summary>
	/// Ties all the primary ship components together.
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(ShipPhysics))]
	[RequireComponent(typeof(WeaponSystem))]

	public class AI_Ship : MonoBehaviour {
		public ShipPhysics Physic { get; internal set; }
		public WeaponSystem Weapons { get; internal set; }

		[Tooltip("How far the ship will bank when turning.")]
		[SerializeField] private float bankLimit = 35f;

		[Tooltip("Sensitivity in the pitch axis.\n\nIt's best to play with this value until you can get something the results in full input when at the edge of the screen.")]
		[SerializeField] private float pitchSensitivity = 2.5f;
		[Tooltip("Sensitivity in the yaw axis.\n\nIt's best to play with this value until you can get something the results in full input when at the edge of the screen.")]
		[SerializeField] private float yawSensitivity = 2.5f;
		[Tooltip("Sensitivity in the roll axis.\n\nTweak to make responsive enough.")]
		[SerializeField] private float rollSensitivity = 1f;

		[Range(-1, 1)]
		[SerializeField] private float pitch;
		[Range(-1, 1)]
		[SerializeField] private float yaw;
		[Range(-1, 1)]
		[SerializeField] private float roll;
		[Range(-1, 1)]
		[SerializeField] private float strafe;
		[Range(0, 1)]
		[SerializeField] private float throttle;

		// How quickly the throttle reacts to input.
		private const float THROTTLE_SPEED = 0.5f;

		public float Pitch { get { return pitch; } }
		public float Yaw { get { return yaw; } }
		public float Roll { get { return roll; } }
		public float Strafe { get { return strafe; } }
		public float Throttle { get { return throttle; } }

		public Transform targetObject;
		public int AI_Routine = 1;
		public float routineTimer = 0;
		public float routineTimeOut = 1;

		private void Awake() {
			Physic = GetComponent<ShipPhysics>();
			Weapons = GetComponent<WeaponSystem>();
		}


		private void getTarget() {
			targetObject = Ship.PlayerShip.transform;
		}

		private void Update() {
			// Pass the input to the physics to move the ship.
			Physic.SetPhysicsInput(new Vector3(Strafe, 0.0f, Throttle), new Vector3(Pitch, Yaw, Roll));

			if(targetObject != null) {
				enegageRoutine();
			}
		}

		private void enegageRoutine() {
			switch(AI_Routine) {
			case 0: 
				patrolRoutine();
				break;
			case 1:
				patrolRoutine();
				break;
			case 2:
				pursueRoutine();
				break;
			case 3:
				breakOffDownRoutine();
				break;
			case 4:
				breakOffUpRoutine();
				break;
			case 5:
				breakOffLeftRoutine();
				break;
			case 6:
				breakOffRightRoutine();
				break;
			case 7:
				boostRoutine();
				break;
			}
		}

		public void setRandomEvasiveRoutine() {
			int move = Random.Range(1,4);
			switch(move) {
			case 0:
				AI_Routine = 3;
				break;
			case 1:
				AI_Routine = 3;
				break;
			case 2:
				AI_Routine = 4;
				break;
			case 3:
				AI_Routine = 5;
				break;
			case 4:
				AI_Routine = 6;
				break;
			}
		}

		private void breakOffDownRoutine() {
			UpdateThrottle(0.7f);
			Vector3 dest = transform.position + -transform.up*10;
			TurnTowardsPoint(dest);
			clearObstacle();
		}

		private void breakOffUpRoutine() {
			UpdateThrottle(0.7f);
			Vector3 dest = transform.position + transform.up*10;
			TurnTowardsPoint(dest);
			clearObstacle();
		}

		private void breakOffLeftRoutine() {
			UpdateThrottle(0.7f);
			Vector3 dest = transform.position + -transform.right*10;
			TurnTowardsPoint(dest);
			clearObstacle();
		}

		private void breakOffRightRoutine() {
			UpdateThrottle(0.7f);
			Vector3 dest = transform.position + transform.right*10;
			TurnTowardsPoint(dest);
			clearObstacle();
		}

		private void boostRoutine() {
			UpdateThrottle(1.0f);
			Vector3 dest = transform.position + transform.forward*10;
			TurnTowardsPoint(dest);
			checkObstacle();
			if(Vector3.Distance(transform.position, targetObject.position) > 120f) {
				AI_Routine = 2;
			}
		}

		private void patrolRoutine() {
			UpdateThrottle(0.2f);
			TurnTowardsPoint(transform.forward * 1600);
		}

		private void pursueRoutine() {
			if(Vector3.Distance(transform.position, targetObject.position) > 100f) {
				UpdateThrottle(0.8f);
			} else {
				if(Vector3.Distance(transform.position, targetObject.position) < 30f) {
					setRandomEvasiveRoutine();
				} else {
					UpdateThrottle(0.1f);
				}
			}
			checkObstacle();
			TurnTowardsPoint(targetObject.position);
			checkWeapon();
		}

		private void checkObstacle() {
			RaycastHit hit;
			if(Physics.SphereCast(transform.position, 3.0f, transform.forward, out hit, 70.0f)) {
				setRandomEvasiveRoutine();
			}
		}

		private void clearObstacle() {
			RaycastHit hit;
			if(!Physics.SphereCast(transform.position, 4.0f, transform.forward, out hit, 50.0f)) {
				AI_Routine = 7;
			}
		}

		private void checkWeapon() {
			Weapons.checkComputerTarget();
		}

		/// <summary>
		/// Raise and lower the throttle.
		/// </summary>
		private void UpdateThrottle(float targetThrottle)
		{
			float target = throttle;
			target = targetThrottle;
			throttle = Mathf.MoveTowards(throttle, target, Time.deltaTime * THROTTLE_SPEED);
		}

		/// <summary>
		/// Pitches and yaws the ship to look at the passed in world position.
		/// </summary>
		/// <param name="gotoPos">World position to turn the ship towards.</param>
		private void TurnTowardsPoint(Vector3 gotoPos)
		{
			Vector3 localGotoPos = transform.InverseTransformVector(gotoPos - transform.position).normalized;

			// Note that you would want to use a PID controller for this to make it more responsive.
			pitch = Mathf.Clamp(-localGotoPos.y * pitchSensitivity, -1f, 1f);
			yaw = Mathf.Clamp(localGotoPos.x * yawSensitivity, -1f, 1f);
		}
	}
}
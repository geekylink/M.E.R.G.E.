using UnityEngine;
using System.Collections;
using System.Linq;

public class TurretSatellite : BaseSatellite {

	public float fireRate = 1;
	public float ammoVelocity;
	public float targetRadius = 50;

	public bool hasTarget;

	public GameObject targetObject;
	public GameObject ammoPrefab;
	public GameObject enemyAmmoPrefab;

	private float lastFire = 0;

	// Use this for initialization
	void Start () {
		hasTarget = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		UpdateOrbit ();
		GetTarget ();
		Fire ();
	}

	// Finds a target
	private void GetTarget() {
		if (hasTarget) {
			if (targetObject) {
				Vector3 dist = targetObject.transform.position - this.transform.position;
				if (dist.magnitude < targetRadius) {
					return;
				}
			}
		}

		hasTarget = false;
		GameObject [] enemies;

		if(team == SatelliteTeam.Player){
			enemies = (GameObject.FindGameObjectsWithTag ("Enemy")).Concat(GameObject.FindGameObjectsWithTag("WeakPoint")).ToArray();
		}
		else{
			enemies = GameObject.FindGameObjectsWithTag ("Player");
		}
		foreach (GameObject obj in enemies) {
			Vector3 dist = obj.transform.position - this.transform.position;
			if (dist.magnitude < targetRadius) {
				targetObject = obj;
				hasTarget = true;
				return;
			}
		}
	}

	// Handles firing the turret
	private void Fire() {
		if (hasTarget) {
			if (lastFire <= 0) {
				GameObject bulletGO;

				if(team == SatelliteTeam.Player){
					bulletGO = Instantiate (ammoPrefab, this.transform.position, this.transform.rotation) as GameObject;
				}
				else{
					bulletGO = Instantiate (enemyAmmoPrefab, this.transform.position, this.transform.rotation) as GameObject;
				}
				Bullet b = bulletGO.GetComponent ("Bullet") as Bullet;

				Vector2 ammoVel = Vector2.zero;
				ammoVel.x = targetObject.transform.position.x - this.gameObject.transform.position.x;
				ammoVel.y = targetObject.transform.position.y - this.gameObject.transform.position.y;

				b.damageDealt = 1;

				b.setDefaults (ammoVel);
				lastFire = fireRate;
			}

			lastFire -= Time.fixedDeltaTime;
		}
	}
}

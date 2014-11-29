using UnityEngine;
using System.Collections;
using System.Linq;

public class TurretSatellite : BaseSatellite {

	public float fireRate = 1;
	public bool shouldAutoFire = true;
	public float ammoVelocity;
	public float targetRadius = 50;

	public bool hasTarget;

	public GameObject targetObject;
	public GameObject ammoPrefab;
	public GameObject enemyAmmoPrefab;

	private float lastFire = 0;

	Vector2 targetPrevPos;

	// Use this for initialization
	void Start () {
		hasTarget = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		UpdateOrbit ();
		GetTarget ();
		if(shouldAutoFire){
			AutoFire ();
		}
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

	public void PlayerFire(Vector2 aimVec){
		if (lastFire <= 0) {
			GameObject bulletGO;
			
			if(team == SatelliteTeam.Player){
				bulletGO = Instantiate (ammoPrefab, this.transform.position, this.transform.rotation) as GameObject;
			}
			else{
				bulletGO = Instantiate (enemyAmmoPrefab, this.transform.position, this.transform.rotation) as GameObject;
			}
			Bullet bull = bulletGO.GetComponent ("Bullet") as Bullet;

			bull.damageDealt = 1;
			bull.setDefaults (aimVec.normalized * ammoVelocity);
			lastFire = fireRate;
		}

		lastFire -= Time.fixedDeltaTime;
		
	}

// Handles firing the turret
	private void AutoFire() {
		if (hasTarget) {
			Vector2 pos = targetObject.transform.position;




			if (lastFire <= 0) {
				GameObject bulletGO;

				if(team == SatelliteTeam.Player){
					bulletGO = Instantiate (ammoPrefab, this.transform.position, this.transform.rotation) as GameObject;
				}
				else{
					bulletGO = Instantiate (enemyAmmoPrefab, this.transform.position, this.transform.rotation) as GameObject;
				}
				Bullet bull = bulletGO.GetComponent ("Bullet") as Bullet;

				Vector2 aimVec = Vector2.zero;
				float bulletVelPerFrame = 1 / ammoVelocity;
				Vector2 targetVelPerFrame = (pos - targetPrevPos) / Time.deltaTime;
				
				//Going to use a quadratic equation to figure out where to aim to lead the target
				float a = Mathf.Pow(targetVelPerFrame.x, 2) + Mathf.Pow (targetVelPerFrame.y, 2) - Mathf.Pow(ammoVelocity, 2);
				float b = 2 * (targetVelPerFrame.x * (pos.x - transform.position.x) + targetVelPerFrame.y * (pos.y - transform.position.y));
				float c = Mathf.Pow(pos.x - transform.position.x, 2) + Mathf.Pow(pos.y - transform.position.y, 2);
				
				float discriminant = Mathf.Pow(b, 2) - 4 * a * c;
				
				if(discriminant < 0){
					aimVec = pos - (Vector2)transform.position;
				}
				else{
					float time1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
					time1 = (time1 < 0) ? Mathf.Infinity : time1;
					
					float time2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
					time2 = (time2 < 0) ? Mathf.Infinity : time2;
					float t = (time1 < time2 ? time1 : time2);
					
					aimVec = (t * targetVelPerFrame + pos) - (Vector2)transform.position;
				}
				
				//So the above is super accurate and actually kinda hard to dodge
				//I think I'll make it randomly select somewhere to shoot at that isn't there
				float yDiff = Random.Range(-4.0f, 4.0f);
				float xDiff = Random.Range (-4.0f, 4.0f);
				aimVec.x += xDiff;
				aimVec.y += yDiff;


				bull.damageDealt = 1;
				bull.setDefaults (aimVec.normalized * ammoVelocity);
				lastFire = fireRate;
			}
			
			targetPrevPos = pos;
			lastFire -= Time.fixedDeltaTime;
		}
	}
}

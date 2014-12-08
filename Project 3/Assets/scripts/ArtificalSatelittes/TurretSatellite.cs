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

	private float lastFire = 0.5f;

	Vector2 targetPrevPos;

	// Use this for initialization
	void Start () {
		hasTarget = false;
		GameManager.S.enemyList.Add (this.gameObject);
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

		//This little bit of randomness gives more randomness for selecting which player
		//turrets choose to shoot at, if all players are in the same area
		int randomIndexStart = Random.Range(0, enemies.Length);
		for(int i = 0; i < enemies.Length; ++i){
			int newI = (i + randomIndexStart) % enemies.Length;
			Vector3 dist = enemies[newI].transform.position - this.transform.position;
			if (dist.magnitude < targetRadius) {
				targetObject = enemies[newI];
				hasTarget = true;
				targetPrevPos = targetObject.transform.position;
				return;
			}
		}
	}

	//Allows player to fire own turrets
	//Since we aren't doing player turrets anymore, I guess this is useless
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

				//Spawn based on team of satellite
				if(team == SatelliteTeam.Player){
					bulletGO = Instantiate (ammoPrefab, this.transform.position, this.transform.rotation) as GameObject;
				}
				else{
					bulletGO = Instantiate (enemyAmmoPrefab, this.transform.position, this.transform.rotation) as GameObject;
				}
				Bullet bull = bulletGO.GetComponent ("Bullet") as Bullet;

				//Get bullet velocity, and estimated target enemy velocity
				Vector2 aimVec = Vector2.zero;
				Vector2 targetVelPerFrame = (pos - targetPrevPos) / Time.deltaTime;
				
				//Going to use a quadratic equation to figure out where to aim to lead the target
				//Aiming solution found at http://stackoverflow.com/questions/2248876/2d-game-fire-at-a-moving-target-by-predicting-intersection-of-projectile-and-u
				float a = Mathf.Pow(targetVelPerFrame.x, 2) + Mathf.Pow (targetVelPerFrame.y, 2) - Mathf.Pow(ammoVelocity, 2);
				float b = 2 * (targetVelPerFrame.x * (pos.x - transform.position.x) + targetVelPerFrame.y * (pos.y - transform.position.y));
				float c = Mathf.Pow(pos.x - transform.position.x, 2) + Mathf.Pow(pos.y - transform.position.y, 2);
				
				float discriminant = Mathf.Pow(b, 2) - 4 * a * c;

				//If discriminant is less than zero, no firing solution found
				if(discriminant < 0){
					aimVec = pos - (Vector2)transform.position;
				}
				else{
					//Otherwise, two possibilities (one of them may be negative, so don't select that one);
					float time1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
					time1 = (time1 < 0) ? Mathf.Infinity : time1;
					
					float time2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
					time2 = (time2 < 0) ? Mathf.Infinity : time2;
					float t = (time1 < time2 ? time1 : time2);
					
					aimVec = (t * targetVelPerFrame + pos) - (Vector2)transform.position;
				}

				/*
				 * Need to add a bit of randomness here.
				 * I had it just randomly oscillate the x and y values it is shooting at, but
				 * it looked pretty dumb when the turret misses you when you are sitting
				 * right in front of it not moving and it misses
				 * 
				 * So, my current plan is have the turret be more accurate the closer
				 * you are, and the closer you are 
				 * "more accurate" meaning a larger range for random.range
				 */
				float randRangeFromDist = 0;
				float randRangeFromSpeed = 0;

				randRangeFromDist = (pos - (Vector2)transform.position).magnitude / 12;
				randRangeFromSpeed = (targetVelPerFrame).magnitude / 10;

				float range = randRangeFromDist + randRangeFromSpeed;
				float yDiff = Random.Range(-range, range);
				float xDiff = Random.Range (-range, range);
				aimVec.x += xDiff;
				aimVec.y += yDiff;


				bull.damageDealt = 1;
				bull.setDefaults (aimVec.normalized * ammoVelocity);
				lastFire = fireRate;
				float littleBitOfRand = Random.Range(-0.2f, 0.2f);
				lastFire += littleBitOfRand;
			}
			
			targetPrevPos = pos;
			lastFire -= Time.fixedDeltaTime;
		}
	}
}

using UnityEngine;
using System.Collections;

public class HomingMissile : Bullet {

	public GameObject currTarget;
	public float startVel;
	public float maxVel;
	bool init = true;
	bool targetAcquired = false;
	float timeSinceLaunch = 1f;

	Vector3 vel;
	public Vector3 Velocity {
		get{ return vel;}
		set{ vel = value;}
	}


	void Start(){
		currTarget = GetRandomEnemyOnScreen();
		damageDealt = 2;
	}

	void Update(){
		timeSinceLaunch += Time.deltaTime;
		if (currTarget != null) {
			targetAcquired = true;
			Vector3 targetPos = currTarget.transform.position;
			var dir = targetPos - transform.position;
			if(init){
				if(startVel < 10f){
					startVel = 10f;
				}
				this.rigidbody2D.velocity = dir.normalized * startVel;
				init = false;
			} 
			if(this.rigidbody2D.velocity.magnitude < maxVel){
				this.rigidbody2D.velocity = dir.normalized * (startVel + 
					(maxVel - startVel) * timeSinceLaunch * timeSinceLaunch);
			} else {
				this.rigidbody2D.velocity = dir.normalized * maxVel;
			}

			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 180;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		else{
			if(!targetAcquired){
				currTarget = GetRandomEnemyOnScreen();
				this.rigidbody2D.velocity = vel.normalized 
					* (startVel + (maxVel - startVel) * timeSinceLaunch * timeSinceLaunch);
			}
		}
	}

	GameObject GetRandomEnemyOnScreen(){
		int counter = 0;
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes (Camera.main);
		if (GameManager.S.enemyList.Count != 0) {
			while(counter++ < 50){
				int idx = Random.Range(0, GameManager.S.enemyList.Count);
				if(GeometryUtility.TestPlanesAABB(planes, GameManager.S.enemyList[idx].collider2D.bounds)){
					return GameManager.S.enemyList[idx];
				}
			}
		}
		return null;
	}
}

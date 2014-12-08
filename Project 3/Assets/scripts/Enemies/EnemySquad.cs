using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//based on pseudocode found at http://www.kfish.org/boids/pseudocode.html
public class EnemySquad : MonoBehaviour {

	public List <EnemyBaseShip> squadMembers;
	public bool avoidTarget;
	public bool disperseSquad;
	public int squadID;
	public GameObject target;
	public float vLimit;

	public bool targetIsPlanet;
	// Use this for initialization

	public Vector3 squadCenter;
	public Vector3 squadVelocity;


	void Update(){

		Vector3 theCenter = Vector3.zero;
		Vector3 theVelocity = Vector3.zero;
		
		foreach (EnemyBaseShip member in squadMembers)
		{
			if(!member) continue;
			theCenter = theCenter + member.transform.localPosition;
			theVelocity = theVelocity + (Vector3)member.rigidbody2D.velocity;
		}
		
		squadCenter = theCenter/(squadMembers.Count);
		squadVelocity = theVelocity/(squadMembers.Count);

		if(targetIsPlanet){
			if(target.GetComponent<CapturePoint>().controlledBy != CapturePoint.ControlledBy.Player){
				target.GetComponent<CapturePoint>().Capture(5f, CapturePoint.ControlledBy.Enemy);

				int ranValue = Random.Range (0, 5);
				if(ranValue == 0){
					targetIsPlanet = true;
					
					CapturePoint furthestPlayerPlanet = null;
					float furthestRadius = 0;
					foreach(CapturePoint cp in GameManager.S.capturePoints){
						
						BaseSatellite bs = cp.GetComponent<BaseSatellite>();
						if(bs.orbitRadius > furthestRadius && cp.controlledBy == CapturePoint.ControlledBy.Player){
							furthestRadius = bs.orbitRadius;
							furthestPlayerPlanet = cp;
						}
					}
					
					if(furthestPlayerPlanet == null){
						ranValue = 1;
					}
					else{
						target = furthestPlayerPlanet.gameObject;
					}
				}
				if(ranValue > 0){
					targetIsPlanet = false;
					target = Camera.main.gameObject;
				}
			}
		}

	}


	public Vector2 ApplyBoids(EnemyBaseShip enemy){
		if(!target){
			target = enemy.getRandomPlayer();
		}

		Vector2 v1, v2, v3, v4;
		v1 = TowardCenter (enemy);
		v2 = Separation (enemy);
		v3 = MatchVelocity (enemy);
		v4 = TendTowardTarget (enemy);

		if (disperseSquad) {
			v1 *= -1f;
		}
		if (avoidTarget) {
			v4 *= -1f;
		}
		Vector2 velocity = v1 + v2 + v3 + v4;
		return velocity;
	}

	Vector2 TowardCenter(EnemyBaseShip enemy){
		Vector2 positionSum = Vector2.zero;

		int numNearby = 0;
		foreach (EnemyBaseShip e in squadMembers) {
			if (e != enemy){
				if (Vector2.Distance(e.transform.position, enemy.transform.position) < Spawner.S.distanceLimit){
					positionSum += (Vector2)e.transform.position;
					numNearby++;
				}
			}
		}
		Vector2 center = Vector2.zero;
		if(numNearby > 0){
			center = positionSum / numNearby;
		}
		squadCenter = center;

		center.x = (center.x - enemy.transform.position.x) / Spawner.S.towardCenterFactor;
		center.y = (center.y - enemy.transform.position.y) / Spawner.S.towardCenterFactor;
		return center;
	}

	Vector2 Separation(EnemyBaseShip enemy){
		Vector2 positionSum = Vector2.zero;

		int numNearby = 0;
		foreach (EnemyBaseShip e in squadMembers) {
			if (e != enemy){
				if (Vector2.Distance(e.transform.position, enemy.transform.position) < Spawner.S.distanceLimit / 3){
					positionSum -= (Vector2)e.transform.position - (Vector2)enemy.transform.position;
					numNearby++;
				}
			}
		}
		if(numNearby > 0){
			positionSum = positionSum / numNearby;
		}
		return positionSum / Spawner.S.separationFactor;
	}

	Vector2 MatchVelocity(EnemyBaseShip enemy){
		Vector2 velocitySum = Vector2.zero;
		int numNearby = 0;
		foreach (EnemyBaseShip e in squadMembers) {
			if(e != enemy){
				if (Vector2.Distance(e.transform.position, enemy.transform.position) < Spawner.S.distanceLimit){
					velocitySum += e.rigidbody2D.velocity;
					numNearby++;
				}
			}
		}
		if(numNearby > 0){
			velocitySum = velocitySum / numNearby;
		}
		return (velocitySum - enemy.rigidbody2D.velocity) / Spawner.S.velocityMatchFactor;
	}

	Vector2 TendTowardTarget(EnemyBaseShip enemy){
		if (target) {
			return (target.transform.position - enemy.transform.position) / Spawner.S.towardTargetFactor;
		} else {
			return Vector2.zero;
		}
	}
}

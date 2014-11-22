using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//based on pseudocode found at http://www.kfish.org/boids/pseudocode.html
public class EnemySquad : MonoBehaviour {

	public List <EnemyBaseShip> squadMembers;
	public float distanceLimit;
	public float velocityMatchFactor;
	public float towardTargetFactor;
	public float towardCenterFactor;
	public bool avoidTarget;
	public bool disperseSquad;
	public int squadID;
	public GameObject target;
	// Use this for initialization
	void Start () {
		//squadMembers = new List<EnemyBaseShip> ();
	}

	// Update is called once per frame
	void Update () {
		foreach (EnemyBaseShip enemy in squadMembers) {
			if(target){
				enemy.currTarget = target;
				ApplyBoids(enemy);
			}
		}
	}

	public Vector3 ApplyBoids(EnemyBaseShip enemy){
		Vector3 v1, v2, v3, v4;
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

		return v1 + v2 + v3 + v4;
	}

	Vector3 TowardCenter(EnemyBaseShip enemy){
		Vector3 positionSum = Vector3.zero;
		foreach (EnemyBaseShip e in squadMembers) {
			if (e){
				if(e != enemy){
					positionSum += e.transform.position;
				}
			}
		}
		Vector3 center = positionSum / (float)(squadMembers.Count - 1);
		center.x = (center.x - enemy.transform.position.x) / towardCenterFactor;
		center.y = (center.y - enemy.transform.position.y) / towardCenterFactor;
		center.z = (center.z - enemy.transform.position.z) / towardCenterFactor;
		return center;
	}

	Vector3 Separation(EnemyBaseShip enemy){
		Vector3 positionSum = Vector3.zero;
		foreach (EnemyBaseShip e in squadMembers) {
			if(e){
				if (e != enemy){
					if (Vector3.Distance(e.transform.position, enemy.transform.position) > distanceLimit){
						positionSum -= e.transform.position - enemy.transform.position;
					}
				}
			}
		}
		return positionSum;
	}

	Vector3 MatchVelocity(EnemyBaseShip enemy){
		Vector3 velocitySum = Vector3.zero;
		foreach (EnemyBaseShip e in squadMembers) {
			if(e != enemy){
				velocitySum += e.velocity;
			}
		}
		velocitySum = velocitySum / (float)(squadMembers.Count - 1);
		return (velocitySum - enemy.velocity) / velocityMatchFactor;
	}

	Vector3 TendTowardTarget(EnemyBaseShip enemy){
		return (target.transform.position - enemy.transform.position) / towardTargetFactor;
	}
}

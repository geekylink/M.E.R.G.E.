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
	public float vLimit;
	// Use this for initialization
	void Start () {
		//squadMembers = new List<EnemyBaseShip> ();
	}

	// Update is called once per frame
	void Update () {

	}

	public Vector2 ApplyBoids(EnemyBaseShip enemy){
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
		Vector2 velocity = v1 + v3 + v4;
		if (velocity.magnitude > vLimit) {
			return new Vector2(velocity.normalized.x / Mathf.Abs(velocity.normalized.x), velocity.normalized.y / Mathf.Abs(velocity.normalized.y)) * vLimit;
		}
			return v1 + v2 + v3 + v4;
	}

	Vector2 TowardCenter(EnemyBaseShip enemy){
		Vector2 positionSum = Vector2.zero;
		foreach (EnemyBaseShip e in squadMembers) {
			if (e){
				if(e != enemy){
					positionSum += (Vector2)e.transform.position;
				}
			}
		}
		Vector2 center = positionSum / (float)(squadMembers.Count);
		center.x = (center.x - enemy.transform.position.x) / towardCenterFactor;
		center.y = (center.y - enemy.transform.position.y) / towardCenterFactor;
		return center;
	}

	Vector2 Separation(EnemyBaseShip enemy){
		Vector2 positionSum = Vector2.zero;
		foreach (EnemyBaseShip e in squadMembers) {
			if(e){
				if (e != enemy){
					if (Vector2.Distance(e.transform.position, enemy.transform.position) > distanceLimit){
						positionSum -= (Vector2)e.transform.position - (Vector2)enemy.transform.position;
					}
				}
			}
		}
		return positionSum;
	}

	Vector2 MatchVelocity(EnemyBaseShip enemy){
		Vector2 velocitySum = Vector2.zero;
		foreach (EnemyBaseShip e in squadMembers) {
			if(e != enemy){
				velocitySum += e.rigidbody2D.velocity;
			}
		}
		velocitySum = velocitySum / (float)(squadMembers.Count);
		return (velocitySum - enemy.rigidbody2D.velocity) / velocityMatchFactor;
	}

	Vector2 TendTowardTarget(EnemyBaseShip enemy){
		return (target.transform.position - enemy.transform.position) / towardTargetFactor;
	}
}

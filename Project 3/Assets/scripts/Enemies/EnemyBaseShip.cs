using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBaseShip : BaseShip {
	public int squadId;
	public GameObject currTarget;
	public float moveSpeed = 2f;
	public Vector3 targetPos;

	public bool boidInit = false;
	public float closeFollowWeight;
	public float closeFollowRange;
	public float closeFollowSpeed;

	public float highSpeedMult = 1;

	bool hasChosenClosePlayer = false;
	
	// Use this for initialization
	public override void Start () {
		squadId = 0;
		base.Start();	
	}
	
	public override void Die() {
		StopAllCoroutines();
		SquadManager.S.RemoveEnemy (this, squadId);
		GameManager.S.enemyList.Remove (this.gameObject);
		base.Die();		
	}

	public void StartRoutines(){
		StartCoroutine(EnemyFlying());
		StartCoroutine(EnemyTurning());
		StartCoroutine(AttackPlayersOnWayToPlanet());
	}

	IEnumerator AttackPlayersOnWayToPlanet(){

		while(true){
			if(squadId > 0 && SquadManager.S.squads.Count >= squadId){
				if(SquadManager.S.squads[squadId - 1].targetIsPlanet){
					if(!hasChosenClosePlayer){
						float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
						int ran = Random.Range(0, 100);
						
						if(distance < 30 && ran < 50){
							currTarget = getRandomPlayer();
							hasChosenClosePlayer = true;
						}
					}
					else{
						
						int ran = Random.Range(0, 100);
						if(ran < 50)
						{
							CapturePoint cp = SquadManager.S.squads[squadId-1].target.GetComponent<CapturePoint>();
							if(cp.satsInOrbit.Count > 0){
								int randomSat = Random.Range (0, cp.satsInOrbit.Count);
								currTarget = cp.satsInOrbit[randomSat].gameObject;
							}
							hasChosenClosePlayer = false;
						}
					}
				}
			}
			
			yield return new WaitForSeconds(Random.Range (2.0f, 6.0f));
		}
	}

	public IEnumerator EnemyFlying ()
	{
		while (true)
		{
			if(boidInit){
				if(currTarget != null){
					targetPos = currTarget.transform.position;
					var dir = targetPos - transform.position;
					Vector2 newVel = Vector2.Lerp (rigidbody2D.velocity, (Vector2)Boids(squadId), Time.deltaTime * 3);

					if(float.IsNaN(newVel.x) || float.IsNaN(newVel.y)) newVel = SquadManager.S.squads [squadId - 1].squadVelocity;

					rigidbody2D.velocity = newVel;
					//rigidbody2D.velocity = rigidbody2D.velocity + (Vector2)Boids(squadId) * Time.deltaTime;
					// enforce minimum and maximum speeds for the boids
					float speed = rigidbody2D.velocity.magnitude;
					if (speed > SquadManager.S.highLimit * highSpeedMult)
					{
						if(dir.magnitude > closeFollowRange){
							rigidbody2D.velocity = rigidbody2D.velocity.normalized * SquadManager.S.highLimit * highSpeedMult;
						}
						else{
							if(speed > closeFollowSpeed){
								rigidbody2D.velocity = rigidbody2D.velocity.normalized * closeFollowSpeed;
							}
						}
					}
					else if (speed < SquadManager.S.lowLimit)
					{
						rigidbody2D.velocity = rigidbody2D.velocity.normalized * SquadManager.S.lowLimit;
					}

				}
			}

			yield return new WaitForSeconds ( Random.Range(0, 0.5f));
		}
	}

	public Vector3 Boids(int squadId){
		List<EnemySquad> squads = SquadManager.S.squads;
		if(squadId > squads.Count && currTarget) return currTarget.transform.localPosition - transform.position;
		if(squadId > squads.Count && !currTarget) return Vector3.zero;
		
		Vector3 randomize = new Vector3 ((Random.value *2) -1, (Random.value * 2) -1, 0);
		
		randomize.Normalize();
		
		Vector3 flockCenter = squads [squadId - 1].squadCenter;
		Vector3 flockVelocity = squads [squadId - 1].squadVelocity;
		Vector3 follow = currTarget.transform.localPosition;
		Vector3 separation = Vector3.zero;
		
		flockCenter = flockCenter - transform.position;
		flockVelocity = flockVelocity - (Vector3)rigidbody2D.velocity;
		follow = follow - transform.position;

		int numShips = 0;
	

		foreach(EnemyBaseShip bs in squads[squadId - 1].squadMembers){
			if(!bs) continue;
			if(bs == this) continue;
			Vector3 bsPos = bs.transform.position;
			Vector3 pos = transform.position;
			if(Vector3.Distance(bsPos, pos) < 10){
				separation += pos - bsPos;
			}
		}
		if(numShips > 0){
			separation = separation / numShips;
		}

		float followWeight = SquadManager.S.followWeight;
		if(follow.magnitude < closeFollowRange){
			followWeight *= closeFollowWeight;
		}
		
		float flockCenterWeight = flockCenter.magnitude;
		flockCenter = flockCenter.normalized;
		if(flockCenterWeight > 0 && follow.magnitude > closeFollowRange){
			flockCenterWeight = flockCenterWeight *  flockCenterWeight * Random.value;
		}

		float randomness = SquadManager.S.randomness;
		if(follow.magnitude < closeFollowRange){
			randomness = randomness / 4;
		}

		return (flockCenter * flockCenterWeight + flockVelocity + follow * followWeight + randomize * randomness + separation * SquadManager.S.separationWeight);
	}

	public IEnumerator EnemyTurning(){
		while (true)
		{
			
			rigidbody2D.angularVelocity = 0;
			if(boidInit){
				if(currTarget != null){
					targetPos = currTarget.transform.position;
					var dir = targetPos - transform.position;

					var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 180;

					Quaternion newRot = Quaternion.AngleAxis(angle, Vector3.forward);
					Quaternion oldRot = transform.rotation;
					transform.rotation = Quaternion.Lerp (oldRot, newRot, Time.deltaTime * 20);

				}
				else{
					if(squadId > 0 && SquadManager.S.squads.Count >= squadId){
						if(SquadManager.S.squads[squadId-1].targetIsPlanet){
							CapturePoint cp = SquadManager.S.squads[squadId-1].target.GetComponent<CapturePoint>();
							if(cp.satsInOrbit.Count > 0){
								int randomSat = Random.Range (0, cp.satsInOrbit.Count);
								currTarget = cp.satsInOrbit[randomSat].gameObject;
							}
						}
						else{
							currTarget = getRandomPlayer();
						}
					}

				}
			}



			yield return 0;
		}
	}
}

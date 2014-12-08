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
		StartCoroutine(EnemySteering());
		StartCoroutine(EnemyTurning());
	}

	public IEnumerator EnemySteering ()
	{
		while (true)
		{
			if(boidInit){
				if(currTarget != null){
					targetPos = currTarget.transform.position;
					var dir = targetPos - transform.position;
					rigidbody2D.velocity = rigidbody2D.velocity + (Vector2)Boids(squadId) * Time.deltaTime;
					// enforce minimum and maximum speeds for the boids
					float speed = rigidbody2D.velocity.magnitude;
					if (speed > SquadManager.S.highLimit)
					{
						if(dir.magnitude > closeFollowRange){
							rigidbody2D.velocity = rigidbody2D.velocity.normalized * SquadManager.S.highLimit;
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

					
					var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 180;
					transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
				}
			}
			
			
			float waitTime = Random.Range(0.3f, 0.5f);
			yield return new WaitForSeconds (waitTime);
		}
	}

	public Vector3 Boids(int squadId){
		List<EnemySquad> squads = SquadManager.S.squads;
		//return squads [squadId - 1].ApplyBoids (enemy);
		if(squadId > squads.Count) return Vector3.zero;
		
		Vector3 randomize = new Vector3 ((Random.value *2) -1, (Random.value * 2) -1, 0);
		
		randomize.Normalize();
		
		Vector3 flockCenter = squads [squadId - 1].squadCenter;
		Vector3 flockVelocity = squads [squadId - 1].squadVelocity;
		Vector3 follow = currTarget.transform.localPosition;
		
		flockCenter = flockCenter - transform.position;
		flockVelocity = flockVelocity - (Vector3)rigidbody2D.velocity;
		follow = follow - transform.position;

		/*float followWeight = SquadManager.S.followWeight;
		if(follow.magnitude < 30f){
			followWeight *= closeFollowWeight;
		}*/
		
		return (flockCenter + flockVelocity + follow + randomize * SquadManager.S.randomness);
	}

	public IEnumerator EnemyTurning(){
		while (true)
		{
			if(boidInit){
				if(currTarget != null){
					targetPos = currTarget.transform.position;
					var dir = targetPos - transform.position;

					var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 180;
					transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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

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
				//target.GetComponent<CapturePoint>().Capture(5f, CapturePoint.ControlledBy.Enemy);

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


}

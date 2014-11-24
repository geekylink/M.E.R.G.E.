using UnityEngine;
using System.Collections;

public class GravityWell : MonoBehaviour {

	public float gravityMult;
	public float maxDistance;

	// Use this for initialization
	void Start () {
		maxDistance = this.collider2D.bounds.size.x / 2;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay2D(Collider2D col){
		GameObject ship = col.collider2D.gameObject;

		if(ship.tag == "Player"){
			Vector2 playerPos = ship.transform.position;
			Vector2 pos = transform.position;

			Vector2 gravDir = (pos - playerPos);

			float distance = gravDir.magnitude;
			float distanceMult = (maxDistance - distance) / maxDistance;

			Vector2 force = gravDir.normalized * distanceMult * gravityMult;
			ship.GetComponent<Player>().GravVector = (Vector3)force;

		}

	}
}

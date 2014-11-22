using UnityEngine;
using System.Collections;

public class SolarSystem : MonoBehaviour {

	public GameObject[] wellObjects;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		PlayerManager pm = GameObject.Find ("Main Camera").GetComponent("PlayerManager") as PlayerManager;
		GameObject[] players = pm.getPlayers ();

		if (players == null || players.Length == 0) // Just stop now if there aren't players
			return;

		// Apply gravity for each well to each player
		foreach (GameObject wellObj in wellObjects) {
			GravityWell well = wellObj.GetComponent ("GravityWell") as GravityWell;

			foreach (GameObject player in players) {
				if(player == null) continue;

				//print (wellObj.transform.position + " " + player.transform.position);

				Vector2 force = wellObj.transform.position - player.transform.position;

				float distanceMult = 0;
				if(force.magnitude == 0){
					distanceMult = 1;
				}
				else{
					distanceMult = 1 / force.magnitude;
				}


				player.transform.root.rigidbody2D.AddForceAtPosition (force*distanceMult*well.gravityMult, player.transform.position);
			}
		}
	}
}

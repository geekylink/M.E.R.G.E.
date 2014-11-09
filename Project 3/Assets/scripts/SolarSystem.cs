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
				Vector2 force = wellObj.transform.position - player.transform.position;

				player.transform.root.rigidbody2D.AddForceAtPosition (force*(1/force.magnitude)*well.gravityMult, player.transform.position);
			}
		}
	}
}

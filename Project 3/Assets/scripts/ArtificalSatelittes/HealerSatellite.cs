using UnityEngine;
using System.Collections;

public class HealerSatellite : BaseSatellite {

	public float healRadius = 50;
	public float healRate = 10;

	private float lastHeal = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		UpdateOrbit ();
		Heal ();
	}

	// Slowly heals a random player within range
	private void Heal() {
		if (lastHeal <= 0) {
			lastHeal = 0;
			GameObject cam = GameObject.Find ("Main Camera");
			PlayerManager pm = cam.GetComponent ("PlayerManager") as PlayerManager;
			GameObject[] players = pm.getPlayers ();

			foreach (GameObject playerObj in players) {
				if(playerObj == null) continue;
				Player player = playerObj.GetComponent ("Player") as Player;
				Vector3 dist = player.transform.position - this.transform.position;
				if (dist.magnitude < healRadius) {
					player.Heal (1);
					lastHeal = healRate;
				}
			}
		}
		lastHeal -= Time.fixedDeltaTime;
	}
}

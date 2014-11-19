using UnityEngine;
using System.Collections;

public class MinerSatellite : BaseSatellite {

	public float mineRate = 10;

	private float lastMine;

	// Use this for initialization
	void Start () {
		lastMine = mineRate;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		UpdateOrbit ();
		Mine ();
	}

	private void Mine() {
		if (lastMine < 0) {
			Player player = creatorObj.GetComponent("Player") as Player;
			player.GetResources(1);
			lastMine = mineRate;
		}

		lastMine -= Time.fixedDeltaTime;
	}
}

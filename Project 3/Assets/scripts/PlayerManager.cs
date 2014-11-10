﻿using UnityEngine;
using System.Collections;
using InControl;

public class PlayerManager : MonoBehaviour {

	public GameObject[] players;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void FixedUpdate () {
		// Cycle through the controllers and players
		for (int i = 0; i < InputManager.Devices.Count; i++) {
			if (i < players.Length) {
				if(players[i]){
					Player p = players[i].GetComponent ("Player") as Player;
					UpdatePlayer (InputManager.Devices[i], p);
				}
			}
		}
	}

	public GameObject[] getPlayers() {
		return players;
	}

	private void UpdatePlayer(InputDevice device, Player player) {
		float leftX = device.LeftStickX;
		float leftY = device.LeftStickY;
		float rightX = device.RightStickX;
		float rightY = device.RightStickY;
		//Player p = player.GetComponent ("Player") as Player;

		float leftAngle = Mathf.Atan2 (leftY, leftX)*Mathf.Rad2Deg;
		float rightAngle = Mathf.Atan2 (rightY, rightX)*Mathf.Rad2Deg;

		if(Mathf.Abs (leftY) < 0.3f && Mathf.Abs(leftX) < 0.3f){
			leftAngle = 0;
		}
		if(Mathf.Abs (rightY) < 0.3f && Mathf.Abs(rightX) < 0.3f){
			rightAngle = 0;
		}

		// Updates angles
		player.UpdateTurrets (leftAngle, rightAngle);
		//if (device.LeftTrigger)		player.FireLeftTurret(); 
		//if (device.RightTrigger)	player.FireRightTurret();

		//Check if A button is pushed (currently for merge/unmerge purposes
		//Comment out this line if testing with keyboard
		player.CheckMerge (device.Action1, device.Action2);

		if (device.Action3)	player.useBreaks ();

		// Fires the engines
		//Comment out this line if testing with keyboard
		player.FireEngines (device.LeftBumper, device.RightBumper);
	}
}

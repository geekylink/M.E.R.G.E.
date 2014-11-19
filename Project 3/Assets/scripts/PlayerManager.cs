using UnityEngine;
using System.Collections;
using InControl;

public class PlayerManager : MonoBehaviour {
	public static PlayerManager S;
	public GameObject[] players;

	public float playerSpeed = 30;

	// Use this for initialization
	void Start () {
		if(S == null)
		{
			//If I am the first instance, make me the Singleton
			S = this;
			//DontDestroyOnLoad(this);
		}
		else
		{
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if(this != S)
				Destroy(this.gameObject);
		}
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
			leftX = leftY = 0;
			leftAngle = 0;
		}
		if(Mathf.Abs (rightY) < 0.3f && Mathf.Abs(rightX) < 0.3f){
			rightAngle = 0;
		}

		if (device.DPadDown.WasPressed) {
			player.SpawnTurret(BaseSatellite.SatelliteType.TURRET, player.gameObject);
		}

		if (device.DPadUp.WasPressed) {
			player.SpawnTurret(BaseSatellite.SatelliteType.HEALER, player.gameObject);
		}

		if (device.DPadRight.WasPressed) {
			player.SpawnTurret(BaseSatellite.SatelliteType.MINER, player.gameObject);
		}

		// Updates angles

		//if (device.LeftTrigger)		player.FireLeftTurret(); 
		//if (device.RightTrigger)	player.FireRightTurret();

		//Check if A button is pushed (currently for merge/unmerge purposes
		//Comment out this line if testing with keyboard
		player.CheckMerge (device.Action1, device.Action2);



		#region Aim with sticks, fly with bumpers
		/*player.UpdateTurrets(leftAngle, rightAngle);
		player.FireEngines (device.LeftBumper, device.RightBumper);*/
		#endregion


		#region Turn left/right with stick, fly forward with bumper
		/*player.UpdateTurrets (0, rightAngle);
		player.Turn(leftX, -10);
		
		if(device.LeftBumper){
			player.FlyForward(0.2f);
		}
		else{
			player.useBreaks();
		}*/
		#endregion

		#region Aim in desired flying direction with left stick, fly forward with bumper
		/*player.UpdateTurrets (0, rightAngle);
		player.TurnTowards(leftAngle);
		if(device.LeftBumper){
			player.FlyForward(0.2f);
		}
		else{
			player.useBreaks();
		}*/
		#endregion

		#region Fly in the direction of the left stick
		player.UpdateTurrets (0, rightAngle);
		player.TurnTowards(leftAngle);
		Vector3 rot = Vector3.zero;
		rot.z = leftAngle;
		float speed = Mathf.Sqrt(leftX * leftX + leftY * leftY);
		player.FlyForward(speed, playerSpeed);
		/*if(speed == 0){
			player.useBreaks();
		}*/
		#endregion


		//if (device.Action3)	player.useBreaks ();

		//

	}
}

using UnityEngine;
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
		player.UpdateTurrets (0, rightAngle);
		player.TurnTowards(leftAngle);
		if(device.LeftBumper){
			player.FlyForward(0.2f);
		}
		else{
			player.useBreaks();
		}
		#endregion

		#region Fly in the direction of the left stick
		/*player.UpdateTurrets (0, rightAngle);
		player.TurnTowards(leftAngle);
		Vector3 rot = Vector3.zero;
		rot.z = leftAngle;
		float speed = Mathf.Sqrt(leftX * leftX + leftY * leftY);
		if(speed > 0){
			player.FlyForward((speed / 3));
		}
		else{
			player.useBreaks();
		}*/
		#endregion

		//if (device.Action3)	player.useBreaks ();

		//

	}
}

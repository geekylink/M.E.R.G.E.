using UnityEngine;
using System.Collections;
using InControl;

public class PlayerManager : MonoBehaviour {

	public GameObject player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		InputDevice device = InputManager.ActiveDevice;

		UpdateTurrets (device);
	}

	private void UpdateTurrets(InputDevice device) {
		float leftX = device.LeftStickX;
		float leftY = device.LeftStickY;
		float rightX = device.RightStickX;
		float rightY = device.RightStickY;
		Player p = player.GetComponent ("Player") as Player;

		float leftAngle = Mathf.Atan2 (leftY, leftX)*Mathf.Rad2Deg;
		float rightAngle = Mathf.Atan2 (rightY, rightX)*Mathf.Rad2Deg;

		// Updates angles
		p.UpdateTurrets (leftAngle, rightAngle);

		// Fires turrets
		if (device.LeftTrigger)		p.FireLeftTurret();
		if (device.RightTrigger)	p.FireRightTurret();

		// Fires the engines
		p.FireEngines (device.LeftBumper, device.RightBumper);
	}
}

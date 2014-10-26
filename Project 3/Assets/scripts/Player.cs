using UnityEngine;
using System.Collections;

public class Player : BaseShip {

	public float velocityMult = 1;
	public float bulletVelocity = 1;
	public GameObject ammoPrefab;

	public UnityEngine.UI.Text gtHealth;

	// Used to prevent firing constantly
	private float lastLeftFire = 0;
	private float lastRightFire = 0;

	// Use this for initialization
	void Start () {
		health = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateTurrets ();
		UpdatePlayer ();
		UpdateHUD ();
	}

	private void UpdateHUD() {
		print ("lol");
		gtHealth.text = "Health: " + health;
	}

	// Handles the angles of the turrets and firing bullets
	private void UpdateTurrets() {
		float leftX = Input.GetAxis ("Left Analog X");
		float leftY = Input.GetAxis ("Left Analog Y");
		float rightX = Input.GetAxis ("Right Analog X");
		float rightY = Input.GetAxis ("Right Analog Y");
		float leftFire = Input.GetAxis ("Fire1");
		float rightFire = Input.GetAxis ("Fire2");

		float leftAngle = Mathf.Atan2 (leftY, leftX)*Mathf.Rad2Deg;
		float rightAngle = Mathf.Atan2 (rightY, rightX)*Mathf.Rad2Deg;


		GameObject leftTurret = GameObject.Find ("LeftTurret");
		GameObject rightTurret = GameObject.Find ("RightTurret");
		Vector3 leftRot = Vector3.zero, rightRot = Vector3.zero;

        if (leftAngle < 0 && leftAngle > -180)
            leftRot.z = -leftAngle;
        else
            leftRot.z = leftTurret.transform.eulerAngles.z;

        if (rightAngle > 0 && rightAngle < 180)
            rightRot.z = -rightAngle;
        else
            rightRot.z = rightTurret.transform.eulerAngles.z;

		
		leftTurret.transform.eulerAngles = leftRot;
		rightTurret.transform.eulerAngles = rightRot;

		if (leftFire == 1 && lastLeftFire != 1) {
			GameObject bulletGO = Instantiate(ammoPrefab, leftTurret.transform.position, leftTurret.transform.rotation) as GameObject;
			Bullet b = bulletGO.GetComponent("Bullet") as Bullet;
            b.setDefaults(-leftRot.z, bulletVelocity);
		}

		if (rightFire == 1 && lastRightFire != 1) {
			GameObject bulletGO = Instantiate(ammoPrefab, rightTurret.transform.position, rightTurret.transform.rotation) as GameObject;
			Bullet b = bulletGO.GetComponent("Bullet") as Bullet;
            b.setDefaults(-rightRot.z, bulletVelocity);
		}

		lastLeftFire = leftFire;
		lastRightFire = rightFire;
	}

	// Handles player movement, likely to be replaced with thrusters
	private void UpdatePlayer() {
		float dPadX = Input.GetAxis ("DPad Horizontal");
		float dPadY = Input.GetAxis ("DPad Vertical");

		Vector3 pos = this.transform.position;

		if (dPadY == -1) {
			pos.y += 1*velocityMult*Time.deltaTime;
		}
		if (dPadY == 1) {
			pos.y -= 1*velocityMult*Time.deltaTime;
		}
		if (dPadX == -1) {
			pos.x -= 1*velocityMult*Time.deltaTime;
		}
		if (dPadX == 1) {
			pos.x += 1*velocityMult*Time.deltaTime;
		}

		this.transform.position = pos;
	}
}

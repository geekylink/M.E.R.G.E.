using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : BaseShip {

	public float velocityMult = 1;
	public float bulletVelocity = 1;

	public float bounciness = 0.5f;

	public GameObject ammoPrefab;
    public float fireRate = 0.5f;
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

		if (leftFire == 1 && lastLeftFire <= 0) {
			GameObject bulletGO = Instantiate(ammoPrefab, leftTurret.transform.position, leftTurret.transform.rotation) as GameObject;
			Bullet b = bulletGO.GetComponent("Bullet") as Bullet;
            b.setDefaults(-leftRot.z, bulletVelocity);
            lastLeftFire = fireRate;
		}

		if (rightFire == 1 && lastRightFire <= 0) {
			GameObject bulletGO = Instantiate(ammoPrefab, rightTurret.transform.position, rightTurret.transform.rotation) as GameObject;
			Bullet b = bulletGO.GetComponent("Bullet") as Bullet;
            b.setDefaults(-rightRot.z, bulletVelocity);
            lastRightFire = fireRate;
		}

        lastRightFire -= Time.deltaTime;
        lastLeftFire -= Time.deltaTime;
	}

    void ResetLeftFire()
    {
        lastLeftFire = 0;
    }
    void ResetRightFire()
    {
        lastRightFire = 0;
    }

	// Handles player movement, likely to be replaced with thrusters
	private void UpdatePlayer() {

		bool engineLeft = Input.GetButton("EngineLeft");
		bool engineRight = Input.GetButton("EngineRight");

		if(engineLeft && engineRight){
			
			foreach(GameObject engine in enginesTurnLeft){
				engine.GetComponent<Engine>().TurnOff();
			}
			foreach(GameObject engine in enginesTurnRight){
				engine.GetComponent<Engine>().TurnOff();
			}

			foreach(GameObject engine in enginesStraight){
				engine.GetComponent<Engine>().TurnOn();
			}
		}
		else if(engineLeft){

			foreach(GameObject engine in enginesTurnRight){
				engine.GetComponent<Engine>().TurnOff();
			}
			foreach(GameObject engine in enginesStraight){
				engine.GetComponent<Engine>().TurnOff();
			}

			foreach(GameObject engine in enginesTurnLeft){
				engine.GetComponent<Engine>().TurnOn(true);
			}
		}
		else if(engineRight){
			
			foreach(GameObject engine in enginesTurnLeft){
				engine.GetComponent<Engine>().TurnOff();
			}
			foreach(GameObject engine in enginesStraight){
				engine.GetComponent<Engine>().TurnOff();
			}

			foreach(GameObject engine in enginesTurnRight){
				engine.GetComponent<Engine>().TurnOn(true);
			}
		}
		else{
			foreach(GameObject engine in enginesTurnLeft){
				engine.GetComponent<Engine>().TurnOff();
			}
			foreach(GameObject engine in enginesTurnRight){
				engine.GetComponent<Engine>().TurnOff();
			}
			foreach(GameObject engine in enginesStraight){
				engine.GetComponent<Engine>().TurnOff();
			}
		}
		ClampObjectIntoView ();

		if(Mathf.Abs (rigidbody2D.angularVelocity) > maxRotSpeed){
			rigidbody2D.angularVelocity = maxRotSpeed * rigidbody2D.angularVelocity/(Mathf.Abs (rigidbody2D.angularVelocity));
		}

	}

	void ClampObjectIntoView () {
		float z = transform.position.z-Camera.main.transform.position.z;

		float topPosY = Camera.main.ViewportToWorldPoint(new Vector3(0,1,z)).y;
		float bottomPosY = Camera.main.ViewportToWorldPoint(new Vector3(0,0,z)).y;
		float leftPosX = Camera.main.ViewportToWorldPoint(new Vector3(0,0,z)).x;
		float rightPosX = Camera.main.ViewportToWorldPoint(new Vector3(1,0,z)).x;

		
		Vector2 vel = rigidbody2D.velocity;
		Vector3 pos = transform.position;

		if (transform.position.y>topPosY) {
			vel.y = -vel.y * bounciness;
			pos.y = topPosY;
		} 
		else if (transform.position.y < bottomPosY) {
			vel.y = -vel.y * bounciness;
			pos.y = bottomPosY;
		}
		else if (transform.position.x>rightPosX) {
			vel.x = -vel.x * bounciness;
			pos.x = rightPosX;
		} 
		else if (transform.position.x<leftPosX) {
			vel.x = -vel.x * bounciness;
			pos.x = leftPosX;
		}
		rigidbody2D.velocity = vel;
		transform.position = pos;
	}


}

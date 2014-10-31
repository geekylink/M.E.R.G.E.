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
		//UpdateTurrets ();
		UpdatePlayer ();
		UpdateHUD ();

		lastRightFire -= Time.deltaTime;
		lastLeftFire -= Time.deltaTime;
	}

	private void UpdateHUD() {
		gtHealth.text = "Health: " + health;
	}
	
	// Updates angles for the left and right turrets
	public void UpdateTurrets(float leftAngle, float rightAngle) {
		GameObject leftTurret = GameObject.Find ("LeftTurret");
		GameObject rightTurret = GameObject.Find ("RightTurret");

		Vector3 leftRot = Vector3.zero, rightRot = Vector3.zero;

		leftRot.z = leftAngle;
		rightRot.z = rightAngle;		
		
		leftTurret.transform.eulerAngles = leftRot;
		rightTurret.transform.eulerAngles = rightRot;
	}

	// Fires from the left turret
	public void FireLeftTurret() {
		GameObject leftTurret = GameObject.Find ("LeftTurret");

		if (lastLeftFire <= 0) {
			GameObject bulletGO = Instantiate(ammoPrefab, leftTurret.transform.position, leftTurret.transform.rotation) as GameObject;
			Bullet b = bulletGO.GetComponent("Bullet") as Bullet;
			b.setDefaults(-leftTurret.transform.eulerAngles.z, bulletVelocity);
			lastLeftFire = fireRate;
		}
	}

	// Fires from the right turret
	public void FireRightTurret() {
		GameObject rightTurret = GameObject.Find ("RightTurret");

		if (lastRightFire <= 0) {
			GameObject bulletGO = Instantiate(ammoPrefab, rightTurret.transform.position, rightTurret.transform.rotation) as GameObject;
			Bullet b = bulletGO.GetComponent("Bullet") as Bullet;
			b.setDefaults(-rightTurret.transform.eulerAngles.z, bulletVelocity);
			lastRightFire = fireRate;
		}
	}

	// Fires the engines
	public void FireEngines(bool engineLeft, bool engineRight) {
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

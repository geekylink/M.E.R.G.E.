﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : BaseShip {

	public KeyCode leftFire;
	public KeyCode rightFire;
	public KeyCode mergeButton;

	public float velocityMult = 1;
	public float bulletVelocity = 1;

	public float bounciness = 0.5f;

	public GameObject ammoPrefab;
    public float fireRate = 0.5f;
	public UnityEngine.UI.Text gtHealth;

	public GameObject leftTurret;
	public GameObject rightTurret;

	// Used to prevent firing constantly
	private float lastLeftFire = 0;
	private float lastRightFire = 0;

	bool isCurrentlyMerged = false;
	public bool IsCurrentlyMerged{
		get{return isCurrentlyMerged;}
		set{isCurrentlyMerged = value;}
	}

	bool canMerge = false;
	public bool CanMerge{
		get{return canMerge;}
		set{canMerge = value;}
	}

	bool tryingToMerge = false;
	public bool TryingToMerge{
		get{return tryingToMerge;}
		set{tryingToMerge = value;}
	}

	// Use this for initialization
	void Start () {
		health = maxHealth;
		lastLeftFire = lastRightFire = 0;
	}
	
	// Update is called once per frame
	void Update () {
		//UpdateTurrets ();
		UpdatePlayer ();
		UpdateHUD ();
		
		lastRightFire -= Time.deltaTime;
		lastLeftFire -= Time.deltaTime;
		CheckMerge();
	}

	private void CheckMerge(){
		if(Input.GetKey(mergeButton)){
			tryingToMerge = true;
		}
		else{
			tryingToMerge = false;
		}
	}

	private void UpdateHUD() {
		gtHealth.text = "Health: " + health;
	}
	
	// Updates angles for the left and right turrets
	public void UpdateTurrets(float leftAngle, float rightAngle) {
		Vector3 leftRot = Vector3.zero, rightRot = Vector3.zero;

		leftRot.z = leftAngle;
		rightRot.z = rightAngle;		
		
		leftTurret.transform.eulerAngles = leftRot;
		rightTurret.transform.eulerAngles = rightRot;
	}

	// Fires from the left turret
	public void FireLeftTurret() {
		if (lastLeftFire <= 0) {
			GameObject bulletGO = Instantiate(ammoPrefab, leftTurret.transform.position, leftTurret.transform.rotation) as GameObject;
			Bullet b = bulletGO.GetComponent("Bullet") as Bullet;
			b.setDefaults(-leftTurret.transform.eulerAngles.z, bulletVelocity);
			lastLeftFire = fireRate;
		}
	}

	// Fires from the right turret
	public void FireRightTurret() {
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
			if(rigidbody2D != null){
				this.rigidbody2D.angularVelocity = this.rigidbody2D.angularVelocity / 1.01f;
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
		bool engLeft = Input.GetKey(leftFire);
		bool engRight = Input.GetKey(rightFire);

		FireEngines(engLeft, engRight);

		ClampObjectIntoView ();

		if(rigidbody2D != null){
			if(Mathf.Abs (rigidbody2D.angularVelocity) > maxRotSpeed){
				rigidbody2D.angularVelocity = maxRotSpeed * rigidbody2D.angularVelocity/(Mathf.Abs (rigidbody2D.angularVelocity));
			}
		}
	}

	void ClampObjectIntoView () {
		if(rigidbody2D == null) return;

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

	public void ChangeColor(Color color){
		Transform body = transform.FindChild("Body");
		body.GetComponent<SpriteRenderer>().color = color;
	}
	
	public Color GetColor(){
		Transform body = transform.FindChild("Body");
		Color returnColor = body.GetComponent<SpriteRenderer>().color;
		return returnColor;
	}

}
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : BaseShip {

	//Used for testing on keyboard - when I don't have 4 xbox controllers
	public KeyCode leftFire;
	public KeyCode rightFire;
	public KeyCode mergeButton;
	public KeyCode unmergeButton;

	//easily editable variables in the inspector
	public float velocityMult = 1;
	public float bulletVelocity = 1;
	public float gravityMult = 0.5f;
	public float breakMult = 0.01f;

	public float bounciness = 0.5f;

	public GameObject ammoPrefab;
	public GameObject autoTurretPrefab;
	public GameObject healSatPrefab;
	public GameObject mineSatPrefab;

    public float fireRate = 0.5f;
	public UnityEngine.UI.Text gtHealth;
	public UnityEngine.UI.Text gtRes;

	//turrets should be assigned here in the inspector
	public GameObject leftTurret;
	public GameObject rightTurret;

	// Used to prevent firing constantly
	private float lastLeftFire = 0;
	private float lastRightFire = 0;

	//lots of things to deal with merging
	bool isMerging = false;
	public bool IsMerging{
		get{return isMerging;}
		set{isMerging = value;}
	}

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

	public GameObject minimapBlip;
	public Color playerColor;
	public GameObject body;
	public GameObject leftEnginePiece;
	public GameObject rightEnginePiece;

	private int numResources;

	// Use this for initialization
	void Start () {
		numResources = 0;
		health = maxHealth;
		lastLeftFire = lastRightFire = 0;
		minimapBlip.renderer.material.color = playerColor;
		body.GetComponent<SpriteRenderer>().color = playerColor;

		enginesTurnLeft[0].particleSystem.enableEmission = true;
		enginesTurnRight[0].particleSystem.enableEmission = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(rigidbody2D){
			RestrictToMap();
		}
		//UpdateTurrets ();
		UpdatePlayer ();
		UpdateHUD ();
		
		lastRightFire -= Time.deltaTime;
		lastLeftFire -= Time.deltaTime;
		
		//comment out this line when using controllers
		//CheckMerge(Input.GetKey(mergeButton), Input.GetKey (unmergeButton));
	}

	
	public override void Die(){
		UpdateHUD();
		if(isCurrentlyMerged){
			MergeManager.S.Unmerge(this);
		}
		base.Die();
	}

	//checking to see if the player wants to merge/unmerge
	public void CheckMerge(bool pushingMerge, bool pushingUnmerge){
		if(isMerging) return;

		if(pushingMerge){
			tryingToMerge = true;
		}
		else{
			tryingToMerge = false;
		}

		if(isCurrentlyMerged && pushingUnmerge){
			MergeManager.S.Unmerge(this);
		}
	}


	private void UpdateHUD() {
		gtHealth.text = "Health: " + health;
		gtRes.text = "Resources: " + numResources;
	}
	
	// Updates angles for the left and right turrets
	public void UpdateTurrets(float leftAngle, float rightAngle) {
		if(leftAngle == 0 && rightAngle == 0){
			return;
		}

		Vector3 leftRot = Vector3.zero, rightRot = Vector3.zero;

		leftRot.z = leftAngle;
		rightRot.z = rightAngle;		
		
		leftTurret.transform.eulerAngles = leftRot;
		rightTurret.transform.eulerAngles = rightRot;
		if(leftAngle != 0){
			FireLeftTurret();
		}
		if(rightAngle != 0){
			FireRightTurret();
		}

	}

	// Fires from the left turret
	public void FireLeftTurret() {
		if (lastLeftFire <= 0) {
			GameObject bulletGO = Instantiate(ammoPrefab, leftTurret.transform.position, leftTurret.transform.rotation) as GameObject;
			Bullet b = bulletGO.GetComponent("Bullet") as Bullet;

			int id = MergeManager.S.players.IndexOf(this);
			b.damageDealt = 1 + MergeManager.S.currentlyMergedWith[id].Count;

			b.setDefaults(-leftTurret.transform.eulerAngles.z, bulletVelocity);
			b.rigidbody2D.velocity += transform.root.rigidbody2D.velocity;
			lastLeftFire = fireRate;
		}
	}

	// Fires from the right turret
	public void FireRightTurret() {
		if (lastRightFire <= 0) {
			GameObject bulletGO = Instantiate(ammoPrefab, rightTurret.transform.position, rightTurret.transform.rotation) as GameObject;
			Bullet b = bulletGO.GetComponent("Bullet") as Bullet;

			int id = MergeManager.S.players.IndexOf(this);
			b.damageDealt = 1 + MergeManager.S.currentlyMergedWith[id].Count;

			b.setDefaults(-rightTurret.transform.eulerAngles.z, bulletVelocity);
			b.rigidbody2D.velocity += transform.root.rigidbody2D.velocity;
			lastRightFire = fireRate;
		}
	}

	public void FlyForward(float speed, float actualSpeed){

		Vector2 finalSpeed = ((Vector2)transform.right * speed * actualSpeed) / transform.root.rigidbody2D.mass;

		transform.root.rigidbody2D.velocity = Vector2.Lerp(transform.root.rigidbody2D.velocity, finalSpeed, Time.deltaTime * 2);



		enginesTurnLeft[0].particleSystem.startLifetime = speed / 3.0f;
		enginesTurnRight[0].particleSystem.startLifetime = speed / 3.0f;

	}

	public void Turn(float turnDir, float speed){
		if(Mathf.Abs (turnDir) < 0.1f){
			if(rigidbody2D != null){
				this.rigidbody2D.angularVelocity = this.rigidbody2D.angularVelocity / 1.1f;
			}
			return;
		}

		rigidbody2D.angularVelocity += turnDir * speed;
	}

	public void TurnTowards(float angle){
		if(angle == 0){
			return;
		}

		Vector3 turnVector = Vector3.zero;
		turnVector.z = angle;
		transform.rotation =  Quaternion.Lerp(transform.rotation, Quaternion.Euler(turnVector), Time.deltaTime * 5);

	}

	// Fires the engines
	public void FireEngines(bool engineLeft, bool engineRight) {
		if(isMerging) return;

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
				this.rigidbody2D.angularVelocity = this.rigidbody2D.angularVelocity / 5.01f;
			}
		}
		else if(engineLeft){
			useBreaks();
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
			useBreaks();
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
			useBreaks();
			
			if(rigidbody2D != null){
				this.rigidbody2D.angularVelocity = this.rigidbody2D.angularVelocity / 5.01f;
			}
		}
	}

	public void useBreaks() {
		Vector3 vel = transform.root.rigidbody2D.velocity;
		vel *= breakMult*Time.fixedDeltaTime;
		transform.root.rigidbody2D.velocity = vel;
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

		if(rigidbody2D != null){
			if(Mathf.Abs (rigidbody2D.angularVelocity) > maxRotSpeed){
				rigidbody2D.angularVelocity = maxRotSpeed * rigidbody2D.angularVelocity/(Mathf.Abs (rigidbody2D.angularVelocity));
			}
		}
	}

	//clamps object when using a main camera (all players in same view, so probably want some way to clamp it)
	void ClampObjectIntoView () {
		if(rigidbody2D == null) return;
		if(Camera.main == null) return;

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
    
	public void GetResources(int amount) {
		numResources += amount;
	}

	// Spawns a turret
	public void SpawnTurret(BaseSatellite.SatelliteType Type, GameObject orbitObj) {
		switch (Type) {
		case BaseSatellite.SatelliteType.TURRET:
			GameObject autoSat = Instantiate (autoTurretPrefab, this.transform.position, this.transform.rotation) as GameObject;
			TurretSatellite satTurret = autoSat.GetComponent ("TurretSatellite") as TurretSatellite;
			satTurret.orbitTarget = orbitObj;
			satTurret.creatorObj = this.gameObject;
			satTurret.targetObject = this.gameObject;
			break;
		case BaseSatellite.SatelliteType.HEALER:
			GameObject healSat = Instantiate (healSatPrefab, this.transform.position, this.transform.rotation) as GameObject;
			HealerSatellite satHealer = healSat.GetComponent ("HealerSatellite") as HealerSatellite;
			satHealer.orbitTarget = orbitObj;
			satHealer.creatorObj = this.gameObject;
			break;
		case BaseSatellite.SatelliteType.MINER:
			GameObject mineSat = Instantiate (mineSatPrefab, this.transform.position, this.transform.rotation) as GameObject;
			MinerSatellite satMine = mineSat.GetComponent ("MinerSatellite") as MinerSatellite;
			satMine.orbitTarget = orbitObj;
			satMine.creatorObj = this.gameObject;
			break;
		}
	}
}

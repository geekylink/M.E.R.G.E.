using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : BaseShip {
	
	//easily editable variables in the inspector
	public float velocityMult = 1;
	public float bulletVelocity = 1;
	public float gravityMult = 0.5f;

	public float bounciness = 0.5f;

	public GameObject ammoPrefab;
	public GameObject autoTurretPrefab;
	public GameObject healSatPrefab;
	public GameObject mineSatPrefab;

    public float fireRate = 0.5f;
	public UnityEngine.UI.Text gtRes;

	//turrets should be assigned here in the inspector
	public GameObject rightTurret;

	// Used to prevent firing constantly
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
	public GameObject rightEnginePiece;
	public GameObject leftEnginePiece;

	private int numResources;

	public int playerManagerArrayPos = -1;

	// Use this for initialization
	void Start () {
		numResources = 0;
		health = maxHealth;
		minimapBlip.renderer.material.color = playerColor;
		body.GetComponent<SpriteRenderer>().color = playerColor;

		leftEnginePiece.particleSystem.enableEmission = true;
		rightEnginePiece.particleSystem.enableEmission = true;
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
	}

	
	public override void Die(){
		//UpdateHUD();
		if(isCurrentlyMerged){
			MergeManager.S.Unmerge(this);
		}

		PlayerManager.S.PlayerDied(this, playerManagerArrayPos);

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
		//gtHealth.text = "Health: " + health;
		gtRes.text = "Resources: " + numResources;
	}
	
	// Updates angles for the left and right turrets
	public void UpdateTurrets(float rightAngle) {
		if(rightAngle == 0){
			return;
		}

		Vector3 rightRot = Vector3.zero;

		rightRot.z = rightAngle;		

		rightTurret.transform.eulerAngles = rightRot;
		if(rightAngle != 0){
			FireRightTurret();
		}

	}

	// Fires from the right turret
	public void FireRightTurret() {
		if (lastRightFire <= 0) {
			GameObject bulletGO = Instantiate(ammoPrefab, rightTurret.transform.position, rightTurret.transform.rotation) as GameObject;
			Bullet b = bulletGO.GetComponent("Bullet") as Bullet;

			int id = MergeManager.S.players.IndexOf(this);
			b.damageDealt = 1 + MergeManager.S.currentlyMergedWith[id].Count;

			b.setDefaults(-rightTurret.transform.eulerAngles.z, bulletVelocity + transform.root.rigidbody2D.velocity.magnitude);
			//b.rigidbody2D.velocity += transform.root.rigidbody2D.velocity;
			lastRightFire = fireRate;
		}
	}

	public void FlyForward(float speed, float actualSpeed){

		if(transform.parent != null){
			MergedShip parentShip = transform.root.GetComponent<MergedShip>();
			parentShip.Fly(speed, actualSpeed);
			return;
		}

		Vector2 finalSpeed = ((Vector2)transform.right * speed * actualSpeed) / transform.root.rigidbody2D.mass;

		transform.root.rigidbody2D.velocity = Vector2.Lerp(transform.root.rigidbody2D.velocity, finalSpeed, Time.deltaTime * 2);

		leftEnginePiece.particleSystem.startLifetime = speed / 3.0f;
		rightEnginePiece.particleSystem.startLifetime = speed / 3.0f;

	}

	public void TurnTowards(float angle){
		if(angle == 0){
			return;
		}

		Vector3 turnVector = Vector3.zero;
		turnVector.z = angle;
		transform.rotation =  Quaternion.Lerp(transform.rotation, Quaternion.Euler(turnVector), Time.deltaTime * 5);

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
		body.GetComponent<SpriteRenderer>().color = color;
		gtRes.color = color;
		minimapBlip.renderer.material.color = color;
		playerColor = color;
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

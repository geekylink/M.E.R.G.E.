using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : BaseShip {
	
	//easily editable variables in the inspector
	public float velocityMult = 1;
	public float bulletVelocity = 1;
	public float gravityMult = 0.5f;
	public float turnSpeed = 5;

	public float bounciness = 0.5f;
	public float camBuffer;
	public float satSpawnRadius = 50;
	public int maxOwnSats = 3;

	public int score = -1;
	public float scoreTimer = 5.0f;

	public GameObject ammoPrefab;
	public GameObject autoTurretPrefab;
	public GameObject healSatPrefab;
	public GameObject mineSatPrefab;

    public float fireRate = 1.0f;
	public UnityEngine.UI.Text gtRes;

	//turrets should be assigned here in the inspector
	public GameObject rightTurret;

	// Used to prevent firing constantly
	private float lastRightFire = 0;
	private float lastScore = 0;

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

	Vector3 gravVector;
	public Vector3 GravVector{
		get{return gravVector;}
		set{gravVector = value;}
	}

	public GameObject minimapBlip;
	public Color playerColor;
	public GameObject body;
	public GameObject rightEnginePiece;
	public GameObject leftEnginePiece;

	private int numResources;

	private ArrayList ownSats;
	List<GameObject> ghostSatellites = new List<GameObject>();
	public int playerManagerArrayPos = -1;

	public CapturePoint planetBeingCaptured;

	// Use this for initialization
	public override void Start () {
		numResources = 0;
		health = maxHealth;
		minimapBlip.renderer.material.color = playerColor;
		body.GetComponent<SpriteRenderer>().color = playerColor;

		ownSats = new ArrayList ();

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
		UpdateUpgrades ();
		
		lastRightFire -= Time.deltaTime;
		lastScore -= Time.deltaTime;
	}

	// Handle the upgrades
	public void UpdateUpgrades() {
		if (lastScore <= 0) {
			score++;
			lastScore = scoreTimer;
		}

		// Halve fire rate every 15 points
		if (score <= 15) {
			fireRate = 1.0f;
		}
		else if (score <= 30) {
			fireRate = 0.5f;
		}
		else if (score <= 45) {
			fireRate = 0.25f;
		}
		else {
			fireRate = 0.125f;
		}
	}

	public void UpdateTurnSpeed(float speedUp){
		turnSpeed += speedUp;
	}

	
	public override void Die(){
		//UpdateHUD();
		//UnshowSats();
		if(planetBeingCaptured){
			planetBeingCaptured.StopCapture ();
		}

		if(isCurrentlyMerged){
			MergeManager.S.Unmerge(this);
		}

		// Remove satellites on death
		foreach (BaseSatellite sat in ownSats) {
			Destroy (sat.gameObject);
		}

		ownSats.Clear ();
		PlayerManager.S.PlayerDied(this, playerManagerArrayPos);

		score = 0;

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
		gtRes.text = "Score: " + score;
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
			b.owner = this;

            b.SetColor(playerColor);
			b.setDefaults(-rightTurret.transform.eulerAngles.z, bulletVelocity + transform.root.rigidbody2D.velocity.magnitude);
			//b.rigidbody2D.velocity += transform.root.rigidbody2D.velocity;
			lastRightFire = fireRate;
		}

		foreach(var satVar in ownSats){
			TurretSatellite ts = satVar as TurretSatellite;
			if(ts){
				Vector2 vel = Vector2.zero;
				vel.y = -Mathf.Sin (-rightTurret.transform.eulerAngles.z*Mathf.Deg2Rad);
				vel.x = Mathf.Cos (-rightTurret.transform.eulerAngles.z*Mathf.Deg2Rad);
				ts.PlayerFire(vel);
			}
		}
	}

	public void Fly(float engineLength, float speed){
		if(isMerging) return;
		
		Vector2 finalSpeed = ((Vector2)transform.right * engineLength * speed + (Vector2)gravVector);
		
		transform.root.rigidbody2D.velocity = Vector2.Lerp(transform.root.rigidbody2D.velocity, finalSpeed, Time.deltaTime * 2);
		
		leftEnginePiece.particleSystem.startLifetime = engineLength / 6.0f;
		rightEnginePiece.particleSystem.startLifetime = engineLength / 6.0f;
	}

	public void ApplyFly(float engineLength, float actualSpeed){


		if(transform.parent != null){
			MergedShip parentShip = transform.root.GetComponent<MergedShip>();
			parentShip.Fly(engineLength, actualSpeed);
			return;
		}
		Fly (engineLength, actualSpeed);
	}

	public void TurnTowards(float angle){
		if(angle == 0){
			return;
		}

		Vector3 turnVector = Vector3.zero;
		turnVector.z = angle;
		transform.rotation =  Quaternion.Lerp(transform.rotation, Quaternion.Euler(turnVector), Time.deltaTime * turnSpeed);

	}

    void ResetRightFire()
    {
        lastRightFire = 0;
    }

	// Handles player movement, likely to be replaced with thrusters
	private void UpdatePlayer() {
		ClampObjectIntoView ();

		if(rigidbody2D != null){
			rigidbody2D.angularVelocity = 0;
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

		if (transform.position.y>topPosY - camBuffer) {
			vel.y = -vel.y * bounciness;
			pos.y = topPosY - camBuffer;
		} 
		else if (transform.position.y < bottomPosY + camBuffer) {
			vel.y = -vel.y * bounciness;
			pos.y = bottomPosY + camBuffer;
		}
		else if (transform.position.x>rightPosX - camBuffer) {
			vel.x = -vel.x * bounciness;
			pos.x = rightPosX - camBuffer;
		} 
		else if (transform.position.x<leftPosX + camBuffer) {
			vel.x = -vel.x * bounciness;
			pos.x = leftPosX + camBuffer;
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


}

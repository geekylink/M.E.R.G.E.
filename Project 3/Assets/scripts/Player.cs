using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : BaseShip {
	
	//easily editable variables in the inspector
	public float velocityMult = 1;
	public float bulletVelocity = 1;
	public float gravityMult = 0.5f;

	public float bounciness = 0.5f;
	public float satSpawnRadius = 50;
	public int maxOwnSats = 3;

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
		
		lastRightFire -= Time.deltaTime;
	}

	
	public override void Die(){
		//UpdateHUD();
		UnshowSats();
		if(isCurrentlyMerged){
			MergeManager.S.Unmerge(this);
		}

		// Remove satellites on death
		foreach (BaseSatellite sat in ownSats) {
			Destroy (sat.gameObject);
		}

		ownSats.Clear ();
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

            b.SetColor(playerColor);
			b.setDefaults(-rightTurret.transform.eulerAngles.z, bulletVelocity + transform.root.rigidbody2D.velocity.magnitude);
			//b.rigidbody2D.velocity += transform.root.rigidbody2D.velocity;
			lastRightFire = fireRate;
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

	public void RemoveSat(BaseSatellite sat){
		ownSats.Remove (sat);
	}

	void SpawnSpecificSat(GameObject prefab, GameObject orbitObj, CapturePoint planet){
		GameObject autoSat = Instantiate (prefab, orbitObj.transform.position, this.transform.rotation) as GameObject;
		BaseSatellite satTurret = autoSat.GetComponent ("BaseSatellite") as BaseSatellite;
		satTurret.orbitTarget = orbitObj;
		satTurret.creatorObj = this.gameObject;
		satTurret.playerWhoSpawned = playerManagerArrayPos;
		if (orbitObj != this.gameObject) {
			planet.AddSat (satTurret, CapturePoint.ControlledBy.Player);
			satTurret.orbiting = BaseSatellite.OrbitingType.Planet;
			
			autoSat.layer = 8;
		} else {
			if (orbitObj == this.gameObject) {
				// Position each satellite equal distance apart
				float startAngle = ((2 * Mathf.PI) / maxOwnSats);
				if (ownSats.Count > 0) {
					BaseSatellite sat = ownSats[ownSats.Count-1] as BaseSatellite;
					startAngle += sat.orbitAngle;
				}
				satTurret.SetStartAngle (startAngle);
				ownSats.Add (satTurret);
				satTurret.orbiting = BaseSatellite.OrbitingType.Player;

			}
			satTurret.team = BaseSatellite.SatelliteTeam.Player;
			autoSat.layer = 8;
		}
	}

	void GetOrbitObj(ref GameObject orbitObj, ref CapturePoint planet){
		GameObject[] planetObjs = GameObject.FindGameObjectsWithTag ("Planet");
		foreach (GameObject planetObj in planetObjs) {
			Vector3 dist = planetObj.transform.position - this.transform.position;
			if (dist.magnitude < satSpawnRadius) {
				planet = planetObj.GetComponent<CapturePoint>();
				if(!planet.CanAddSat(CapturePoint.ControlledBy.Player)) continue;
				orbitObj = planetObj;
				break;
			}
		}

	}

	// Spawns a turret
	public void SpawnTurret(BaseSatellite.SatelliteType Type) {
		GameObject orbitObj = this.gameObject;
		CapturePoint planet = null;
		GetOrbitObj(ref orbitObj, ref planet);

		if(planet != null && orbitObj == this.gameObject){
			return;
		}

		if(orbitObj == this.gameObject){
			// Limit the number of satellites we can make
			if (ownSats.Count >= maxOwnSats) {
				return;
			}
		}

		switch (Type) {
		case BaseSatellite.SatelliteType.TURRET:
			SpawnSpecificSat(autoTurretPrefab, orbitObj, planet);
			break;
		case BaseSatellite.SatelliteType.HEALER:
			SpawnSpecificSat(healSatPrefab, orbitObj, planet);
			break;
		case BaseSatellite.SatelliteType.MINER:
			// Only spawn miners on planets
			if (orbitObj != this.gameObject) {
				SpawnSpecificSat(mineSatPrefab, orbitObj, planet);
			}
			break;
		}
	}

	
	
	public void UnshowSats(){
		for(int i = 0; i < ghostSatellites.Count; ++i){
			Destroy(ghostSatellites[i]);
		}
		ghostSatellites.RemoveRange(0, ghostSatellites.Count);
	}
	
	public void ShowSats(){
		UnshowSats();
		
		
		GameObject orbitObj = this.gameObject;
		CapturePoint planet = null;
		GetOrbitObj(ref orbitObj, ref planet);

		
		if(planet != null && orbitObj == this.gameObject){
			return;
		}

		if(orbitObj == this.gameObject){
			// Limit the number of satellites we can make
			if (ownSats.Count >= maxOwnSats) {
				return;
			}
		}
		
		
		GameObject turret = new GameObject();
		turret.transform.position = orbitObj.transform.position + Vector3.down * 7;
		turret.AddComponent<SpriteRenderer>();
		turret.GetComponent<SpriteRenderer>().sprite = autoTurretPrefab.GetComponent<SpriteRenderer>().sprite;
		Vector3 temp = turret.transform.localScale * 2;
		turret.transform.localScale = temp;
		
		GameObject healer = new GameObject();
		healer.transform.position = orbitObj.transform.position + Vector3.up * 7;
		healer.AddComponent<SpriteRenderer>();
		healer.GetComponent<SpriteRenderer>().sprite = healSatPrefab.GetComponent<SpriteRenderer>().sprite;
		temp = healer.transform.localScale * 2;
		healer.transform.localScale = temp;
		
		GameObject miner = new GameObject();
		miner.transform.position = orbitObj.transform.position + Vector3.right * 7;
		miner.AddComponent<SpriteRenderer>();
		miner.GetComponent<SpriteRenderer>().sprite = mineSatPrefab.GetComponent<SpriteRenderer>().sprite;
		temp = miner.transform.localScale * 2;
		miner.transform.localScale = temp;
		
		ghostSatellites.Add (turret);
		ghostSatellites.Add (healer);
		ghostSatellites.Add (miner);
	}
}

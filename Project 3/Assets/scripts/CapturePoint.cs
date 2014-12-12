using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CapturePoint : MonoBehaviour {

	public enum CaptureType{
		Planet,
		Satellite
	}

	public enum ControlledBy{
		Player,
		Enemy,
		Neutral
	}
	public Color color;

	public GameObject planetObj;
	SpriteRenderer sr;
	public GameObject minimapBlip;

	public CaptureType captureType;
	public ControlledBy controlledBy;

	public GameObject autoTurretPrefab;
	public GameObject enemyAutoTurret;
	public GameObject linearBarPrefab;
/*
	bool allDestroyed = false;
	SubCapturePoint[] subPoints;*/

	public int maxSatellites;
	public List<BaseSatellite> satsInOrbit = new List<BaseSatellite>();

	public float angle;

	public bool beingCaptured = false;
	public GameObject turretBeingBuilt;
	GameObject linearBar;
	bool buildingTurrets = false;
	bool secondTurretSpawnPhase = false;

	GameObject prompt;

	public float attackingSquadDistance;

	// Use this for initialization
	void Start () {
		sr = planetObj.GetComponent<SpriteRenderer>();
		ChangeController();
	}

	void Awake(){
		//If already indicated that it is controlled by a specific team, create an initial satellite, then begin spawning more satellites
		if(controlledBy == ControlledBy.Player){			
			GameObject autoSat = Instantiate (autoTurretPrefab, this.transform.position, this.transform.rotation) as GameObject;
			TurretSatellite satTurret = autoSat.GetComponent ("TurretSatellite") as TurretSatellite;
			satTurret.orbitTarget = this.gameObject;
			satTurret.creatorObj = this.gameObject;

			satsInOrbit.Add (satTurret);
			
		}
		if(controlledBy == ControlledBy.Enemy){
			GameObject autoSat = Instantiate (enemyAutoTurret, this.transform.position, this.transform.rotation) as GameObject;
			TurretSatellite satTurret = autoSat.GetComponent ("TurretSatellite") as TurretSatellite;
			satTurret.orbitTarget = this.gameObject;
			satTurret.creatorObj = this.gameObject;
			satsInOrbit.Add (satTurret);
		}

		BaseSatellite bs = gameObject.GetComponent<BaseSatellite>();
		bs.SetStartAngle(Mathf.Deg2Rad * angle);
	}

	public bool CanAddSat(ControlledBy tryingToAdd){
		if(satsInOrbit.Count >= maxSatellites) return false;
		if(tryingToAdd == ControlledBy.Enemy && controlledBy == ControlledBy.Player) return false;
		if(controlledBy == ControlledBy.Enemy && tryingToAdd == ControlledBy.Player) return false;
		return true;
	}

	public void RemoveSat(BaseSatellite sat){
		satsInOrbit.Remove(sat);
		if(satsInOrbit.Count == 0 && !secondTurretSpawnPhase){
			if(buildingTurrets && !beingCaptured){
				buildingTurrets = false;
				
				StopCoroutine("SpawnTurret");
				if(turretBeingBuilt) Destroy (turretBeingBuilt);
				if(linearBar) Destroy(linearBar);
			}
			if(controlledBy == ControlledBy.Player){
				controlledBy = ControlledBy.Enemy;
				GameObject autoSat = Instantiate (enemyAutoTurret, this.transform.position, this.transform.rotation) as GameObject;
				TurretSatellite satTurret = autoSat.GetComponent ("TurretSatellite") as TurretSatellite;
				satTurret.orbitTarget = this.gameObject;
				satTurret.creatorObj = this.gameObject;
				satsInOrbit.Add (satTurret);
			}
			else{
				controlledBy = ControlledBy.Neutral;
			}
			ChangeController();
		}

	}

	/// <summary>
	/// Changes the color of the planet based on who now controls it, 
	/// and checks to see if boss spawn condition is met (done in GameManager.cs)
	/// </summary>
	void ChangeController(){
		if(controlledBy == ControlledBy.Player){
			sr.color = Color.blue;
			minimapBlip.renderer.material.color = Color.blue;
			color = new Color(.2f, .2f, 1);

		}
		if(controlledBy == ControlledBy.Enemy){
			sr.color = Color.red;
			color = Color.red;
			minimapBlip.renderer.material.color = Color.red;
		}
		if(controlledBy == ControlledBy.Neutral){
			sr.color = Color.grey;
			color = Color.grey;
			minimapBlip.renderer.material.color = Color.grey;
		}
		if(GameManager.S){
			GameManager.S.CheckForBossSpawn();

		}
		gameObject.GetComponent<PlanetOrbitLine>().UpdateOrbitLine();
		GameManager.S.CheckFurthestPlanet();
	}

	/// <summary>
	/// Spawns the turret, then changes controller of planet (may be unnecessary in some cases)
	/// Then, if more planets can be spawned, runs the coroutine again
	/// </summary>
	/// <param name="parms">Parms: parms[0] = time takes to spawn. parms[1] = controller of turret/planet</param>
	IEnumerator SpawnTurret( object[] parms){
		float time = (float)parms[0];
		ControlledBy controller = (ControlledBy)parms[1];
		buildingTurrets = true;

		//Begin spawning the turret
		//This just creates a game object that is basically only a sprite,
		//then slowly increases its alpha as time goes on.
		turretBeingBuilt = new GameObject();
		turretBeingBuilt.AddComponent<SpriteRenderer>();
		if(controller == ControlledBy.Player){
			turretBeingBuilt.GetComponent<SpriteRenderer>().sprite = autoTurretPrefab.GetComponent<SpriteRenderer>().sprite;
		}
		else{
			turretBeingBuilt.GetComponent<SpriteRenderer>().sprite = enemyAutoTurret.GetComponent<SpriteRenderer>().sprite;
		}
		Vector3 temp = turretBeingBuilt.transform.localScale * 2;
		turretBeingBuilt.transform.localScale = temp;
		turretBeingBuilt.transform.position = transform.position;
		turretBeingBuilt.transform.parent = gameObject.transform;

		SpriteRenderer ts = turretBeingBuilt.GetComponent<SpriteRenderer>();

		if(linearBar != null) Destroy(linearBar);
		linearBar = Instantiate(linearBarPrefab) as GameObject;
		linearBar.transform.position = transform.position - Vector3.up * 4;
		linearBar.transform.parent = gameObject.transform;
		LinearBar lb = linearBar.GetComponent<LinearBar>();

		//increase alpha as time goes on
		float t = 0;
		while(t < 1){
			t += Time.deltaTime * Time.timeScale / time;
			Color c = ts.material.color;
			c.a = t;
			ts.material.color = c;

			lb.SetPercent(t);
			yield return 0;
		}

		//This part really only matters if it were the first turret being built
		//on the planet, but it can't really hurt either way

		Destroy (linearBar);
		beingCaptured = false;
		controlledBy = controller;
		ChangeController();

		secondTurretSpawnPhase = true;
		//Get the angle it will start at, and lerp it to that position
		float betweenAngle = ((2 * Mathf.PI) / maxSatellites);
		float turretAngle = betweenAngle;


		Vector3 startingPos = turretBeingBuilt.transform.position;

		float lastSatAngle = 0;

		List<float> usedAngles = new List<float>();
		
		Vector3 moveToPos = transform.position;
		t = 0;
		while(t < 1){
			usedAngles.RemoveRange(0, usedAngles.Count);
			if(satsInOrbit.Count > 0){
				foreach(BaseSatellite sat in satsInOrbit){
					usedAngles.Add (sat.orbitAngle);
				}
			}

			foreach(float angle in usedAngles){
				float possibleAngle = angle + betweenAngle;
				bool isTheAngle = true;
				foreach(float newAngle in usedAngles){
					if(Mathf.Abs (possibleAngle - newAngle) * Mathf.Rad2Deg < 5){
						isTheAngle = false;
						break;
					}
				}
				if(isTheAngle){
					lastSatAngle = angle;
					break;
				}
			}
			turretAngle = lastSatAngle + betweenAngle;
			moveToPos.x = transform.position.x + (Mathf.Cos (turretAngle) * 10);
			moveToPos.y = transform.position.y + (Mathf.Sin (turretAngle) * 10);
			t += Time.deltaTime * Time.timeScale / 2;
			turretBeingBuilt.transform.position = Vector3.Lerp(startingPos, moveToPos, t);
			yield return 0;
		}
		//Don't need that fake turret anymore, so destroy it. . .
		Destroy(turretBeingBuilt);
		secondTurretSpawnPhase = false;


		//. . .and instantiate the actual turret
		//Setup some initial variables, and spawn another turret, if possible
		GameObject autoSat;
		if(controller == ControlledBy.Player){
			autoSat = Instantiate (autoTurretPrefab, moveToPos, this.transform.rotation) as GameObject;

			if (satsInOrbit.Count == 0) {
				SFXManager man = SFXManager.getManager ();
				man.playSound ("Capture");
			}
		}
		else{
			autoSat = Instantiate (enemyAutoTurret, moveToPos, this.transform.rotation) as GameObject;
		}
		BaseSatellite satTurret = autoSat.GetComponent ("BaseSatellite") as BaseSatellite;
		satTurret.orbitTarget = this.gameObject;
		satTurret.creatorObj = this.gameObject;

		satTurret.SetStartAngle (turretAngle);
		satsInOrbit.Add (satTurret);
		buildingTurrets = false;

	}

	//Capture a neutral planet by calling the coroutine above
	public void Capture(float captureTime, ControlledBy controller){
		beingCaptured = true;
		object[] parms = new object[2]{captureTime, controller};
		StartCoroutine("SpawnTurret", parms);
	}

	//Stop capturing a planet - stop building the current turret, etc.
	public void StopCapture(){
		if(!beingCaptured) return;
		beingCaptured = false;
		StopCoroutine("SpawnTurret");
		if(turretBeingBuilt) Destroy (turretBeingBuilt);
		if(linearBar) Destroy(linearBar);
	}

	
	// Update is called once per frame
	void Update () {
		//Check to see if neutral
		//If so, stop building turrets
		if(controlledBy == ControlledBy.Neutral){
			if(buildingTurrets && !beingCaptured && !secondTurretSpawnPhase){
				buildingTurrets = false;
				
				StopCoroutine("SpawnTurret");
				if(turretBeingBuilt) Destroy (turretBeingBuilt);
				if(linearBar) Destroy(linearBar);
			}
		}

		//Check to see if planet has less than max turrets
		//and is not building any (can't be neutral obviously
		if(satsInOrbit.Count < maxSatellites && controlledBy != ControlledBy.Neutral && !buildingTurrets){
			object[] parms = new object[2]{15f * satsInOrbit.Count, controlledBy};
			StartCoroutine("SpawnTurret", parms);
		}


		//Check to see if any players are close enough to capture the planet (if neutral)
		//and show the capture prompt
		if(PlayerManager.S.players == null) return;


		bool shouldNotHavePrompt = true;

		if(controlledBy == ControlledBy.Neutral && !beingCaptured){
			foreach(GameObject p in PlayerManager.S.players){
				if(!p) continue;
				if((p.transform.position - transform.position).magnitude < PlayerManager.S.captureDistance){
					shouldNotHavePrompt = false;
					if(prompt) break;
					prompt = Instantiate(PlayerManager.S.capturePrompt, transform.position, Quaternion.identity) as GameObject;
					prompt.transform.parent = gameObject.transform;
					break;
				}
			}
		}
		if(shouldNotHavePrompt){
			if(prompt){
				Destroy(prompt);
			}
		}
	}
}

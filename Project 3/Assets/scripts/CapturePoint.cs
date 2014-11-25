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

	public GameObject planetObj;
	SpriteRenderer sr;
	public GameObject minimapBlip;

	public CaptureType captureType;
	public ControlledBy controlledBy;

	public GameObject autoTurretPrefab;
/*
	bool allDestroyed = false;
	SubCapturePoint[] subPoints;*/

	public int maxSatellites;
	public List<BaseSatellite> satsInOrbit = new List<BaseSatellite>();

	public float angle;

	// Use this for initialization
	void Start () {
		sr = planetObj.GetComponent<SpriteRenderer>();
		ChangeController();
	}

	void Awake(){
		if(controlledBy == ControlledBy.Player){			
			GameObject autoSat = Instantiate (autoTurretPrefab, this.transform.position, this.transform.rotation) as GameObject;
			TurretSatellite satTurret = autoSat.GetComponent ("TurretSatellite") as TurretSatellite;
			satTurret.orbitTarget = this.gameObject;
			satTurret.creatorObj = this.gameObject;
			autoSat.layer = 8;
			satTurret.orbiting = BaseSatellite.OrbitingType.Planet;
			satTurret.team = BaseSatellite.SatelliteTeam.Player;

			satsInOrbit.Add (satTurret);
			
		}
		if(controlledBy == ControlledBy.Enemy){
			GameObject autoSat = Instantiate (autoTurretPrefab, this.transform.position, this.transform.rotation) as GameObject;
			TurretSatellite satTurret = autoSat.GetComponent ("TurretSatellite") as TurretSatellite;
			satTurret.orbitTarget = this.gameObject;
			satTurret.creatorObj = this.gameObject;
			autoSat.layer = 10;
			satTurret.orbiting = BaseSatellite.OrbitingType.Planet;
			satTurret.team = BaseSatellite.SatelliteTeam.Enemy;
			satsInOrbit.Add (satTurret);
		}

		BaseSatellite bs = gameObject.GetComponent<BaseSatellite>();
		bs.SetStartAngle(Mathf.Deg2Rad * angle);
		//subPoints = GetComponentsInChildren<SubCapturePoint> ();
	}

	public bool CanAddSat(ControlledBy tryingToAdd){
		if(satsInOrbit.Count >= maxSatellites) return false;
		if(tryingToAdd == ControlledBy.Enemy && controlledBy == ControlledBy.Player) return false;
		if(controlledBy == ControlledBy.Enemy && tryingToAdd == ControlledBy.Player) return false;
		return true;
	}

	public void AddSat(BaseSatellite sat, ControlledBy capturedBy){
		float startAngle = ((2 * Mathf.PI) / maxSatellites);
		if (satsInOrbit.Count > 0) {
			BaseSatellite lastSat = satsInOrbit[satsInOrbit.Count-1] as BaseSatellite;
			startAngle += lastSat.orbitAngle;
		}
		sat.SetStartAngle (startAngle);

		satsInOrbit.Add (sat);
		controlledBy = capturedBy;
		ChangeController();
	}

	public void RemoveSat(BaseSatellite sat){
		satsInOrbit.Remove(sat);

		if(satsInOrbit.Count == 0){
			controlledBy = ControlledBy.Neutral;
			ChangeController();
		}

	}

	void ChangeController(){
		if(controlledBy == ControlledBy.Player){
			sr.color = Color.blue;
			minimapBlip.renderer.material.color = Color.blue;

		}
		if(controlledBy == ControlledBy.Enemy){
			sr.color = Color.red;
			minimapBlip.renderer.material.color = Color.red;
		}
		if(controlledBy == ControlledBy.Neutral){
			sr.color = Color.grey;
			minimapBlip.renderer.material.color = Color.grey;
		}
		if(GameManager.S){
			GameManager.S.CheckForBossSpawn();

		}
	}


	
	// Update is called once per frame
	void Update () {
	}
}

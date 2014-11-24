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
			satTurret.targetObject = this.gameObject;
			autoSat.layer = 8;
			satTurret.orbiting = BaseSatellite.OrbitingType.Planet;
			
		}
		if(controlledBy == ControlledBy.Enemy){
			GameObject autoSat = Instantiate (autoTurretPrefab, this.transform.position, this.transform.rotation) as GameObject;
			TurretSatellite satTurret = autoSat.GetComponent ("TurretSatellite") as TurretSatellite;
			satTurret.orbitTarget = this.gameObject;
			satTurret.creatorObj = this.gameObject;
			satTurret.targetObject = this.gameObject;
			autoSat.layer = 10;
			satTurret.orbiting = BaseSatellite.OrbitingType.Planet;
		}
		//subPoints = GetComponentsInChildren<SubCapturePoint> ();
	}

	public bool CanAddSat(){
		if(satsInOrbit.Count >= maxSatellites) return false;
		return true;
	}

	public void AddSat(BaseSatellite sat, ControlledBy capturedBy){
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
	}


	
	// Update is called once per frame
	void Update () {
	}
}

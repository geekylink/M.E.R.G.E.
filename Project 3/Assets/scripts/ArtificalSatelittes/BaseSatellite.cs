using UnityEngine;
using System.Collections;

public class BaseSatellite : MonoBehaviour {

	public int orbitRadius = 10;
	public float orbitSpeed = 0.01f;
	public GameObject orbitTarget;
	
	private float orbitAngle = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		UpdateOrbit ();
	}

	// Updates the position of the turret's orbit
	protected void UpdateOrbit() {
		orbitAngle += orbitSpeed;
		
		Vector3 pos = this.transform.position;
		pos.x = orbitTarget.transform.position.x + (Mathf.Cos (orbitAngle) * orbitRadius);
		pos.y = orbitTarget.transform.position.y + (Mathf.Sin (orbitAngle) * orbitRadius);
		
		this.transform.position = pos;
	}
}

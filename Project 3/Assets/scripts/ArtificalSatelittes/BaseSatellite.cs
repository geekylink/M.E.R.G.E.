using UnityEngine;
using System.Collections;

public class BaseSatellite : MonoBehaviour {
	public enum OrbitingType {Player, Planet};
	public enum SatelliteType { TURRET, HEALER, MINER };

	public int orbitRadius = 10;
	public float orbitSpeed = 0.01f;
	public GameObject orbitTarget;
	public GameObject creatorObj;
	public GameObject explosion;

	public OrbitingType orbiting;

	public int health;


	private float orbitAngle = 0;

	// Use this for initialization
	void Start () {

		//This puts the satellite at the angle that is set in the inspector (for planets and such that are already spawned)
		float angle = Vector3.Angle(transform.position - orbitTarget.transform.position, Vector3.right);
		float sign = Mathf.Sign(Vector3.Dot(transform.position - orbitTarget.transform.position, Vector3.right));
		orbitAngle = Mathf.Deg2Rad * sign * angle;
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

	public void Die(){
		if(orbiting == OrbitingType.Planet){
			CapturePoint controller = orbitTarget.GetComponent<CapturePoint>();
			controller.RemoveSat(this);
		}
		else{
			Player controller = orbitTarget.GetComponent<Player>();
			controller.RemoveSat(this);
		}

		if (explosion != null)
		{
			Instantiate(explosion, this.transform.position, Quaternion.identity);
		}

		Destroy(this.gameObject);
	}

	public void TakeDamage(int damage){
		health -= damage;
		if (health <= 0){
			Die ();
		}
	}
}

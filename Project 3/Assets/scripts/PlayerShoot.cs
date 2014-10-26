using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour {

	public GameObject projectile;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown ("f")){
			FireProjectiles();
		}
	}

	void FireProjectiles(){
		GameObject newProj = (GameObject)Instantiate (projectile, transform.position, Quaternion.identity);
		newProj.GetComponent<Projectile> ().SetVelocity (Vector3.up);
	}
}

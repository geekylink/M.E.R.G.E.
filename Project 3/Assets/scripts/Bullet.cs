using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public void setDefaults(float angle, float velocity) {
		Vector3 vel = Vector3.zero;
		vel.y = -Mathf.Sin (angle*Mathf.Deg2Rad)*velocity;
		vel.x = Mathf.Cos (angle*Mathf.Deg2Rad)*velocity;


		this.rigidbody.velocity = vel;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

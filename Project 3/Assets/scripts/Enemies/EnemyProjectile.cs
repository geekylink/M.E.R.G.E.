using UnityEngine;
using System.Collections;

public class EnemyProjectile : MonoBehaviour {

	public float moveSpeed = 2f;
	public float timeLimit = 5f;
	float lifetime = 0f;
	public Vector3 velocity;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		lifetime += Time.deltaTime;
		Vector3 pos = transform.position;
		pos.x += velocity.x * moveSpeed * Time.deltaTime;
		pos.y += velocity.y * moveSpeed * Time.deltaTime;
		transform.position = pos;
		if (lifetime > timeLimit) {
			Destroy (this.gameObject);
		}
	}
	
	public void SetVelocity(Vector3 vel){
		velocity = vel;
	}
}

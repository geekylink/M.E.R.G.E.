using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	public float moveSpeed = 2f;
	public float waitTime = 0.25f;
	Sprite sprite;
	SpriteRenderer sr;
	Vector3 velocity;
	float passedTime = 0f;

	// Use this for initialization
	void Start () {
		//sr = this.GetComponent<SpriteRenderer> ();
		//sprite = this.gameObject.GetComponent<SpriteRenderer> ().sprite;
	}
	
	// Update is called once per frame
	void Update () {
		passedTime += Time.deltaTime;
		if(passedTime > waitTime){
			Vector3 pos = transform.position;
			pos.x += velocity.x * moveSpeed * Time.deltaTime;
			pos.y += velocity.y * moveSpeed * Time.deltaTime;
			transform.position = pos;
			if (Mathf.Abs (transform.position.x) > 10.5f || Mathf.Abs (transform.position.y) > 6.2f) {
				Destroy (this.gameObject);
			}
		}
	}

	public void SetVelocity(Vector3 vel){
		velocity = vel;
	}
}

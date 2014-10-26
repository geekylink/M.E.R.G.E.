using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	GameObject currTarget;
	public GameObject projectile;
	public int health = 2;
	public float moveSpeed = 2f;
	Vector3 velocity;
	public Vector3 targetPos;
	public float waitTime = 2f;
	float timeElapsed = 0f;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//Vector3 dir = currTarget.transform.position - transform.position;
		//float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		//transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		if (currTarget != null) {
			timeElapsed += Time.deltaTime;
			if (timeElapsed > waitTime) {
					FireProjectile ();
					timeElapsed = 0f;
			}
			currTarget = GameObject.FindGameObjectWithTag ("Target");
			targetPos = currTarget.transform.position;
			velocity = (currTarget.transform.position - transform.position).normalized * moveSpeed;
			transform.position = transform.position + velocity * Time.deltaTime;
		}
	}

	void OnTriggerEnter(Collider col){
		if (col.tag == "Bullet") {
			if(health == 1){
				Destroy (this.gameObject);
			} else{
				health -= 1;
			}
			Destroy (col.gameObject);
		}
	}

	void FireProjectile(){
		GameObject newProj = (GameObject)Instantiate (projectile, transform.position, Quaternion.identity);
		newProj.GetComponent<Projectile> ().SetVelocity (currTarget.transform.position - transform.position);
	}
}

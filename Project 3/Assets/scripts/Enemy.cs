using UnityEngine;
using System.Collections;

public class Enemy : BaseShip {

	GameObject currTarget;
	public GameObject projectile;
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
        currTarget = GameObject.FindGameObjectWithTag("Target");

		if (currTarget != null) {
			timeElapsed += Time.deltaTime;
			if (timeElapsed > waitTime) {
					//FireProjectile ();
					timeElapsed = 0f;
			}
			targetPos = currTarget.transform.position;
			velocity = (currTarget.transform.position - transform.position).normalized * moveSpeed;
			transform.position = transform.position + velocity * Time.deltaTime;

            var dir = targetPos - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 180;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

	}

	void OnCollisionEnter2D(Collision2D col){
		print ("col: " + col.gameObject.name);
		if (col.gameObject.tag == "Bullet") {
			Destroy (col.gameObject);
			TakeDamage(1);
		}

        if(col.gameObject.tag == "Target")
        {
			GameObject playerGO = GameObject.Find("Player");
			Player player = playerGO.GetComponent("Player") as Player;
			player.TakeDamage(1);

			Die ();
        }
        print("col");
	}

	void FireProjectile(){
		GameObject newProj = (GameObject)Instantiate (projectile, transform.position, Quaternion.identity);
		newProj.GetComponent<Projectile> ().SetVelocity (currTarget.transform.position - transform.position);
	}
}

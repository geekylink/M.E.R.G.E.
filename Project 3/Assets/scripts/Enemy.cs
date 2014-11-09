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
		currTarget = getRandomPlayer ();
	}
	
	// Update is called once per frame
	void Update () {
		if (currTarget != null) {
            targetPos = currTarget.transform.position;
            var dir = targetPos - transform.position;

            this.rigidbody2D.velocity = dir.normalized * moveSpeed;

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
	}


}

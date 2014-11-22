using UnityEngine;
using System.Collections;

public class Enemy : EnemyBaseShip {
	public GameObject sphere;
	//public GameObject currTarget;
	public GameObject projectile;
	public float moveSpeed = 2f;
	//public Vector3 velocity;
	public Vector3 targetPos;
	public float waitTime = 2f;
	// Use this for initialization
	void Start () {
        sphere.renderer.material.color = Color.red;
		//currTarget = getRandomPlayer ();
	}

	void Awake(){
		sphere.renderer.material.color = Color.red;
	}
	
	// Update is called once per frame
	void Update () {
		if (currTarget != null) {
            targetPos = currTarget.transform.position;
            var dir = targetPos - transform.position;
			if(squadId != 0){
				this.rigidbody2D.velocity = dir.normalized * moveSpeed + SquadManager.S.Boids(this.gameObject.GetComponent<EnemyBaseShip>(), squadId);
			}
			else{
				this.rigidbody2D.velocity = dir.normalized * moveSpeed;
			}

            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 180;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
		else{
			if(squadId == 0){
				currTarget = getRandomPlayer();
			}
		}

	}

	void OnCollisionEnter2D(Collision2D col){


        if(col.gameObject.tag == "Player")
        {
			GameObject playerGO = col.gameObject;
			Player player = playerGO.GetComponent("Player") as Player;
			if(player){
				player.TakeDamage(1);
				Die ();
			}
        }
	}


}

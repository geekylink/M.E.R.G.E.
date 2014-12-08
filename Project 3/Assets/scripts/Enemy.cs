using UnityEngine;
using System.Collections;

public class Enemy : EnemyBaseShip {
	public GameObject sphere;
	public GameObject projectile;

	// Use this for initialization
	public override void Start () {
        sphere.renderer.material.color = Color.red;
	}

	void Awake(){
		sphere.renderer.material.color = Color.red;
		GameManager.S.enemyList.Add (this.gameObject);
	}


	void OnCollisionEnter2D(Collision2D col){


        if(col.gameObject.tag == "Player")
        {
			GameObject playerGO = col.gameObject;
			Player player = playerGO.GetComponent("Player") as Player;
			if(player){
				if(!player.isInvulnerable){
					
					player.TakeDamage(1);
					Die ();
				}
			}
        }
		if(col.gameObject.tag == "Satellite"){
			BaseSatellite sat = col.collider.GetComponent<BaseSatellite>();
			sat.TakeDamage(1);
			if (explosion != null)
			{
				Instantiate(explosion, this.transform.position, Quaternion.identity);
			}
			
			Destroy(this.gameObject);
			
			return;
			
		}
	}


}

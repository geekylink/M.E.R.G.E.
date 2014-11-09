﻿using UnityEngine;
using System.Collections;

public class SpawningEnemy : BaseShip
{
	public GameObject sphere;
    public GameObject projectile;
    public float moveSpeed = 2f;
    public float fireRate = 2f;
    public float hoverDistance = 6;
    public float projectileSpeed = 4;
    private float fireTimer = 0f;
    private GameObject currTarget;

	// Use this for initialization
	void Start () {
		sphere.renderer.material.color = Color.red;
	}
	
	// Update is called once per frame
	void Update () {
        currTarget = GameObject.FindGameObjectWithTag("Target");

        if (currTarget != null)
        {
            Vector3 targetPos = currTarget.transform.position;
            var dist = targetPos - transform.position;
            if (dist.magnitude > hoverDistance)
                this.rigidbody2D.velocity = dist.normalized * moveSpeed;
            else
                this.rigidbody2D.velocity = Vector3.zero;

            var angle = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + 90f, Vector3.forward);

            if (fireTimer > fireRate)
            {
                fireTimer = 0;
                FireShip(transform.rotation);
            }
            else
            {
                fireTimer += Time.deltaTime;
            }
        }
	}

    void FireShip(Quaternion angle)
    {
        GameObject bulletGO = Instantiate(projectile, this.transform.position + Quaternion.Euler(-angle.eulerAngles.z +90,0,0) * (new Vector3( -1, 0, 0)), this.transform.rotation) as GameObject;
        
    }

	void OnCollisionEnter2D(Collision2D col){		
		if (col.gameObject.tag == "Bullet") {
			Bullet b = col.gameObject.GetComponent("Bullet") as Bullet;
			
			Destroy (col.gameObject);
			TakeDamage(b.damageDealt);
		}
		
		if(col.gameObject.tag == "Player")
		{
			GameObject playerGO = col.gameObject;
			Player player = playerGO.GetComponent("Player") as Player;
			player.TakeDamage(1);
			
			Die ();
		}
	}
}

using UnityEngine;
using System.Collections;

public class ShootingEnemy : EnemyBaseShip
{
	public GameObject sphere;
    public GameObject projectile;
    public float fireRate = 2f;
    public float hoverDistance = 0;
    public float projectileSpeed = 4;
    private float fireTimer = 2f;
	public Vector2 velocity;
	// Use this for initialization
	public override void Start () {
		sphere.renderer.material.color = Color.red;
		GameManager.S.enemyList.Add (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        //currTarget = GameObject.FindGameObjectWithTag("Target");

        if (currTarget != null)
        {
            if (fireTimer < 0)
            {
                fireTimer = fireRate + Random.Range(-1f, 1f);
                FireProjectile(transform.rotation);
            }
            else
            {
                fireTimer -= Time.deltaTime;
            }
        }
	}

    void FireProjectile(Quaternion angle)
    {
        GameObject bulletGO = Instantiate(projectile, this.transform.position, this.transform.rotation) as GameObject;
        Bullet b = bulletGO.GetComponent("Bullet") as Bullet;

		Vector2 dir = currTarget.transform.position - transform.position;
		dir = dir.normalized * projectileSpeed;

        b.setDefaults(dir);
    }

    void OnCollisionEnter2D(Collision2D col)
    {


        if (col.gameObject.tag == "Player")
        {
            GameObject playerGO = col.gameObject;
            Player player = playerGO.GetComponent("Player") as Player;
            if (player)
            {
                player.TakeDamage(1);
                Die();
            }
        }
    }
}

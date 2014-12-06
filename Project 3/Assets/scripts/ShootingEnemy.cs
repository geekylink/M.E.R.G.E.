using UnityEngine;
using System.Collections;

public class ShootingEnemy : EnemyBaseShip
{
	public GameObject sphere;
    public GameObject projectile;
    public float moveSpeed = 2f;
    public float fireRate = 2f;
    public float hoverDistance = 0;
    public float projectileSpeed = 4;
    private float fireTimer = 0f;
	public Vector2 velocity;
	// Use this for initialization
	public override void Start () {
		sphere.renderer.material.color = Color.red;
	}
	
	// Update is called once per frame
	void Update () {
        //currTarget = GameObject.FindGameObjectWithTag("Target");

        if (currTarget != null)
        {
            Vector3 targetPos = currTarget.transform.position;
            var dist = targetPos - transform.position;
            if (dist.magnitude > hoverDistance){
				if(squadId != 0){
					rigidbody2D.velocity = rigidbody2D.velocity + (Vector2)(SquadManager.S.Boids(this.gameObject.GetComponent<EnemyBaseShip>(), squadId) * 
						Time.deltaTime);
					if(rigidbody2D.velocity.magnitude < SquadManager.S.lowLimit){
						rigidbody2D.velocity = rigidbody2D.velocity.normalized * SquadManager.S.lowLimit;
					} else if (rigidbody2D.velocity.magnitude > SquadManager.S.highLimit){
						rigidbody2D.velocity = rigidbody2D.velocity.normalized * SquadManager.S.highLimit;
					}
				}
				else{
					rigidbody2D.velocity = dist.normalized * moveSpeed;
				}
				velocity = rigidbody2D.velocity;
			}
            else
				rigidbody2D.velocity = Vector3.zero;
            var angle = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + 90f, Vector3.forward);

            if (fireTimer > fireRate)
            {
                fireTimer = 0;
                FireProjectile(transform.rotation);
            }
            else
            {
                fireTimer += Time.deltaTime;
            }
        }
		
		else{
			currTarget = getRandomPlayer();
		}
	}

    void FireProjectile(Quaternion angle)
    {
        GameObject bulletGO = Instantiate(projectile, this.transform.position, this.transform.rotation) as GameObject;
        Bullet b = bulletGO.GetComponent("Bullet") as Bullet;
        b.setDefaults(-angle.eulerAngles.z + 90, projectileSpeed);
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

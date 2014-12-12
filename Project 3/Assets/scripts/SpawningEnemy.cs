using UnityEngine;
using System.Collections;

public class SpawningEnemy : EnemyBaseShip
{
	public GameObject sphere;
    public GameObject projectile;
    //public float moveSpeed = 2f;
    public float fireRate = 2f;
    public float hoverDistance = 6;
    public float projectileSpeed = 4;
    private float fireTimer = 0f;

	public float distanceToSpawn;

	// Use this for initialization
	public override void Start () {
		sphere.renderer.material.color = Color.red;
		spawner = true;
		health = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
        if (currTarget != null)
        {
			if(Vector3.Distance(transform.position, Camera.main.transform.position) < distanceToSpawn){
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
	}

    void FireShip(Quaternion angle)
    {
        GameObject enemyGO = Instantiate(Spawner.S.enemiesToSpawn[Random.Range (0, 2)], this.transform.position + Quaternion.Euler(-angle.eulerAngles.z +90,0,0) * (new Vector3( -1, 0, 0)), this.transform.rotation) as GameObject;
		EnemyBaseShip enemy = enemyGO.GetComponent<EnemyBaseShip> ();
		enemy.squadId = squadId;
		SquadManager.S.squads [squadId - 1].squadMembers.Add (enemy);
		GameManager.S.enemyList.Add (enemyGO);
		enemy.currTarget = currTarget;
		enemy.boidInit = true;
		enemy.StartRoutines();

			/*GameObject bulletGO = Instantiate(projectile, this.transform.position, this.transform.rotation) as GameObject;
			Bullet b = bulletGO.GetComponent("Bullet") as Bullet;
			
			Vector2 dir = currTarget.transform.position - transform.position;
			dir = dir.normalized * projectileSpeed;
			
			b.setDefaults(dir);*/
    }

	void OnCollisionEnter2D(Collision2D col){
		
		
		if(col.gameObject.tag == "Player")
		{
			GameObject playerGO = col.gameObject;
			Player player = playerGO.GetComponent("Player") as Player;
			if(player){
				if(!player.isInvulnerable){
					player.TakeDamage(1);
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

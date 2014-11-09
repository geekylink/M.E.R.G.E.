using UnityEngine;
using System.Collections;

public class ShootingEnemy : BaseShip
{

    public GameObject projectile;
    public float moveSpeed = 2f;
    public float fireRate = 2f;
    public float hoverDistance = 6;
    public float projectileSpeed = 4;
    private float fireTimer = 0f;
    private GameObject currTarget;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        currTarget = GameObject.FindGameObjectWithTag("Target");

        if (currTarget != null)
        {
            Vector3 targetPos = currTarget.transform.position;
            var dist = targetPos - transform.position;
            print(dist.magnitude);
            if (dist.magnitude > hoverDistance)
                this.rigidbody2D.velocity = dist.normalized * moveSpeed;
            else
                this.rigidbody2D.velocity = Vector3.zero;

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
	}

    void FireProjectile(Quaternion angle)
    {
        GameObject bulletGO = Instantiate(projectile, this.transform.position, this.transform.rotation) as GameObject;
        Bullet b = bulletGO.GetComponent("Bullet") as Bullet;
        b.setDefaults(-angle.eulerAngles.z + 90, projectileSpeed);
    }
}

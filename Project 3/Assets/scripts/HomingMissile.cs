using UnityEngine;
using System.Collections;

public class HomingMissile : MonoBehaviour {

	public GameObject currTarget;
	public GameObject explosion;
	public float startVel;
	public float maxVel;
	public int damageDealt = 2;
	bool init = true;
	float timeSinceLaunch = 1f;

	void Start(){
		currTarget = SquadManager.S.GetRandomOnScreenEnemy ();
	}

	void Update(){
		timeSinceLaunch += Time.deltaTime;
		if (currTarget != null) {
			Vector3 targetPos = currTarget.transform.position;
			var dir = targetPos - transform.position;
			if(init){
				this.rigidbody2D.velocity = dir.normalized * startVel;
				init = false;
			} 
			if(this.rigidbody2D.velocity.magnitude < maxVel){
				this.rigidbody2D.velocity = dir.normalized * (startVel + 
					(maxVel - startVel) * timeSinceLaunch * timeSinceLaunch);
			} else {
				this.rigidbody2D.velocity = dir.normalized * maxVel;
			}

			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 180;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		else{
			currTarget = SquadManager.S.GetRandomOnScreenEnemy();
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.tag == "Satellite"){
			BaseSatellite sat = col.collider.GetComponent<BaseSatellite>();
			sat.TakeDamage(damageDealt);
			if (explosion != null)
			{
				Instantiate(explosion, this.transform.position, Quaternion.identity);
			}
			Destroy(this.gameObject);
			
			return;
		}
		
		BaseShip bs = col.collider.GetComponent<BaseShip>();
		if(bs != null)
		{ 
			if(!bs.isInvulnerable){
				
				bs.TakeDamage(damageDealt);
				if (explosion != null)
				{
					Instantiate(explosion, this.transform.position, Quaternion.identity);
				}
				Destroy(this.gameObject);
			}
		}
	}
	
}

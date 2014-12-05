using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public int damageDealt;

	public float lifeTime = 5;
    public GameObject explosion;
    public Color color;

	public Player owner;

    float lifeCounter = 0;

	public float offscreenBuffer;

	// Use this for initialization
	void Start () {
	    
	}

    public void SetColor(Color color)
    {
        this.GetComponent<SpriteRenderer>().color = color;
        ParticleSystem sys = this.GetComponentInChildren<ParticleSystem>();
        sys.startColor = color;
    }

	public void setDefaults(float angle, float velocity) {
		Vector3 vel = Vector3.zero;
		vel.y = -Mathf.Sin (angle*Mathf.Deg2Rad);
		vel.x = Mathf.Cos (angle*Mathf.Deg2Rad);
		
		this.rigidbody2D.velocity = vel.normalized * velocity;
	}

	public void setDefaults(Vector2 velocity) {
		this.rigidbody2D.velocity = velocity;
	}
	
	// Update is called once per frame
	void Update () {

		lifeCounter += Time.deltaTime;
		if(lifeCounter > lifeTime){
			Destroy(this.gameObject);
		}
		CheckIfOffscreen();
	}

	/// <summary>
	/// Checks if offscreen. If it is, destroy it (with a bit of padding)
	/// Otherwise, players can shoot far offscreen, and randomly destroy turrets
	/// (same with enemies)
	/// We don't really want that happening, so this will fix that
	/// 
	/// </summary>
	void CheckIfOffscreen () {
		if(Camera.main == null) return;
		
		float z = transform.position.z-Camera.main.transform.position.z;
		
		float topPosY = Camera.main.ViewportToWorldPoint(new Vector3(0,1,z)).y;
		float bottomPosY = Camera.main.ViewportToWorldPoint(new Vector3(0,0,z)).y;
		float leftPosX = Camera.main.ViewportToWorldPoint(new Vector3(0,0,z)).x;
		float rightPosX = Camera.main.ViewportToWorldPoint(new Vector3(1,0,z)).x;

		Vector3 pos = transform.position;
		
		if (pos.y>topPosY + offscreenBuffer) {
			Destroy (this.gameObject);
		} 
		else if (pos.y < bottomPosY - offscreenBuffer) {
			Destroy (this.gameObject);
		}
		else if (pos.x>rightPosX + offscreenBuffer) {
			Destroy (this.gameObject);
		} 
		else if (pos.x<leftPosX - offscreenBuffer) {
			Destroy (this.gameObject);
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

			if (owner != null) {
				owner.score+= 3;
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

				if (owner != null) {
					owner.score+= 2;
				}

				Destroy(this.gameObject);
			}
        }

		/*else{
			bs = col.gameObject.GetComponentInChildren<BaseShip>();
			
			bs.TakeDamage(damageDealt);
			Destroy(this.gameObject);
		}*/
    }

}

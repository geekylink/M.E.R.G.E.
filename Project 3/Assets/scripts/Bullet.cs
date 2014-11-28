﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public int damageDealt;

	public float lifeTime = 5;
    public GameObject explosion;
    public Color color;

    float lifeCounter = 0;

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

		/*else{
			bs = col.gameObject.GetComponentInChildren<BaseShip>();
			
			bs.TakeDamage(damageDealt);
			Destroy(this.gameObject);
		}*/
    }

}

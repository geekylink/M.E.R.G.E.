using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public int damageDealt;

	public float lifeTime = 5;
	float lifeCounter = 0;

	// Use this for initialization
	void Start () {
	
	}

	public void setDefaults(float angle, float velocity) {
		Vector3 vel = Vector3.zero;
		vel.y = -Mathf.Sin (angle*Mathf.Deg2Rad)*velocity;
		vel.x = Mathf.Cos (angle*Mathf.Deg2Rad)*velocity;
		
		this.rigidbody2D.velocity = vel;


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
        BaseShip bs = col.gameObject.GetComponent<BaseShip>();
        if(bs != null)
        { 
            bs.TakeDamage(damageDealt);
            Destroy(this.gameObject);
        }
    }

}

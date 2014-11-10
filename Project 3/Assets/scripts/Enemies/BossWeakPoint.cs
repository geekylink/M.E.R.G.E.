using UnityEngine;
using System.Collections;

public class BossWeakPoint : MonoBehaviour {

	public int health = 50;
	public Sprite activeSprite;
	public Sprite altSprite;
	public SpriteRenderer sr;
	public bool activePoint;
	public bool shielded;
	bool flashing;
	bool currSprite = false;
	public float flashTime = 0f;


	// Use this for initialization
	void Start () {
		sr = this.gameObject.GetComponent<SpriteRenderer> ();
		if (activePoint) {
			sr.sprite = activeSprite;
		} else {
			sr.sprite = null;
		}
	}

	void Update(){
		if (flashing) {
			flashTime += Time.deltaTime;
			if (flashTime > 1f){
				flashTime = 0;
				flashing = false;
				sr.sprite = activeSprite;
			} else if (currSprite) {
				sr.sprite = altSprite;
				currSprite = false;
			} else {
				sr.sprite = activeSprite;
				currSprite = true;
			}
		}
	}

	void Fire(){}

	void OnTriggerEnter2D(Collider2D col){
		if (activePoint) {
			if (col.tag == "Bullet") {
				int damage = col.gameObject.GetComponent<Bullet>().damageDealt;
				if(shielded){
					if (damage > 1){
						health -= (damage - 1);
						flashing = true;
					}
				} else {
					health -= damage;
					flashing = true;
				}
				if (health < 0) {
					Destroy (this.gameObject);
					BossShip.S.NextStage ();
				} 
				Destroy (col.gameObject);
			}
		}
	}
}

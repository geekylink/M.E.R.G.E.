using UnityEngine;
using System.Collections;

public class SubCapturePoint : MonoBehaviour {

	public SpriteRenderer sr;
	public Sprite fullHealthSprite;
	public Sprite damagedSprite;
	public Sprite destroyedSprite;
	public int health;
	int baseHealth;

	// Use this for initialization
	void Start () {
		sr = this.GetComponent<SpriteRenderer> ();
		sr.sprite = fullHealthSprite;
		baseHealth = health;
	}
	
	public void Reset(){
		sr.sprite = fullHealthSprite;
		health = baseHealth;
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == "Enemy" || col.gameObject.tag == "Bullet-Enemy") {
			health -= 1;
			if (health <= 0){
				sr.sprite = destroyedSprite;
			} else if(health < baseHealth / 2){
				sr.sprite = damagedSprite;
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class BossWeakPoint : MonoBehaviour {

	public int health = 50;
	public Sprite activeSprite;
	public Sprite inactiveSprite;
	public SpriteRenderer sr;
	public bool activePoint;


	// Use this for initialization
	void Start () {
		sr = this.gameObject.GetComponent<SpriteRenderer> ();
		if (activePoint) {
			sr.sprite = activeSprite;
		} else {
			sr.sprite = inactiveSprite;
		}
	}

	void Fire(){}

	void OnTriggerEnter2D(Collider2D col){
		if (activePoint) {
			if (col.tag == "Bullet") {
				if (health == 1) {
					Destroy (this.gameObject);
					BossShip.S.NextStage ();
				} else {
					health -= 1;
					Destroy (col.gameObject);
				}
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class BossWeakPoint : MonoBehaviour {

	public float health = 50;
	public Sprite activeSprite;
	public Sprite altSprite;
	public SpriteRenderer sr;
	public bool activePoint;
	public bool shielded;
	public bool flashing;
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
		GameManager.S.enemyList.Add (this.gameObject);
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

	public void TakeDamage(float damage, Player pKilled = null){
		if (activePoint) {
			if(shielded){
				if (damage > 1){
					if(pKilled != null){
						pKilled.GiveXP(0.1f);
					}
					health -= (damage - 1);
					flashing = true;
				}
			} else {
				if(pKilled != null){
					pKilled.GiveXP(0.1f);
				}
				health -= damage;
				flashing = true;
			}
			if (health < 0) {
				if(pKilled != null){
					pKilled.GiveXP(3f);
					pKilled.KillSomething();
				}
				Destroy (this.gameObject);
				BossShip.S.NextStage ();
			} 
		}
	}

	void Fire(){}
}

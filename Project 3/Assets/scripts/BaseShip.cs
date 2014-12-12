using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseShip : MonoBehaviour {

    public float maxHealth;
	protected float health;
    public GameObject explosion;
    public static int score;
	public float maxRotSpeed;

    public GameObject drop;

	public bool isInvulnerable = false;
	public bool spawner = false;

	
	
	public GameObject shieldPrefab;
	GameObject shield;

	// Use this for initialization
	public virtual void Start () {
		health = maxHealth;
	}

	public void TakeDamage(float amount) {
		bool applyDamage = true;
		if(isInvulnerable) return;
		if(shield){
			RemoveShield();
			return;
		}
		if (spawner) {
			if(amount > 1){
				amount -= 1;
			}
			else{
				SFXManager.getManager().playSound("NoDamage");
				applyDamage = false;
			}
		}
		if (applyDamage) {
			health -= amount;
		}
		if (health <= 0) {
			health = 0;
			Die();
		}

	}

	public void Heal(int amount) {
		health += amount;

		if (health > maxHealth) {
			health = maxHealth;
		}
	}

	public virtual void Die() {
        if(explosion != null)
        {
            Instantiate(explosion, this.transform.position, Quaternion.identity);
        }
		
        if(this.drop != null)
        {
            Instantiate(drop, transform.position, Quaternion.identity);
        }
        
        Destroy (this.gameObject);

	}

	// Update is called once per frame
	void Update () {
	}

	public void RestrictToMap(){
		if(CameraMove.S.restrictToFurthestOrbit) return;
		Vector2 pos = transform.position;
		if(Mathf.Abs (pos.x) > GameManager.S.mapSize	){
			pos.x = GameManager.S.mapSize * Mathf.Abs (pos.x) / pos.x;

			Vector3 vel = transform.root.rigidbody2D.velocity;
			vel.x = 0;
			transform.root.rigidbody2D.velocity = vel;
		}
		if(Mathf.Abs (pos.y) > GameManager.S.mapSize){
			pos.y = GameManager.S.mapSize * Mathf.Abs (pos.y) / pos.y;

			Vector3 vel = transform.root.rigidbody2D.velocity;
			vel.y = 0;
			transform.root.rigidbody2D.velocity = vel;
		}
		transform.position = pos;
	}

	// Gets a random player
	public GameObject getRandomPlayer() {
		GameObject cam = GameObject.Find ("Main Camera");
		PlayerManager pm = cam.GetComponent ("PlayerManager") as PlayerManager;
		GameObject[] players = pm.getPlayers ();

		if(players == null || players.Length == 0){
			return null;
		}

		int randomNum = Random.Range (0, players.Length);


		if(players.Length != 0){
			
			int counter = 0;
			while(players[randomNum] == null && counter < 4){
				counter++;
				randomNum++;
				if(randomNum >= players.Length){
					randomNum = 0;
				}
			}
			return players [randomNum];
		}
		return null;
	}

	
	public void AddShield(){
		if(shield != null) return;
		GameObject newShield = Instantiate(shieldPrefab) as GameObject;
		shield = newShield;

		//shield.transform.position = transform.position;
		shield.transform.parent = this.transform;
		shield.transform.localPosition = Vector3.zero;
		shield.transform.localScale = Vector2.one * 0.25f;
	}
	
	public void RemoveShield(){
		if(shield == null) return;
		Destroy(shield);
	}
}

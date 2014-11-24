using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseShip : MonoBehaviour {

	public int maxHealth;
	protected int health;
    public GameObject explosion;
    public static int score;
	public float maxRotSpeed;

    public GameObject drop;

	public bool isInvulnerable = false;

	// Use this for initialization
	public virtual void Start () {
		health = maxHealth;
	}

	public void TakeDamage(int amount) {
		if(isInvulnerable) return;

		health -= amount;
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
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseShip : MonoBehaviour {

	public int maxHealth;
	protected int health;
    public GameObject explosion;
    public static int score;
	public float maxRotSpeed;

	
	public List<GameObject> enginesTurnRight;
	public List<GameObject> enginesTurnLeft;
	public List<GameObject> enginesStraight;

	// Use this for initialization
	void Start () {
		health = maxHealth;
	}

	public void TakeDamage(int amount) {
		health -= amount;

		if (health <= 0) {
			health = 0;
			Die();
		}
	}

	public void Die() {
        if(explosion != null)
        {
            GameObject exp = (GameObject)Instantiate(explosion, this.transform.position, Quaternion.identity);
        }
        UnityEngine.UI.Text txt = GameObject.Find("scoreText").GetComponent < UnityEngine.UI.Text>();
        score++;
        txt.text = "Score: " + score;
		Destroy (this.gameObject);
	}

	// Update is called once per frame
	void Update () {
	    
	}

	// Gets a random player
	public GameObject getRandomPlayer() {
		GameObject cam = GameObject.Find ("Main Camera");
		PlayerManager pm = cam.GetComponent ("PlayerManager") as PlayerManager;
		GameObject[] players = pm.getPlayers ();
		int randomNum = Random.Range (0, players.Length);

		return players [randomNum];	
	}
}

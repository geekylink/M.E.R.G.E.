using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBaseShip : MonoBehaviour {
	
	public int maxHealth;
	protected int health;
	public GameObject explosion;
	public static int score;
	public float maxRotSpeed;
	public int squadId;
	public GameObject currTarget;
	public Vector3 velocity;
	
	public List<GameObject> enginesTurnRight;
	public List<GameObject> enginesTurnLeft;
	public List<GameObject> enginesStraight;
	
	public GameObject drop;
	
	// Use this for initialization
	void Start () {
		health = maxHealth;
		squadId = 0;
	}
	
	public void TakeDamage(int amount) {
		health -= amount;
		if (health <= 0) {
			health = 0;
			Die();
		}
	}
	
	public virtual void Die() {
		if(explosion != null)
		{
			GameObject exp = (GameObject)Instantiate(explosion, this.transform.position, Quaternion.identity);
		}
		UnityEngine.UI.Text txt = GameObject.Find("scoreText").GetComponent < UnityEngine.UI.Text>();
		score++;
		txt.text = "Score: " + score;
		
		if(this.drop != null)
		{
			Instantiate(drop, transform.position, Quaternion.identity);
		}
		SquadManager.S.RemoveEnemy (this, squadId);
		Destroy (this.gameObject);
		
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void RestrictToMap(){
		Vector2 pos = transform.position;
		if(Mathf.Abs (pos.x) > 120	){
			pos.x = 120 * Mathf.Abs (pos.x) / pos.x;
			
			Vector3 vel = transform.root.rigidbody2D.velocity;
			vel.x = 0;
			transform.root.rigidbody2D.velocity = vel;
		}
		if(Mathf.Abs (pos.y) > 120){
			pos.y = 120 * Mathf.Abs (pos.y) / pos.y;
			
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
		int randomNum = Random.Range (0, players.Length);
		
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
}

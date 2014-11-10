using UnityEngine;
using System.Collections;

public class BossShip : MonoBehaviour {
	
	public GameObject explosion;

	public GameObject sphere;
	public BossWeakPoint[] weakPoints;
	public float timeBetweenVolleys = 8f;
	public float timeBetweenShots = 0.14f;
	public GameObject projectile;
	bool firing;
	bool first = false, second = false, final = false;
	float waitTime = 0f;
	float fireWaitTime = 0f;
	public float angle = 180f;
	public float bulletVelocity = 2f;
	public int degreesApart;


	public static BossShip S;

	// Use this for initialization
	void Awake () {
		if (S == null) {
			S = this;
		} else if (S != this) {
			Destroy(this.gameObject);
		}
		sphere.renderer.material.color = Color.red;

	}

	public void NextStage(){
		if (!first) {
			weakPoints [1].activePoint = true;
			weakPoints [1].shielded = true;
			weakPoints [1].sr.sprite = weakPoints [1].activeSprite;
			weakPoints [2].activePoint = true;
			weakPoints [2].shielded = true;
			weakPoints [2].sr.sprite = weakPoints [2].activeSprite;
			first = true;
		} else if (first && !second) {
			second = true;
		} else if (first && second && !final) {
			weakPoints [3].activePoint = true;
			weakPoints [3].sr.sprite = weakPoints [2].activeSprite;
			final = true;
		} else {
			Die ();
		}
	}

	void Die(){
		if(explosion != null)
		{
			GameObject exp = (GameObject)Instantiate(explosion, this.transform.position, Quaternion.identity);
		}
		UnityEngine.UI.Text txt = GameObject.Find("scoreText").GetComponent < UnityEngine.UI.Text>();
		BaseShip.score = BaseShip.score + 100;
		txt.text = "Score: " + BaseShip.score;
		Destroy (this.gameObject);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		waitTime += Time.deltaTime;
		if (waitTime > timeBetweenVolleys) {
			waitTime = 0;
			firing = true;
		}
		if (firing) {
			fireWaitTime += Time.deltaTime;
			if(fireWaitTime > timeBetweenShots){
				fireWaitTime = 0f;
				if(angle == 360f){
					angle = 0f;
					firing = false;
				}
				else {
					Fire ();
				}
			}
		}
	}

	void Fire(){
		GameObject proj1 = (GameObject)Instantiate (projectile, transform.position, Quaternion.identity);
		GameObject proj2 = (GameObject)Instantiate (projectile, transform.position, Quaternion.identity);
		proj1.GetComponent<Bullet> ().setDefaults (angle, bulletVelocity);
		proj2.GetComponent<Bullet> ().setDefaults (angle - 180f, bulletVelocity);
		angle += degreesApart;
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == "Player") {
			GameObject playerGO = col.gameObject;
			Player player = playerGO.GetComponent("Player") as Player;
			if(player){
				player.TakeDamage(1);
			}
		}

		if(col.gameObject.tag == "Bullet"){
			Destroy(col.gameObject);
		}
	}

	Vector3 Deg2Vec (float degree)
	{
		float rad = degree * Mathf.Deg2Rad;
		Vector3 vec = new Vector3 (Mathf.Cos (rad), Mathf.Sin (rad), 0);
		return vec;
	}
	
	float Vec2Deg (Vector3 vec)
	{
		Vector3 norm = vec.normalized;
		float result;
		if (norm.x >= 0 && norm.y >= 0) {
			result = Mathf.Asin (norm.y);
		} else if (norm.x <= 0 && norm.y >= 0) {
			result = Mathf.Acos (norm.x);
		} else if (norm.x <= 0 && norm.y >= 0) {
			result = Mathf.Acos (norm.x);
			result += 2f * Mathf.Abs(Mathf.PI - result);
		} else {
			result = Mathf.Acos (norm.x);
			result = Mathf.PI * 2f - result;
		}
		
		if (result > 2f * Mathf.PI) {
			return Mathf.Rad2Deg * result - 360;
		} else if (result < 0) {
			return Mathf.Rad2Deg * result + 360;
		} else {
			return Mathf.Rad2Deg * result;
		}
	}
}

//Vector3 dir = Link.S.transform.position - transform.position;
//float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
//transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);


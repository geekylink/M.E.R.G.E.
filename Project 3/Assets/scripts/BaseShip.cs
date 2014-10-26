using UnityEngine;
using System.Collections;

public class BaseShip : MonoBehaviour {

	public int maxHealth;
	protected int health;

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
		Destroy (this.gameObject);
	}

	// Update is called once per frame
	void Update () {
	
	}
}

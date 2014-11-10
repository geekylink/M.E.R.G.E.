using UnityEngine;
using System.Collections;

public class HealthDrop : MonoBehaviour {

    public int healthRecovered = 1;

	void Start () {
	
	}
	
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            GameObject playerGO = col.gameObject;
            Player player = playerGO.GetComponent("Player") as Player;
            player.TakeDamage(-healthRecovered);

            Destroy(this.gameObject);
        }
    }
}

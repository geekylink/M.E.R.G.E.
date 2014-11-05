using UnityEngine;
using System.Collections;

public class MergedShip : MonoBehaviour {

	public bool[] shipsInPosition;

	float bounciness = 0;
	public float Bounciness{
		get{return bounciness;}
		set{bounciness = value;}
	}

	int numberOfMergedShips = 0;
	public int NumberOfMergedShips{
		get{return numberOfMergedShips;}
		set{numberOfMergedShips = value;}
	}

	// Use this for initialization
	void Start () {
		shipsInPosition = new bool[4];
	}
	
	// Update is called once per frame
	void Update () {
		ClampObjectIntoView();
	}

	void ClampObjectIntoView () {
		if(rigidbody2D == null) return;
		
		float z = transform.position.z-Camera.main.transform.position.z;
		
		float topPosY = Camera.main.ViewportToWorldPoint(new Vector3(0,1,z)).y;
		float bottomPosY = Camera.main.ViewportToWorldPoint(new Vector3(0,0,z)).y;
		float leftPosX = Camera.main.ViewportToWorldPoint(new Vector3(0,0,z)).x;
		float rightPosX = Camera.main.ViewportToWorldPoint(new Vector3(1,0,z)).x;
		
		
		Vector2 vel = rigidbody2D.velocity;
		Vector3 pos = transform.position;
		
		if (transform.position.y>topPosY) {
			vel.y = -vel.y * bounciness;
			pos.y = topPosY;
		} 
		else if (transform.position.y < bottomPosY) {
			vel.y = -vel.y * bounciness;
			pos.y = bottomPosY;
		}
		else if (transform.position.x>rightPosX) {
			vel.x = -vel.x * bounciness;
			pos.x = rightPosX;
		} 
		else if (transform.position.x<leftPosX) {
			vel.x = -vel.x * bounciness;
			pos.x = leftPosX;
		}
		rigidbody2D.velocity = vel;
		transform.position = pos;
	}
}

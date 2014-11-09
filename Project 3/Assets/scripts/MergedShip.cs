using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MergedShip : MonoBehaviour {

	//has to know where ships are in order to
	//add new ships
	public bool[] shipsInPosition = new bool[4];
	public int[] pNumAtPosition = new int[4];

	//time it takes for ships to join - edited in mergeManager.cs
	float rotMergeTime = 0;
	public float RotMergeTime{
		get{return rotMergeTime;}
		set{rotMergeTime = value;}
	}

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
	}
	
	// Update is called once per frame
	void Update () {
		RestrictToMap();
		ClampObjectIntoView();
	}

	public void RestrictToMap(){
		Vector2 pos = transform.position;
		if(Mathf.Abs (pos.x) > 120	){
			pos.x = 120 * Mathf.Abs (pos.x) / pos.x;
			
			Vector3 vel = this.rigidbody2D.velocity;
			vel.x = 0;
			this.rigidbody2D.velocity = vel;
		}
		if(Mathf.Abs (pos.y) > 120){
			pos.y = 120 * Mathf.Abs (pos.y) / pos.y;
			
			Vector3 vel = this.rigidbody2D.velocity;
			vel.y = 0;
			this.rigidbody2D.velocity = vel;
		}
		transform.position = pos;
	}


	//clamp object into view - same as in Player.cs
	void ClampObjectIntoView () {
		if(rigidbody2D == null) return;
		if(Camera.main == null) return;
		
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

	//coroutine which figures out where player should go in larger merge ship,
	//then rotates and moves the player to that position
	//
	//This is the function which should be edited to change up how players should
	//form the larger ship
	IEnumerator MoveAndRotatePlayerShip(Player player, int numberInMerge){
		player.IsMerging = true;
		rigidbody2D.Sleep();
		Vector2 center = transform.position;
		Vector2 right = transform.right;
		Vector2 up = transform.up;

		Vector2 oldRight = player.transform.right;
		Vector2 oldPos = player.transform.position;
		
		Vector2 newPos = Vector2.zero;
		Vector2 newRight = Vector2.zero;

		GameObject leftEngine = player.leftEnginePiece;
		GameObject rightEngine = player.rightEnginePiece;

		Vector3 leftRot = leftEngine.transform.rotation.eulerAngles;
		Vector3 rightRot = rightEngine.transform.rotation.eulerAngles;

		leftRot.x -= 45;
		rightRot.x += 45;

		leftEngine.transform.rotation = Quaternion.Euler(leftRot);
		rightEngine.transform.rotation = Quaternion.Euler(rightRot);

		if(numberInMerge == 0)	{
			newPos = center + right * -0.66f;
			newRight = right;
		}
		else if(numberInMerge == 1)	{
			newPos = center + up * -0.66f;
			newRight = up;
		}
		else if(numberInMerge == 2)	{
			newPos = center + right * 0.66f;
			newRight = -right;
		}
		else if(numberInMerge == 3)	{
			newPos = center + up * 0.66f;
			newRight = -up;
		}
		
		float t = 0;
		while(t < 1){
			t += Time.deltaTime * Time.timeScale / rotMergeTime;
			
			player.transform.right = Vector2.Lerp(oldRight, newRight, t);
			player.transform.position = Vector2.Lerp (oldPos, newPos, t);
			
			yield return 0;
		}
		
		player.IsMerging = false;
		rigidbody2D.WakeUp();
	}

	//insert a player into the merged ship
	public void AddShip(Player playerScript, int pNum){
		playerScript.IsCurrentlyMerged = true;

		int index = System.Array.IndexOf(shipsInPosition, false);
		shipsInPosition[index] = true;
		pNumAtPosition[index] = pNum;

		StartCoroutine(MoveAndRotatePlayerShip(playerScript, index));
		numberOfMergedShips++;
	}

	//remove a player from the merged ship
	public void RemoveShip(Player playerScript, int pNum){
		int index = System.Array.IndexOf(pNumAtPosition, pNum);

		shipsInPosition[index] = false;
		pNumAtPosition[index] = -1;
		
		numberOfMergedShips--;

		GameObject leftEngine = player.leftEnginePiece;
		GameObject rightEngine = player.rightEnginePiece;
		
		Vector3 leftRot = leftEngine.transform.rotation.eulerAngles;
		Vector3 rightRot = rightEngine.transform.rotation.eulerAngles;
		
		leftRot.x += 45;
		rightRot.x -= 45;
		
		leftEngine.transform.rotation = Quaternion.Euler(leftRot);
		rightEngine.transform.rotation = Quaternion.Euler(rightRot);
	}
}

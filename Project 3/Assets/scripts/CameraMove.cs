using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	public static CameraMove S;

	public bool restrictToFurthestOrbit;


	public float camMoveSpeed;
	public float camSizeSpeed;

	public float minCameraSize;
	public float maxCameraSize;

	public Vector3 cameraCenter;
	Vector3 previousCamCenter;

	public Vector2 camBuffer;
	
	public float minX, maxX, minY, maxY;

	float camSize;
	float previousCamSize;

	bool allPlayersDead = false;

	void Start(){
		if(S == null)
		{
			//If I am the first instance, make me the Singleton
			S = this;
			//DontDestroyOnLoad(this);
		}
		else
		{
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if(this != S)
				Destroy(this.gameObject);
		}
	}

	void Update() {
		CalculateBounds();
		CalculateCameraPosAndSize();
	}

	void CalculateBounds() { 

		minX = Mathf.Infinity; maxX = -Mathf.Infinity; minY = Mathf.Infinity; maxY = -Mathf.Infinity;
		
		GameObject[] players = PlayerManager.S.players;
		int numPlayers = 0;
		foreach (GameObject player in players){
			if(!player) continue;
			numPlayers++;
			Vector3 tempPlayer = player.transform.position;
			
			//X Bounds
			if (tempPlayer.x < minX)
				minX = tempPlayer.x;
			
			if (tempPlayer.x > maxX)
				maxX = tempPlayer.x;
			
			//Y Bounds
			if (tempPlayer.y < minY)
				minY = tempPlayer.y;
			
			if (tempPlayer.y > maxY)
				maxY = tempPlayer.y;
		}
		if(numPlayers == 0){
			allPlayersDead = true;
		}
		else{
			allPlayersDead = false;
		}
		//print (numPlayers);
	}

	/// <summary>
	/// Basically, calculate where the camera should be,
	/// and how large based on positions of the players
	/// 
	/// Restricts to furthest radius found in GameManager
	/// </summary>
	void CalculateCameraPosAndSize() { 
		GameObject[] players = PlayerManager.S.players;

		foreach(GameObject player in players){
			if(!player) continue;
			cameraCenter += player.transform.position;
		}

		if(maxX == -Mathf.Infinity){
			maxX = 0;
		}
		if(minX == Mathf.Infinity){
			minX = 0;
		}
		if(maxY == -Mathf.Infinity){
			maxY = 0;
		}
		if(minY == Mathf.Infinity){
			minY = 0;
		}

		cameraCenter.x = (maxX + minX) * 0.5f;
		cameraCenter.y = (maxY + minY) * 0.5f;
		cameraCenter.z = -10;

		if(allPlayersDead){
			cameraCenter = previousCamCenter;
		}

		transform.position = Vector3.Lerp(transform.position, cameraCenter, camMoveSpeed * Time.deltaTime);

		
		//Size
		float sizeX = maxX - minX + camBuffer.x;
		float sizeY = maxY - minY + camBuffer.y;
		
		camSize = (sizeX > sizeY ? sizeX : sizeY);

		camSize = camSize * 0.5f;
		camSize = (camSize < minCameraSize ? minCameraSize : camSize);
		camSize = (camSize > maxCameraSize ? maxCameraSize : camSize);

		camSize = Mathf.Lerp(previousCamSize, camSize, camSizeSpeed * Time.deltaTime);

		camera.orthographicSize = camSize;

		Vector3 pos = transform.position;
		pos.z = -10;
		float width = camSize * camera.aspect;

		if(!restrictToFurthestOrbit){
			//Just restrict to entire map
			if(Mathf.Abs (pos.x) > (GameManager.S.mapSize - width)	){
				pos.x = (GameManager.S.mapSize - width) * Mathf.Abs (pos.x) / pos.x;
			}
			if(Mathf.Abs (pos.y) > (GameManager.S.mapSize - camSize)){
				pos.y = (GameManager.S.mapSize - camSize) * Mathf.Abs (pos.y) / pos.y;
			}
		}
		else{
			//restrict to radius of furthestAllowedRadius
			float distFromCent = Mathf.Sqrt (pos.x * pos.x + pos.y * pos.y);
			if(distFromCent > GameManager.S.furthestAllowedRadius){
				pos = pos.normalized * GameManager.S.furthestAllowedRadius;
				pos.z = -10;
			}
		}
		transform.position = pos;
		cameraCenter = pos;

		previousCamCenter = cameraCenter;
		previousCamSize = camSize;
		
	} 

	public void MoveCamCenter(Vector2 newCamCenter){
		cameraCenter = (Vector3)newCamCenter;
		cameraCenter.z = -10;
		previousCamCenter = cameraCenter;
		transform.position = cameraCenter;
	}
}

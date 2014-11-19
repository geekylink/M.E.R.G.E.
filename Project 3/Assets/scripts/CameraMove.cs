using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

	public float camSpeed;
	public float minCameraSize;
	public float maxCameraSize;

	Vector3 cameraCenter;

	public Vector2 camBuffer;
	
	public float minX, maxX, minY, maxY;

	float camSize;

	void Start(){
		cameraCenter = Vector3.zero;
		cameraCenter.z = -10;
	}

	void Update() {
		CalculateBounds();
		CalculateCameraPosAndSize();
	}

	void CalculateBounds() { 

		minX = Mathf.Infinity; maxX = -Mathf.Infinity; minY = Mathf.Infinity; maxY = -Mathf.Infinity;
		
		GameObject[] players = PlayerManager.S.players;
		
		foreach (GameObject player in players){
			if(!player) continue;
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
		
	}

	void CalculateCameraPosAndSize() { 
		GameObject[] players = PlayerManager.S.players;

		foreach(GameObject player in players){
			if(!player) continue;
			cameraCenter += player.transform.position;
		}
		cameraCenter.x = (maxX + minX) * 0.5f;
		cameraCenter.y = (maxY + minY) * 0.5f;

		transform.position = Vector3.Lerp(transform.position, cameraCenter, camSpeed * Time.deltaTime);
		
		//finalLookAt = Vector3.Lerp (finalLookAt, finalCameraCenter, camSpeed * Time.deltaTime);
		
		//transform.LookAt(finalLookAt);
		
		//Size
		float sizeX = maxX - minX + camBuffer.x;
		float sizeY = maxY - minY + camBuffer.y;
		
		camSize = (sizeX > sizeY ? sizeX : sizeY);
		
		camera.orthographicSize = camSize * 0.5f;
		camera.orthographicSize = (camera.orthographicSize < minCameraSize ? minCameraSize : camera.orthographicSize);
		camera.orthographicSize = (camera.orthographicSize > maxCameraSize ? maxCameraSize : camera.orthographicSize);
		
} 
}

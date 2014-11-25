using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	public static GameManager S;

	public float mapSize;

	public Sprite borderSprite;

	public List<Color> playerColors; 
	public List<CapturePoint> capturePoints = new List<CapturePoint>();

	// Use this for initialization
	void Start () {
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
		DontDestroyOnLoad(this.gameObject);

		playerColors = new List<Color>();

	}

	public void AcquireCaptures(){
		GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
		foreach(GameObject planet in planets){
			capturePoints.Add (planet.GetComponent<CapturePoint>());
		}
	}

	public void CreateMapBoundaries(){
		CreateWall(0, mapSize, mapSize * 2.5f, 1);
		CreateWall(0, -mapSize, mapSize * 2.5f, 1);
		CreateWall(mapSize, 0, 1, mapSize * 2.5f);
		CreateWall(-mapSize, 0, 1, mapSize * 2.5f);
	}

	void CreateWall(float x, float y, float xScale, float yScale){
		GameObject edge = new GameObject();
		Vector3 tempPos = new Vector2(x, y);
		edge.transform.position = tempPos;
		Vector3 tempScale = edge.transform.localScale;
		tempScale.x = xScale;
		tempScale.y = yScale;
		edge.transform.localScale = tempScale;
		edge.AddComponent<SpriteRenderer>();
		edge.GetComponent<SpriteRenderer>().sprite = borderSprite;
	}

	public void CheckForBossSpawn(){
		bool bossShouldSpawn = true;

		if(capturePoints.Count == 0) return;

		foreach(CapturePoint cp in capturePoints){
			if(cp.controlledBy != CapturePoint.ControlledBy.Player){
				bossShouldSpawn = false;
			}
		}

		if(bossShouldSpawn && Spawner.S){
			Spawner.S.SpawnBoss();
		}

	}

	IEnumerator EndGame(){
		yield return new WaitForSeconds(3);
		Application.LoadLevel("WinScreen");
	}

	public void End(){
		StartCoroutine(EndGame());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

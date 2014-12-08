using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	public static GameManager S;
	public bool getPlanetOnStart = false;

	public float mapSize;

	public Sprite borderSprite;

	public List<Color> playerColors; 
	public List<CapturePoint> capturePoints = new List<CapturePoint>();
	public GameObject directionIndicator;
	List<GameObject> capturePointDirIndicators = new List<GameObject>();

	public BaseSatellite furthestPlanet;
	public float furthestAllowedRadius;
	public List<GameObject> enemyList;

	public Sprite capturePlanetSprite;

	public List<GameObject> directionIndicators;

	float timer = 0;

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
		enemyList = new List<GameObject>();
		playerColors = new List<Color>();
	}

	/// <summary>
	/// Acquires the captures.
	/// This happens at the beginning of the game - not the menus
	/// It will simply grab all objects with tag "Planet" and add them to the list
	/// </summary>
	public void AcquireCaptures(){
		GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");

		foreach(GameObject planet in planets){
			CapturePoint cp = planet.GetComponent<CapturePoint>();
			capturePoints.Add (cp);
		}

		CheckFurthestPlanet();
	}

	/// <summary>
	/// Checks the furthest planet.
	/// This is used to limit player movement to the planet outside the furthest one they own
	/// </summary>
	public void CheckFurthestPlanet(){
		//First find the planet owned by the player that is furthest out
		float furthestRadius = 0;
		foreach(CapturePoint cp in capturePoints){
			
			BaseSatellite bs = cp.GetComponent<BaseSatellite>();
			if(bs.orbitRadius > furthestRadius && cp.controlledBy == CapturePoint.ControlledBy.Player){
				furthestRadius = bs.orbitRadius;
			}
		}

		//What I actually want is the radius of the planet that is next furthest out
		float tempRadius = Mathf.Infinity;
		foreach(CapturePoint cp in capturePoints){
			BaseSatellite bs = cp.GetComponent<BaseSatellite>();
			if(bs.orbitRadius > furthestRadius && bs.orbitRadius < tempRadius){
				tempRadius = bs.orbitRadius;
				furthestPlanet = bs;
			}
		}
		if(tempRadius == Mathf.Infinity){
			tempRadius = furthestRadius;
		}

		furthestAllowedRadius = tempRadius + 5;

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

	/// <summary>
	/// Checks for boss spawn.
	/// When all the capture points are owned by the players, boss will spawn
	/// </summary>
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

	/// <summary>
	/// Shows the capture points on screen.
	/// The capture points are indicated on screen by, currently, circles
	/// at the edge of the screen. This could easily be changed, it's just a prefab
	/// </summary>
	public void ShowCapturePointsOnScreen(){
		foreach(GameObject go in capturePointDirIndicators){
			Destroy(go);
		}
		capturePointDirIndicators.RemoveRange(0, capturePointDirIndicators.Count);

		foreach(CapturePoint cp in capturePoints){
			BaseSatellite bs = cp.GetComponent<BaseSatellite>();
			if(bs.orbitRadius > furthestAllowedRadius){
				continue;
			}

			Vector3 viewportPoint = Camera.main.WorldToViewportPoint(cp.transform.position);
			float x = viewportPoint.x;
			float y = viewportPoint.y;

			if(x >= 0 && x <= 1 && y >= 0 && y <= 1){
				continue;
			}

			//A little bit of math to determine where the line between the
			//center of the screen and the capture point falls on the edge of 
			//the screen
			x -= 0.5f;
			y -= 0.5f;
			float slope = y/x;

			Vector3 newPoint = viewportPoint;
			float newX, newY;
			newX = x;
			newY = y;

			float edgePoint = .45f;

			if(y > 0.5f){
				newX = edgePoint / slope;
				newY = edgePoint;
				if(newX > 0.5f){
					newX = edgePoint;
					newY = edgePoint * slope;
				}
				if(newX < -0.5f){
					newX = -edgePoint;
					newY = -edgePoint * slope;
				}
			}
			else if(y < -0.5f){
				newY = -edgePoint;
				newX = -edgePoint / slope;
				if(newX > 0.5f){
					newX = edgePoint;
					newY = edgePoint * slope;
				}
				if(newX < -0.5f){
					newX = -edgePoint;
					newY = -edgePoint * slope;
				}
			}
			else if(x > 0.5f){
				newY = edgePoint * slope;
				newX = edgePoint;
			}
			else if(x < -0.5f){
				newY = -edgePoint * slope;
				newX = -edgePoint;
			}
			newX += 0.5f;
			newY += 0.5f;

			newPoint.x = newX;
			newPoint.y = newY;


			
			float distanceFromPlayers = (cp.transform.position - Camera.main.transform.position).magnitude;
			float mapIncrements = mapSize*2 / directionIndicators.Count;

			GameObject indicatorToUse = directionIndicators[0];

			for(int i = directionIndicators.Count; i > 0; --i){
				if(distanceFromPlayers < mapIncrements * i){
					indicatorToUse = directionIndicators[i-1];
				}
			}

			//put a capture point indicator at the edge of the screen
			//and color it based on how far/close it is
			Vector3 newWorldPoint = Camera.main.ViewportToWorldPoint(newPoint);
			newWorldPoint.z = 0;
			GameObject dirIndicator = Instantiate(indicatorToUse, newWorldPoint, Quaternion.identity) as GameObject;
			dirIndicator.transform.parent = Camera.main.transform;

			Vector3 lookAtPoint = cp.transform.position;
			lookAtPoint.z = 0;
			Vector3 lookDir = lookAtPoint - newWorldPoint;
			dirIndicator.transform.up = lookDir;


			capturePointDirIndicators.Add (dirIndicator);

		
			Color color = cp.color;
			dirIndicator.GetComponent<SpriteRenderer>().material.color = color;

			float closestSquad = Mathf.Infinity;
			foreach(EnemySquad squad in SquadManager.S.squads){
				if(squad.squadMembers.Count > 0){
					if(squad.targetIsPlanet){
						if(squad.target.GetComponent<CapturePoint>() == cp){
							if((squad.squadCenter - cp.transform.position).magnitude < closestSquad){
								closestSquad = (squad.squadCenter - cp.transform.position).magnitude;
							}
						}
					}
				}
			}

			if(closestSquad < 10){
				closestSquad = 10;
			}
			if(closestSquad < Mathf.Infinity){
				float indicatorFlicker = closestSquad / GameManager.S.mapSize * 10f;
				if(timer % indicatorFlicker < indicatorFlicker / 2){
					dirIndicator.GetComponent<SpriteRenderer>().material.color = Color.white;
				}
			}


		}
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(timer > 60){
			timer = 0;
		}
		ShowCapturePointsOnScreen();
	}
}

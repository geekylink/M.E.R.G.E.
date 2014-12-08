using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	public static Spawner S;

	public List<GameObject> enemiesToSpawn;
	public GameObject squadPrefab;
	public float spawnTimer;

	public Vector3[] spawnLocs;

	public float mapSize;

	public GameObject boss;
	public float bossSpawnTimer;
	public Vector3 bossSpawnLoc;

	
	public float distanceLimit;
	public float velocityMatchFactor;
	public float towardTargetFactor;
	public float towardCenterFactor;
	public float separationFactor;


	public int minSquadSize;
	public int maxSquadSize;

	public GameObject bossOnScreen;
	//public UnityEngine.UI.Text bossTimer;

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

	IEnumerator SpawnCoroutine(){
		float timer = 0;
		
		while(timer < 1){
			timer += Time.deltaTime * Time.timeScale / spawnTimer;
			yield return 0;
		}
		
		int randomEnemy = Random.Range (0, enemiesToSpawn.Count);
		int side = Random.Range (0, 4);
		
		Vector2 posToSpawn = Vector2.zero;
		if(side == 0){
			posToSpawn.y = mapSize; 
			posToSpawn.x = (Random.value - 0.5f) * 2 * mapSize;
		}
		else if(side == 1){
			posToSpawn.y = -mapSize;
			posToSpawn.x = (Random.value - 0.5f) * 2 * mapSize;
		}
		else if(side == 2){
			posToSpawn.x = mapSize;
			posToSpawn.y = (Random.value - 0.5f) * 2 * mapSize;
		}
		else{
			posToSpawn.x = -mapSize;
			posToSpawn.y = (Random.value - 0.5f) * 2 * mapSize;
		}
		//posToSpawn = minimapCam.ViewportToWorldPoint(posToSpawn);
		
		GameObject enemyGO = Instantiate (enemiesToSpawn[randomEnemy]) as GameObject;
		enemyGO.transform.position = posToSpawn;
		enemyGO.transform.right = posToSpawn;
		
		StartCoroutine(SpawnCoroutine());
	}

	IEnumerator SpawnSquad(){
		float timer = 0;
		
		while(timer < 1){
			timer += Time.deltaTime * Time.timeScale / spawnTimer;
			yield return 0;
		}

		GameObject eSquadGO = Instantiate (squadPrefab) as GameObject;
		EnemySquad eSquad = eSquadGO.GetComponent<EnemySquad>();

		int ranValue = Random.Range (0, 5);
		if(ranValue == 0){
			eSquad.targetIsPlanet = true;

			CapturePoint furthestPlayerPlanet = null;
			float furthestRadius = 0;
			foreach(CapturePoint cp in GameManager.S.capturePoints){
				
				BaseSatellite bs = cp.GetComponent<BaseSatellite>();
				if(bs.orbitRadius > furthestRadius && cp.controlledBy == CapturePoint.ControlledBy.Player){
					furthestRadius = bs.orbitRadius;
					furthestPlayerPlanet = cp;
				}
			}

			if(furthestPlayerPlanet == null){
				ranValue = 1;
			}
			else{
				eSquad.target = furthestPlayerPlanet.gameObject;
			}
		}
		if(ranValue > 0){
			eSquad.targetIsPlanet = false;
			eSquad.target = Camera.main.gameObject;
		}

		/*int i = Random.Range (0, 4);
		Vector2 loc = SquadManager.S.startingLocations [Random.Range (0, SquadManager.S.startingLocations.Count)];
		List<Vector2> enemyLocs = new List<Vector2>();
		if (i == 0) {
			enemyLocs = SquadManager.S.ThreeSquad (loc);
		} else if (i == 1) {
			enemyLocs = SquadManager.S.FourSquad (loc);
		} else if (i == 2) {
			enemyLocs = SquadManager.S.FiveSquad (loc);
		} else {
			enemyLocs = SquadManager.S.SevenSquad (loc);
		}*/

		int squadSize = Random.Range(minSquadSize, maxSquadSize);
		if(eSquad.targetIsPlanet) squadSize *= 2;

		eSquad.squadMembers = new List<EnemyBaseShip> ();
		Vector2 planetSpawnPos = Vector2.zero;
		List<CapturePoint> cPoints = GameManager.S.capturePoints;
		int ranNum = Random.Range (0, cPoints.Count);
		for(int i = 0; i < cPoints.Count; ++i){
			ranNum = ranNum % cPoints.Count;

			if(cPoints[ranNum].controlledBy == CapturePoint.ControlledBy.Enemy){
				planetSpawnPos = cPoints[ranNum].transform.position;
				break;
			}
			ranNum++;
		}
		if(planetSpawnPos != Vector2.zero){

			for (int i = 0; i < squadSize; ++i) {
				GameObject squadMemberGO = Instantiate(enemiesToSpawn[Random.Range(0,enemiesToSpawn.Count)]) as GameObject;
				EnemyBaseShip ship = squadMemberGO.GetComponent<EnemyBaseShip>();
				ship.boidInit = true;
				eSquad.squadMembers.Add(ship);
				ship.squadId = SquadManager.nextID;

				float randomX = Random.Range (-10.0f, 10.0f);
				float randomY = Random.Range (-10.0f, 10.0f);
				Vector3 pos = new Vector3(randomX, randomY, 0);

				squadMemberGO.transform.position = pos + (Vector3)planetSpawnPos;

				if(eSquad.targetIsPlanet){
					CapturePoint cp = eSquad.target.GetComponent<CapturePoint>();
					if(cp.satsInOrbit.Count > 0){
						int randomSat = Random.Range (0, cp.satsInOrbit.Count);
						ship.currTarget = cp.satsInOrbit[randomSat].gameObject;
					}
				}
				else{
					ship.currTarget = ship.getRandomPlayer();
				}

				ship.StartRoutines();
			}
			SquadManager.S.squads.Add (eSquad);
			SquadManager.nextID++;
		}
		StartCoroutine (SpawnSquad ());
	}

	IEnumerator SpawnBossCo(){
		float timer = 0;
		
		while(timer < bossSpawnTimer){
			timer += Time.deltaTime * Time.timeScale;
			//int timeUntilSpawn = Mathf.RoundToInt(bossSpawnTimer - timer);
			//bossTimer.text = "Time Until Boss Spawn: " + timeUntilSpawn;
			yield return 0;
		}

		SpawnBoss();
	}

	public void SpawnBoss(){
		bossOnScreen = Instantiate (boss) as GameObject;
		bossOnScreen.transform.position = bossSpawnLoc;
	}

	void Awake(){
		//StartCoroutine(SpawnCoroutine());
		StartCoroutine (SpawnSquad ());
	}

	/*public GameObject getRandomPlayer() {
		GameObject cam = GameObject.Find ("Main Camera");
		PlayerManager pm = cam.GetComponent ("PlayerManager") as PlayerManager;
		GameObject[] players = pm.getPlayers ();
		int randomNum = Random.Range (0, players.Length);


		if(players.Length != 0){
			
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
		return null;
			
	}*/

}

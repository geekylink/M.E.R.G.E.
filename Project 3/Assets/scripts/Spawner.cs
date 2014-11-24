using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	public List<GameObject> enemiesToSpawn;
	public GameObject squadPrefab;
	public float spawnTimer;

	public List<int> squad1;
	public Vector3[] spawnLocs;

	public float mapSize;

	public GameObject boss;
	public float bossSpawnTimer;
	public Vector3 bossSpawnLoc;
	public UnityEngine.UI.Text bossTimer;

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
		
		while(timer < 7){
			timer += Time.deltaTime * Time.timeScale / spawnTimer;
			yield return 0;
		}

		GameObject eSquadGO = squadPrefab;
		EnemySquad eSquad = eSquadGO.GetComponent<EnemySquad>();

		while(eSquad.target == null){
			eSquad.target = getRandomPlayer();
			yield return 0;
		}

		int i = Random.Range (0, 4);
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
		}

		eSquad.squadMembers = new List<EnemyBaseShip> ();

		foreach (Vector2 eLoc in enemyLocs) {
			GameObject squadMemberGO = Instantiate(enemiesToSpawn[Random.Range(0,2)]) as GameObject;
			EnemyBaseShip ship = squadMemberGO.GetComponent<EnemyBaseShip>();
			eSquad.squadMembers.Add(ship);
			ship.squadId = SquadManager.nextID;
			squadMemberGO.transform.position = eLoc;
			ship.currTarget = eSquad.target;
		}
		SquadManager.S.squads.Add (eSquad);
		SquadManager.nextID++;
		StartCoroutine (SpawnSquad ());
	}

	IEnumerator SpawnBoss(){
		float timer = 0;
		
		while(timer < bossSpawnTimer){
			timer += Time.deltaTime * Time.timeScale;
			int timeUntilSpawn = Mathf.RoundToInt(bossSpawnTimer - timer);
			bossTimer.text = "Time Until Boss Spawn: " + timeUntilSpawn;
			yield return 0;
		}

		
		GameObject bossGO = Instantiate (boss) as GameObject;
		bossGO.transform.position = bossSpawnLoc;
	}

	void Awake(){
		StartCoroutine(SpawnCoroutine());
		StartCoroutine(SpawnBoss());
		StartCoroutine (SpawnSquad ());
	}

	public GameObject getRandomPlayer() {
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
			
	}

}

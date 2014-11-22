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

	// Use this for initialization
	void Start () {

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
		int i = 0;
		GameObject eSquadGO = squadPrefab;
		EnemySquad eSquad = eSquadGO.GetComponent<EnemySquad>();
		eSquad.squadMembers = new List<EnemyBaseShip> ();
		eSquad.target = getRandomPlayer ();
		SquadManager.S.squads.Add (eSquad);
		foreach (int enemy in squad1) {
			GameObject squadMemberGO = Instantiate(enemiesToSpawn[enemy]) as GameObject;
			EnemyBaseShip ship = squadMemberGO.GetComponent<EnemyBaseShip>();
			eSquad.squadMembers.Add(ship);
			ship.squadId = SquadManager.nextID;
			squadMemberGO.transform.position = spawnLocs[i++];
			ship.currTarget = eSquad.target;
		}
		SquadManager.nextID++;
		yield return 0;
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

	// Update is called once per frame
	void Update () {
	}
}

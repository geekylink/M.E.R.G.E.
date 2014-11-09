using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	public List<GameObject> enemiesToSpawn;

	public float spawnTimer;

	public float mapSize;

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

	void Awake(){
		StartCoroutine(SpawnCoroutine());
	}
	
	// Update is called once per frame
	void Update () {
	}
}

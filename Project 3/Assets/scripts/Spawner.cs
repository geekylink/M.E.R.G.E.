using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	public List<GameObject> enemiesToSpawn;

	public float spawnTimer;

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
			posToSpawn.y = 1.1f; 
			posToSpawn.x = Random.value;
		}
		else if(side == 1){
			posToSpawn.y = -.1f;
			posToSpawn.x = Random.value;
		}
		else if(side == 2){
			posToSpawn.x = 1.1f;
			posToSpawn.y = Random.value;
		}
		else{
			posToSpawn.x = -.1f;
			posToSpawn.y = Random.value;
		}
		posToSpawn = Camera.main.ViewportToWorldPoint(posToSpawn);
		
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

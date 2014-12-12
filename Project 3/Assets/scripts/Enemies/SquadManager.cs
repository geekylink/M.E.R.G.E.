using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquadManager : MonoBehaviour {

	public static int nextID;
	public List<EnemySquad> squads;
	public static SquadManager S { get; private set; }
	public List<Vector2> startingLocations;
	public float mapsize;
	public float lowLimit;
	public float highLimit;
	public float randomness;
	public float separationWeight;
	public float followWeight;

	// Use this for initialization
	void Start () {
		if (S == null) {
			S = this;
		} else if (S != this) {
			Destroy (this.gameObject);
		}
		//mapsize = GameManager.S.mapSize;
		mapsize = 60;
		nextID = 1;
		squads = new List<EnemySquad>();
		startingLocations = new List<Vector2> ();
	}

	EnemySquad getRandomSquad(){
		int id = Random.Range (0, squads.Count);
		return squads [id];
	}

	public void RemoveEnemy(EnemyBaseShip enemy, int id){
		if(id != 0){
			if(squads.Count < id) return;
			squads [id - 1].squadMembers.Remove (enemy);
			if(squads[id - 1].squadMembers.Count == 0){
				//squads.RemoveAt(id - 1);
			}
		}
	}
}

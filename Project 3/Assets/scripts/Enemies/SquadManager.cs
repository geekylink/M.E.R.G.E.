using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquadManager : MonoBehaviour {

	public static int nextID;
	public List<EnemySquad> squads;
	public static SquadManager S { get; private set; }

	// Use this for initialization
	void Awake () {
		if (S == null) {
			S = this;
		} else if (S != this) {
			Destroy (this.gameObject);
		}
		nextID = 1;
		squads = new List<EnemySquad>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public Vector3 Boids(EnemyBaseShip enemy, int squadId){
		return squads [squadId - 1].ApplyBoids (enemy);
	}

	EnemySquad getRandomSquad(){
		int id = Random.Range (0, squads.Count);
		return squads [id];
	}

	public void RemoveEnemy(EnemyBaseShip enemy, int id){

		if(id != 0){
			squads [id - 1].squadMembers.Remove (enemy);
		}

	}
}

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquadManager : MonoBehaviour {

	public static int nextID;
	public List<EnemySquad> squads;
	public static SquadManager S { get; private set; }
	public List<Vector2> startingLocations;
	public float mapsize;

	// Use this for initialization
	void Start () {
		if (S == null) {
			S = this;
		} else if (S != this) {
			Destroy (this.gameObject);
		}
		nextID = 1;
		squads = new List<EnemySquad>();
		startingLocations = new List<Vector2> ();
		SetSpawnLocs ();
	}

	public void SetSpawnLocs(){
		startingLocations.Add(new Vector2(mapsize - 6, mapsize / 2));
		startingLocations.Add(new Vector2(mapsize - 6, 0));
		startingLocations.Add(new Vector2(mapsize - 6, -mapsize / 2));
		startingLocations.Add(new Vector2(-mapsize + 6, mapsize / 2));
		startingLocations.Add(new Vector2(- mapsize + 6, -mapsize / 2));
		startingLocations.Add(new Vector2(-mapsize + 6, 0));
		startingLocations.Add(new Vector2(mapsize / 2, mapsize - 6));
		startingLocations.Add(new Vector2(0, mapsize - 6));
		startingLocations.Add(new Vector2(-mapsize / 2, mapsize - 6));
		startingLocations.Add(new Vector2(mapsize / 2, -mapsize + 6));
		startingLocations.Add(new Vector2(0, -mapsize + 6));
		startingLocations.Add(new Vector2(-mapsize / 2, -mapsize + 6));
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

	public List<Vector2> ThreeSquad(Vector2 loc){
		List<Vector2> squadLocs = new List<Vector2>();
		squadLocs.Add (loc);
		squadLocs.Add (new Vector2 (loc.x - 6, loc.y + 3));
		squadLocs.Add (new Vector2 (loc.x - 6, loc.y - 3));
		return squadLocs;
	}

	public List<Vector2> FourSquad(Vector2 loc){
		List<Vector2> squadLocs = new List<Vector2>();
		squadLocs = FiveSquad (loc);
		squadLocs.Remove (loc);
		return squadLocs;
	}

	public List<Vector2> FiveSquad(Vector2 loc){
		List<Vector2> squadLocs = new List<Vector2>();
		squadLocs.Add (loc);
		squadLocs.Add (new Vector2 (loc.x - 6, loc.y + 3));
		squadLocs.Add (new Vector2 (loc.x - 6, loc.y - 3));
		squadLocs.Add (new Vector2 (loc.x + 6, loc.y + 3));
		squadLocs.Add (new Vector2 (loc.x + 6, loc.y - 3));
		return squadLocs;
	}

	public List<Vector2> SevenSquad(Vector2 loc){
		List<Vector2> squadLocs = new List<Vector2>();
		squadLocs.Add (loc);
		squadLocs.Add (new Vector2 (loc.x - 6, loc.y + 3));
		squadLocs.Add (new Vector2 (loc.x - 6, loc.y - 3));
		squadLocs.Add (new Vector2 (loc.x, loc.y + 6));
		squadLocs.Add (new Vector2 (loc.x, loc.y - 6));
		squadLocs.Add (new Vector2 (loc.x + 6, loc.y + 3));
		squadLocs.Add (new Vector2 (loc.x + 6, loc.y - 3));
		return squadLocs;
	}


}

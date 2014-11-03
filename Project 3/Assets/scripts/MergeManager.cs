using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MergeManager : MonoBehaviour {

	public GameObject mergeImage;
	public float mergeDistance;

	List<Player> players = new List<Player>();
	List<Color> normPlayerColor;

	List<GameObject> currMergeImages = new List<GameObject>();
	List<List<Player>> tryingToMergeWith = new List<List<Player>>();
	List<List<Player>> currentlyMergedWith = new List<List<Player>>();

	// Use this for initialization
	void Awake () {
		Player[] playArr = FindObjectsOfType(typeof(Player)) as Player[];
		players.AddRange(playArr);
		
		for(int i = 0; i < players.Count; ++i){
			tryingToMergeWith.Add(new List<Player>());
			currentlyMergedWith.Add(new List<Player>());
		}
	}
	
	// Update is called once per frame
	void Update () {
		CheckPlayerDistance ();
		Merge ();
	}


	void CheckPlayerDistance(){

		foreach(GameObject go in currMergeImages){
			Destroy (go);
		}
		currMergeImages.RemoveRange(0, currMergeImages.Count);
		foreach(List<Player> p in tryingToMergeWith){
			p.RemoveRange(0, p.Count);
		}

		for(int i = 0; i < players.Count; ++i){
			Vector2 posCurr = players[i].transform.position;
			for(int j = i + 1; j < players.Count; ++j){
				Vector2 posComp = players[j].transform.position;

				float dist = Mathf.Abs (Vector2.Distance(posCurr, posComp));
				if(dist < mergeDistance){


					Vector2 dir = posComp - posCurr;
					if(players[i].TryingToMerge && players[j].TryingToMerge && (!currentlyMergedWith[i].Contains(players[j]))){
						Debug.DrawRay(posCurr, dir, Color.green);
						tryingToMergeWith[i].Add (players[j]);
					}
					else{
						
						GameObject mImage = Instantiate(mergeImage) as GameObject;
						mImage.transform.position = posCurr + dir/2;
						currMergeImages.Add (mImage);
					}
				}
			}
		}




	}

	void Merge(){
		for(int i = 0; i < players.Count; ++i){
			Player pOne = players[i];

			Vector2 posOne = pOne.transform.position;
			for(int j = 0; j < tryingToMergeWith[i].Count; ++j){
				Player pTwo = tryingToMergeWith[i][j];

				currentlyMergedWith[i].Add (pTwo);
				currentlyMergedWith[players.IndexOf(pTwo)].Add (pOne);

				Vector2 posTwo = pTwo.transform.position;
				Vector2 dir = posOne - posTwo;


				GameObject merged = new GameObject();
				Rigidbody2D mergedRB = merged.AddComponent<Rigidbody2D>();
				merged.transform.position = posOne + dir/2;

				mergedRB.mass = pOne.rigidbody2D.mass + pTwo.rigidbody2D.mass;
				mergedRB.velocity = pOne.rigidbody2D.velocity + pTwo.rigidbody2D.velocity;
				mergedRB.angularVelocity = pOne.rigidbody2D.angularVelocity + pTwo.rigidbody2D.angularVelocity;
				mergedRB.gravityScale = 0;
				
				
				pOne.transform.parent = merged.transform;
				Destroy (pOne.rigidbody2D);
				pTwo.transform.parent = merged.transform;
				Destroy (pTwo.rigidbody2D);
			}
		}
	}

}

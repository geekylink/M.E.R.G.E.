using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MergeManager : MonoBehaviour {
	public static MergeManager S;

	public GameObject mergeImage;
	public float mergeDistance;
	public float rotMergeTime;

	List<Player> players = new List<Player>();
	List<Color> normPlayerColor;

	List<GameObject> mergeVisualCues = new List<GameObject>();

	List<List<Player>> currentlyMergedWith = new List<List<Player>>();

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

	// Use this for initialization
	void Awake () {
		Player[] playArr = FindObjectsOfType(typeof(Player)) as Player[];
		players.AddRange(playArr);
		
		for(int i = 0; i < players.Count; ++i){
			currentlyMergedWith.Add(new List<Player>());
		}
	}
	
	// Update is called once per frame
	void Update () {
		int[] mergers = MergeChecker ();
		MergeSetup(mergers);
	}


	int[] MergeChecker(){

		foreach(GameObject go in mergeVisualCues){
			Destroy (go);
		}
		mergeVisualCues.RemoveRange(0, mergeVisualCues.Count);

		int[] mergingAtNextUpdateWith = new int[players.Count];
		for(int i = 0; i < players.Count; ++i){
			mergingAtNextUpdateWith[i] = -1;
		}

		for(int i = 0; i < players.Count; ++i){
			if(mergingAtNextUpdateWith[i] != -1){
				continue;
			}

			Vector2 posCurr = players[i].transform.position;
			if(players[i].transform.parent != null){
				posCurr = players[i].transform.parent.position;
			}


			for(int j = i + 1; j < players.Count; ++j){
				if(mergingAtNextUpdateWith[j] != -1){
					continue;
				}

				Vector2 posComp = players[j].transform.position;
				if(players[j].transform.parent != null){
					posComp = players[j].transform.parent.position;
				}

				float dist = Mathf.Abs (Vector2.Distance(posCurr, posComp));
				if(dist < mergeDistance){
					Vector2 dir = posComp - posCurr;
					if(players[i].TryingToMerge && players[j].TryingToMerge && (!currentlyMergedWith[i].Contains(players[j]))){
						Debug.DrawRay(posCurr, dir, Color.green);
						mergingAtNextUpdateWith[i] = j;
						mergingAtNextUpdateWith[j] = i;
					}
					else{
						if(!currentlyMergedWith[i].Contains(players[j])){
							GameObject mImage = Instantiate(mergeImage) as GameObject;
							mImage.transform.position = posCurr + dir/2;
							mergeVisualCues.Add (mImage);
						}
					}
				}
			}
		}
		return mergingAtNextUpdateWith;
	}

	void MergeSetup(int[] mergers){
		bool[] wantToSkip = new bool[players.Count];
		for(int i = 0; i < players.Count; ++i){
			wantToSkip[i] = false;
		}

		for(int i = 0; i < players.Count; ++i){
			if(mergers[i] == -1) continue;
			if(wantToSkip[i]) continue;
			if(wantToSkip[mergers[i]]) continue;

			int numMergedFirst = currentlyMergedWith[i].Count;
			int numMergedSecond = currentlyMergedWith[mergers[i]].Count;

			if(numMergedFirst == 0 && numMergedSecond == 0){
				wantToSkip[mergers[i]] = true;
				Merge_And_CreateNewCombinedShip(i, mergers[i]);
			}
			else if(numMergedFirst != 0 && numMergedSecond == 0){
				wantToSkip[mergers[i]] = true;
				Merge_AddToExistingShip(i, mergers[i]);
			}
			else if(numMergedFirst == 0 && numMergedSecond != 0){
				wantToSkip[mergers[i]] = true;
				Merge_AddToExistingShip(mergers[i], i);
			}
			else{
				for(int j = 0; j < players.Count; ++j){
					wantToSkip[j] = true;
				}
				Merge_TwoLargerShips(i, mergers[i]);

			}
		}
	}

	void Merge_TwoLargerShips(int p1, int p2){
		Player pOne = players[p1];
		Player pTwo = players[p2];

		Player pThree = currentlyMergedWith[p2][0];
		int p3 = players.IndexOf(pThree);

		Player pFour = currentlyMergedWith[p1][0];
		int p4 = players.IndexOf(pFour);
		
		currentlyMergedWith[p1].Add (pTwo);
		currentlyMergedWith[p1].Add (pThree);
		currentlyMergedWith[p4].Add (pTwo);
		currentlyMergedWith[p4].Add (pThree);
		currentlyMergedWith[p2].Add (pOne);
		currentlyMergedWith[p3].Add (pOne);
		currentlyMergedWith[p2].Add (pFour);
		currentlyMergedWith[p3].Add (pFour);
		
		MergedShip mergedShipScript = pOne.transform.parent.GetComponent<MergedShip>();
		Rigidbody2D mergedRB = mergedShipScript.gameObject.rigidbody2D;

		GameObject oldMergedGO = pTwo.transform.parent.gameObject;
		Rigidbody2D oldMergedRB = oldMergedGO.rigidbody2D;

		mergedRB.mass += oldMergedRB.mass;
		mergedRB.velocity += oldMergedRB.velocity;
		mergedRB.angularVelocity += oldMergedRB.angularVelocity;


		pTwo.transform.parent = mergedShipScript.transform;
		pThree.transform.parent = mergedShipScript.transform;

		Destroy(oldMergedGO);
		
		mergedShipScript.AddShip(pTwo, p2);
		mergedShipScript.AddShip(pThree, p3);

	}

	void Merge_AddToExistingShip(int p1, int p2){
		Player pOne = players[p1];
		Player pTwo = players[p2];

		currentlyMergedWith[p1].Add (pTwo);
		currentlyMergedWith[p2].Add (pOne);
				
		MergedShip mergedShipScript = pOne.transform.parent.GetComponent<MergedShip>();
		Rigidbody2D mergedRB = mergedShipScript.gameObject.rigidbody2D;
		
		mergedRB.mass += pTwo.rigidbody2D.mass;
		mergedRB.velocity += pTwo.rigidbody2D.velocity;
		mergedRB.angularVelocity += pTwo.rigidbody2D.angularVelocity;

		pTwo.transform.parent = mergedShipScript.transform;
		Destroy (pTwo.rigidbody2D);

		mergedShipScript.AddShip(pTwo, p2);

		foreach(Player p in currentlyMergedWith[p1]){
			if(p == pTwo) continue;

			int pNum = players.IndexOf(p);
			if(!currentlyMergedWith[pNum].Contains(pTwo)){
				currentlyMergedWith[pNum].Add(pTwo);
			}
			if(!currentlyMergedWith[p2].Contains(p)){
				currentlyMergedWith[p2].Add(p);
			}
		}

	}


	void Merge_And_CreateNewCombinedShip(int p1, int p2){
		Player pOne = players[p1];
		Player pTwo = players[p2];

		Vector2 posOne = pOne.transform.position;
		Vector2 posTwo = pTwo.transform.position;

		currentlyMergedWith[p1].Add (pTwo);
		currentlyMergedWith[p2].Add (pOne);

		Vector2 dir = posOne - posTwo;

		GameObject merged = new GameObject();
		Rigidbody2D mergedRB = merged.AddComponent<Rigidbody2D>();
		MergedShip mergedShipScript = merged.AddComponent<MergedShip>();

		
		for(int i = 0; i < 4; ++i){
			mergedShipScript.shipsInPosition[i] = false;
			mergedShipScript.pNumAtPosition[i] = -1;
		}

		mergedShipScript.Bounciness = pOne.bounciness;
		mergedShipScript.RotMergeTime = rotMergeTime;

		merged.transform.position = posTwo + dir/2;
		Vector2 avgRightVector = (pOne.transform.right + pTwo.transform.right).normalized;
		merged.transform.right = avgRightVector;


		mergedRB.mass = pOne.rigidbody2D.mass + pTwo.rigidbody2D.mass;
		mergedRB.velocity = pOne.rigidbody2D.velocity + pTwo.rigidbody2D.velocity;
		mergedRB.angularVelocity = pOne.rigidbody2D.angularVelocity + pTwo.rigidbody2D.angularVelocity;
		mergedRB.gravityScale = 0;		
		
		pOne.transform.parent = merged.transform;
		Destroy (pOne.rigidbody2D);
		pTwo.transform.parent = merged.transform;
		Destroy (pTwo.rigidbody2D);

		mergedShipScript.AddShip(pOne, p1);
		mergedShipScript.AddShip(pTwo, p2);

	}
	
	
	public void Unmerge(Player player){
		
		int index = players.IndexOf(player);

		MergedShip mergedShipScript = player.transform.parent.GetComponent<MergedShip>();
		mergedShipScript.RemoveShip(player, index);

		Player mergePartner = null;
		if(currentlyMergedWith[index].Count != 0){
			mergePartner = currentlyMergedWith[index][0];
		}
		currentlyMergedWith[index].RemoveRange(0, currentlyMergedWith[index].Count);

		for(int i = 0; i < players.Count; ++i){
			if(i == index) continue;
			currentlyMergedWith[i].Remove(player);
		}
		Rigidbody2D mergedRB = player.transform.parent.GetComponent<Rigidbody2D>();

		player.transform.parent = null;

		Rigidbody2D newRB = player.gameObject.AddComponent<Rigidbody2D>();
		newRB.mass = 1;
		newRB.velocity = -(Vector2)player.transform.right + mergedRB.velocity;
		newRB.angularVelocity = mergedRB.angularVelocity;
		newRB.gravityScale = 0;

		if(mergedShipScript.NumberOfMergedShips == 1){
			Unmerge (mergePartner);
			Destroy(mergedShipScript.gameObject);
		}

		player.IsCurrentlyMerged = false;
	}

}

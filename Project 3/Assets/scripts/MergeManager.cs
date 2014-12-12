using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using InControl;

public class MergeManager : MonoBehaviour {

	public static MergeManager S;

	//Image used to indicate players can merge, e.g., an Xbox 'A'
	public GameObject mergeImage;
    public GameObject mergeLine;
	//maximum distance at which players can merge
	public float mergeDistance;
	//time it takes for players to merge into the ship.

	public float rotMergeTime;
	public float rumbleDuration = .25f;


	public List<Player> players = new List<Player>();

	//list of images telling players they can merge
	List<GameObject> mergeVisualCues = new List<GameObject>();
	List<bool> iSignalJ = new List<bool> ();

	//Who each player is merged with
	public List<List<Player>> currentlyMergedWith = new List<List<Player>>();

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
		GameObject.FindGameObjectWithTag ("InControl").GetComponent<InControlManager> ().enableXInput = true;
		for (int i = 0; i < InputManager.Devices.Count; i++) {
			for (int j = 0; j < InputManager.Devices.Count; j++) {
				iSignalJ.Add (false);
			}
		}
	}

	public void AddPlayer(Player player, int index){

		if(index >= players.Count){
			players.Add (player);
			currentlyMergedWith.Add (new List<Player>());
		}
		else{
			players[index] = player;
		}

	}
	
	// Update is called once per frame
	void Update () {
		int[] mergers = MergeChecker ();
		MergeSetup(mergers);
	}


	IEnumerator Rumble(InputDevice device){
		InputManager.EnableXInput = true;
		float rumbleTime = 0f;
		device.Vibrate (3, 3);
		print ("RumbleStart");
		while (rumbleTime < rumbleDuration) {
			rumbleTime += Time.deltaTime;
			yield return 0;
		}
		device.Vibrate (0, 0);
		print ("RumbleStop");
	}

	int[] MergeChecker(){
		//Destroy all visual cues currently in the scene
		foreach(GameObject go in mergeVisualCues){
			Destroy (go);
		}
		mergeVisualCues.RemoveRange(0, mergeVisualCues.Count);
		//Make an array which will tell who is going to merge after this function runs
		int[] mergingAtNextUpdateWith = new int[players.Count];
		for(int i = 0; i < players.Count; ++i){
			mergingAtNextUpdateWith[i] = -1;
		}
		//loop through all players
		for(int i = 0; i < players.Count; ++i){
			if(mergingAtNextUpdateWith[i] != -1){
				continue;
			}
			if(players[i] == null) continue;

			Vector2 posCurr = players[i].transform.position;
			if(players[i].transform.parent != null){
				posCurr = players[i].transform.parent.position;
			}

			//loop through all other players
			for(int j = i + 1; j < players.Count; ++j){
				if(mergingAtNextUpdateWith[j] != -1){
					continue;
				}
				if(players[j] == null) continue;
				Vector2 posComp = players[j].transform.position;
				if(players[j].transform.parent != null){
					posComp = players[j].transform.parent.position;
				}
				//check if close enough
				float dist = Mathf.Abs (Vector2.Distance(posCurr, posComp));
				if(dist < mergeDistance){
					Vector2 dir = posComp - posCurr;
					//check if both are pressing the merge button, and are not merged with each other
					//if((players[i].TryingToMerge || players[j].TryingToMerge) && (!currentlyMergedWith[i].Contains(players[j]))){
					if((players[i].TryingToMerge && players[j].TryingToMerge) && (!currentlyMergedWith[i].Contains(players[j])) && !(players[i].IsMerging || players[j].IsMerging)){
						Debug.DrawRay(posCurr, dir, Color.green);
						mergingAtNextUpdateWith[i] = j;
						mergingAtNextUpdateWith[j] = i;
						//will tell MergeSetup who is going to merge
					}
					else{
						if(!currentlyMergedWith[i].Contains(players[j])){
							//If they aren't currently merged, let them know they could if they wanted
							GameObject mImage = Instantiate(mergeImage) as GameObject;
							mImage.transform.position = posCurr + dir/2;
							mergeVisualCues.Add (mImage);

                            if(mergeLine)
                            { 
                                GameObject mLine1 = Instantiate(mergeLine) as GameObject;
                                GameObject mLine2 = Instantiate(mergeLine) as GameObject;

                                LineRenderer mRender1 = mLine1.GetComponent<LineRenderer>();
                                mRender1.SetPosition(0, posCurr);
                                mRender1.SetPosition(1, posCurr + dir / 2);

                                LineRenderer mRender2 = mLine2.GetComponent<LineRenderer>();
                                mRender2.SetPosition(0, posComp);
                                mRender2.SetPosition(1, posCurr + dir / 2);
                                if(players[i].TryingToMerge)
                                {
                                    mRender1.SetColors(Color.green, new Color(0,200,0,166));
									if(iSignalJ[i * j] == false){
										StartCoroutine("Rumble", InputManager.Devices[j]);
										iSignalJ[i * j] = true;
									}
								}
                                if (players[j].TryingToMerge)
                                {
                                    mRender2.SetColors(Color.green, new Color(0, 200, 0, 166));
									if(iSignalJ[i * j] == false){
										StartCoroutine("Rumble", InputManager.Devices[i]);
										iSignalJ[i * j] = true;
									}
								}
								if(!players[i].TryingToMerge && !players[j].TryingToMerge){
									iSignalJ[i*j] = false;
								}
                                mergeVisualCues.Add(mLine1);
                                mergeVisualCues.Add(mLine2);

                            }

						}
					}
				}
			}
		}
		return mergingAtNextUpdateWith;
	}

	//Deal with merging players
	void MergeSetup(int[] mergers){
		//wantToSkip is just a safeguard to make sure some awkward
		//double merging doesn't ever happen
		bool[] wantToSkip = new bool[players.Count];
		for(int i = 0; i < players.Count; ++i){
			wantToSkip[i] = false;
		}

		for(int i = 0; i < players.Count; ++i){
			if(mergers[i] == -1) continue;			//If they aren't going to merge with someone
			if(wantToSkip[i]) continue;				//Or they should be skip
			if(wantToSkip[mergers[i]]) continue;	//Or their partner should be skipped, do not merge

			wantToSkip[i] = true;

			//Get the number of ships they are merged with
			int numMergedFirst = currentlyMergedWith[i].Count;
			int numMergedSecond = currentlyMergedWith[mergers[i]].Count;


			//We only have a few possibilities here:
			if(numMergedFirst == 0 && numMergedSecond == 0){		//Either neither is currently merged
				wantToSkip[mergers[i]] = true;
				Merge_And_CreateNewCombinedShip(i, mergers[i]);
			}
			else if(numMergedFirst != 0 && numMergedSecond == 0){	//Or the first is (which means just add the second)
				wantToSkip[mergers[i]] = true;
				Merge_AddToExistingShip(i, mergers[i]);
			}
			else if(numMergedFirst == 0 && numMergedSecond != 0){	//Or the second is (add the first)
				wantToSkip[mergers[i]] = true;
				Merge_AddToExistingShip(mergers[i], i);
			}
			else{													//Or they both are, which means there must be two groups of two ships that need to merge
				for(int j = 0; j < players.Count; ++j){
					wantToSkip[j] = true;
				}
				Merge_TwoLargerShips(i, mergers[i]);

			}
		}
	}

	//Merge two groups of two ships together
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

	//Add p2 to the merged ship p1 is a member of (can have 2 or 3 ships already merged)
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

	//Neither p1 nor p2 are in a merged ship, so create one and add them
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
		mergedShipScript.mergedLine = mergeLine;

		
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
	
	//Called when player pushed the unmerge button and is in a merged ship
	public void Unmerge(Player player){
		
		int index = players.IndexOf(player);

		MergedShip mergedShipScript = player.transform.parent.GetComponent<MergedShip>();
		mergedShipScript.RemoveShip(player, index);

		Player mergePartner = null;
		if(currentlyMergedWith[index].Count != 0){
			//If merged, grab the first partner - used later to possibly destroy ship
			mergePartner = currentlyMergedWith[index][0];
		}
		//If merging, let merging partner know it doesn't have to merge anymore
		if(player.IsMerging){
			foreach(Player p in currentlyMergedWith[index]){
				p.IsMerging = false;
			}
		}

		//Remove all partners
		currentlyMergedWith[index].RemoveRange(0, currentlyMergedWith[index].Count);

		//Remove anyone who has this ship as a partner
		for(int i = 0; i < players.Count; ++i){
			if(i == index) continue;
			currentlyMergedWith[i].Remove(player);
		}
		Rigidbody2D mergedRB = player.transform.parent.GetComponent<Rigidbody2D>();

		//no longer parented by the merged ship
		player.transform.parent = null;

		//Give the player its own rigidbody again
		Rigidbody2D newRB = player.gameObject.AddComponent<Rigidbody2D>();
		newRB.mass = 1;
		newRB.velocity = -(Vector2)player.transform.right + mergedRB.velocity;
		newRB.angularVelocity = mergedRB.angularVelocity;
		newRB.gravityScale = 0;

		//If there is only one ship remaining in the merged ship, we want
		//to unmerge that partner (mostly just to give it the rigidbody and unparent it
		//And then destroy the ship object
		//Note that this will not get called when Unmerge(mergePartner) is run,
		//as NumberOfMergedShips, at that point, will be 0
		if(mergedShipScript.NumberOfMergedShips == 1){
			Unmerge (mergePartner);
			foreach(GameObject go in mergedShipScript.lineList){
				Destroy (go);
			}
			mergedShipScript.lineList.RemoveRange(0, mergedShipScript.lineList.Count);
			Destroy(mergedShipScript.gameObject);
		}

		player.IsCurrentlyMerged = false;
	}

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class SelectionScreen : MonoBehaviour {
	struct PlayerSelection{
		public GameObject playerObj;
		public SpriteRenderer playerBody;
		public Color playerColor;
	}


	public GameObject playerSprite;
	List<PlayerSelection> players = new List<PlayerSelection>();

	public List<Color> possibleColorSelections;
	// Use this for initialization
	void Start () {

	}

	IEnumerator WaitTillEndOfFrame(){
		yield return new WaitForEndOfFrame();

		for(int i = 0; i < InputManager.Devices.Count; ++i){
			PlayerSelection temp = new PlayerSelection();
			GameObject tempObj = Instantiate(playerSprite) as GameObject;
			tempObj.transform.position = new Vector2(-5, 0) + new Vector2(2.5f * i, 0);
			
			temp.playerObj = tempObj;
			temp.playerBody = tempObj.transform.FindChild("body").GetComponent<SpriteRenderer>();
			
			players.Add (temp);
			print ("Trying to access playerColors");
			GameManager.S.playerColors.Add (Color.white);
		}
	}

	void Awake(){
		StartCoroutine(WaitTillEndOfFrame());
	}

	void Continue(){
		Application.LoadLevel("mainscene");
	}
	
	// Update is called once per frame
	void Update () {
		if(InputManager.ActiveDevice.MenuWasPressed){
			Continue ();
		}

		for(int i = 0; i < InputManager.Devices.Count; ++i){
			if (InputManager.Devices[i].DPadRight.WasPressed) {
				ChangeColor(i, 1);
			}
			if (InputManager.Devices[i].DPadLeft.WasPressed) {
				ChangeColor(i, -1);
			}
		}
	}

	void ChangeColor(int playerIndex, int dir){
		int currColorIndex = possibleColorSelections.IndexOf(players[playerIndex].playerBody.color);
		int newColorIndex = currColorIndex + dir;
		if(newColorIndex >= possibleColorSelections.Count) newColorIndex = 0;
		if(newColorIndex < 0) newColorIndex = possibleColorSelections.Count - 1;

		Color newColor = possibleColorSelections[newColorIndex];
		players[playerIndex].playerBody.color = newColor;

		GameManager.S.playerColors[playerIndex] = newColor;
	}
}

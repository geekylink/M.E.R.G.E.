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
	public GameObject colorWheelSprite;

	public float selectionTimeToSwitch;
	//float selectionTimer = 0;
	List<PlayerSelection> players = new List<PlayerSelection>();
	List<GameObject> colorWheels = new List<GameObject>();

	List<GameObject> selectionCircles = new List<GameObject>();
	public GameObject selectCirclePrefab;

	public List<Color> possibleColorSelections;
	public UnityEngine.UI.Text topText;
	List<bool> hasPushedA = new List<bool>();

	public List<UnityEngine.UI.Text> readyTexts = new List<UnityEngine.UI.Text>();
	// Use this for initialization
	void Start () {
		topText.text = "Press A to Confirm";
	}

	IEnumerator WaitTillEndOfFrame(){
		yield return new WaitForEndOfFrame();

		for(int i = 0; i < InputManager.Devices.Count; ++i){
			PlayerSelection temp = new PlayerSelection();
			GameObject tempObj = Instantiate(playerSprite) as GameObject;
			tempObj.transform.position = new Vector2(-7, 0) + new Vector2(5f * i, 0);
			
			temp.playerObj = tempObj;
			temp.playerBody = tempObj.transform.FindChild("body").GetComponent<SpriteRenderer>();
			
			players.Add (temp);
			GameManager.S.playerColors.Add (Color.white);

			GameObject cw = Instantiate(colorWheelSprite) as GameObject;
			cw.transform.position = tempObj.transform.position;
			colorWheels.Add (cw);

			GameObject sc= Instantiate(selectCirclePrefab) as GameObject;
			selectionCircles.Add (sc);
			sc.transform.position = cw.transform.position + Vector3.up * 2.08f;

			temp.playerBody.color = Color.red;
			GameManager.S.playerColors[i] = Color.red;

			bool tempBool = false;
			hasPushedA.Add (tempBool);

			Vector2 oldPos = readyTexts[i].rectTransform.anchoredPosition;
			oldPos.x = -2*Screen.width / 6 + Screen.width/6 * i;
			oldPos.y = Screen.height / 8;
			readyTexts[i].rectTransform.anchoredPosition = oldPos;
		}
	}

	void Awake(){
		StartCoroutine(WaitTillEndOfFrame());
	}

	void Continue(){
		Application.LoadLevel("dom-dev");
	}
	
	// Update is called once per frame
	void Update () {
		if(InputManager.ActiveDevice.MenuWasPressed){
			Continue ();
		}

		bool allHavePushedA = true;
		for(int i = 0; i < InputManager.Devices.Count; ++i){
			if(InputManager.Devices[i].Action1.WasReleased && i < hasPushedA.Count){
				hasPushedA[i] = !hasPushedA[i];
				if(hasPushedA[i]) readyTexts[i].text = "READY";
				else readyTexts[i].text = "";
			}

			if(i < hasPushedA.Count){
				if(!hasPushedA[i])
					allHavePushedA = false;
			}


			//move the selection thing
			if(selectionCircles.Count == 0) continue;
			float rightX = InputManager.Devices[i].RightStickX;
			float rightY = InputManager.Devices[i].RightStickY;
			
			float leftX = InputManager.Devices[i].LeftStickX;
			float leftY = InputManager.Devices[i].LeftStickY;

			float angleFloat = Mathf.Atan2 (rightY, rightX)*Mathf.Rad2Deg;
			if(angleFloat < 0) angleFloat += 360;/*
			if(angleFloat >= 30 && angleFloat < 150) angleFloat = 90;
			if(angleFloat < 30 || angleFloat >= 270) angleFloat = 330;
			if(angleFloat >= 150 && angleFloat < 270) angleFloat = 210;*/
			angleFloat = Mathf.Round(angleFloat / 30) * 30;

			Vector2 angleVec = new Vector2(rightX, rightY);
			if(angleVec.magnitude < .9f) continue;
			angleVec = (Vector2)(Quaternion.AngleAxis(angleFloat, Vector3.forward) * Vector2.right);

			Vector3 pos = colorWheels[i].transform.position + (Vector3)angleVec * 2.08f;
			selectionCircles[i].transform.position = pos;

			Vector3 dir = pos - colorWheels[i].transform.position;
			selectionCircles[i].transform.up = dir;

			Color color;
			float r, g, b;

			//super awesome maths to choose the color based on angle
			angleFloat -= 90;
			if(angleFloat > 180) angleFloat -= 360;
			r = 1 - (Mathf.Abs (angleFloat) / 120);
			if(r < 0) r = 0;

			angleFloat += 120;
			if(angleFloat > 180) angleFloat -= 360;
			g = 1 - (Mathf.Abs (angleFloat) / 120);
			if(g < 0) g = 0;

			angleFloat += 120;
			if(angleFloat > 180) angleFloat -= 360;
			b = 1 - (Mathf.Abs (angleFloat) / 120);
			if(b < 0) b = 0;


			color = new Color(r, g, b);
			color = color * 2;
			
			players[i].playerBody.color = color;
			GameManager.S.playerColors[i] = color;


		}

		if(allHavePushedA){
			topText.text = "Press Start to Continue";
			topText.color = Color.red;
		}
		else{
			topText.text = "Press A to Confirm";
			topText.color = Color.white;
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

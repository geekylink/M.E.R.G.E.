using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class GraphManager : MonoBehaviour {

	float startTime;
	float elapsedTime;

	public GameObject linePrefab;

	List<LineRenderer> renderList = new List<LineRenderer>();

	List<LineRenderer> graphBars = new List<LineRenderer>();

	public Text headerText;
	int graphToShow = 0;

	// Use this for initialization
	void Start () {
		startTime = GameManager.S.startTime;
		elapsedTime = GameManager.S.endTime - startTime;

		StartCoroutine (WaitForEndOfFrameToShow());

	}

	IEnumerator WaitForEndOfFrameToShow(){
		yield return new WaitForEndOfFrame();
		DrawGraphLines();
		ShowLevelGraph();
	}

	void DrawGraphLines(){
		GameObject temp = Instantiate(linePrefab) as GameObject;
		LineRenderer lr = temp.GetComponent<LineRenderer>();
		lr.SetVertexCount(2);
		lr.SetColors(Color.white, Color.white);
		Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0.1f));
		pos.z = 0;
		lr.SetPosition(0, pos);
		pos = Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0.9f));
		pos.z = 0;
		lr.SetPosition(1, pos);
		lr.SetWidth(0.1f, 0.1f);
		
		graphBars.Add (lr);
		
		temp = Instantiate(linePrefab) as GameObject;
		lr = temp.GetComponent<LineRenderer>();
		lr.SetVertexCount(2);
		lr.SetColors(Color.white, Color.white);
		pos = Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0.1f));
		pos.z = 0;
		lr.SetPosition(0, pos);
		pos = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.1f));
		pos.z = 0;
		lr.SetPosition(1, pos);
		lr.SetWidth(0.1f, 0.1f);
		
		graphBars.Add (lr);
		

	}

	void ShowLevelGraph(){
		headerText.text = "Player Levels";

		float highestLevel = 0;
		for(int i = 0; i < GameManager.S.playerTracker.Count; ++i){
			for(int j = 0; j < GameManager.S.playerTracker[i].Count; ++j){
				float level = GameManager.S.playerTracker[i][j].level;
				if(level > highestLevel){
					highestLevel = level;
				}
			}			
		}
		
		foreach(LineRenderer lr in renderList){
			Destroy(lr.gameObject);
		}
		
		renderList.RemoveRange(0, renderList.Count);


		for(int i = 0; i < GameManager.S.playerTracker.Count; ++i){
			GameObject tempObj = Instantiate(linePrefab) as GameObject;
			renderList.Add(tempObj.GetComponent<LineRenderer>());
			
			renderList[i].SetVertexCount(GameManager.S.playerTracker[i].Count + 1);
			renderList[i].SetColors(GameManager.S.playerColors[i], GameManager.S.playerColors[i]);
			renderList[i].SetWidth(0.1f, 0.1f);
			Vector3 pos = Vector3.zero;

			for(int j = 0; j < GameManager.S.playerTracker[i].Count; ++j){
				float time = GameManager.S.playerTracker[i][j].time - startTime;
				float level = GameManager.S.playerTracker[i][j].level;

				float x, y;
				x = time/elapsedTime * 0.8f + 0.1f;

				y = level / highestLevel * 0.8f + 0.1f;

				pos = Camera.main.ViewportToWorldPoint(new Vector3(x, y));
				pos.z = 0;
				renderList[i].SetPosition(j, pos);
			}

			renderList[i].SetPosition(GameManager.S.playerTracker[i].Count, pos);

		}
	}

	void ShowKillGraphs(){
		headerText.text = "Player Kills";

		float highestKills = 0;
		for(int i = 0; i < GameManager.S.playerTracker.Count; ++i){
			float kills = GameManager.S.playerTracker[i][GameManager.S.playerTracker[i].Count - 1].kills;
			if(kills > highestKills){
				highestKills = kills;
			}			
		}

		foreach(LineRenderer lr in renderList){
			Destroy(lr.gameObject);
		}
		renderList.RemoveRange(0, renderList.Count);
		
		
		for(int i = 0; i < GameManager.S.playerTracker.Count; ++i){
			GameObject tempObj = Instantiate(linePrefab) as GameObject;
			renderList.Add(tempObj.GetComponent<LineRenderer>());
			
			renderList[i].SetVertexCount(GameManager.S.playerTracker[i].Count + 1);
			renderList[i].SetColors(GameManager.S.playerColors[i], GameManager.S.playerColors[i]);
			renderList[i].SetWidth(0.1f, 0.1f);
			Vector3 pos = Vector3.zero;
			
			print (GameManager.S.playerTracker.Count + " " + GameManager.S.playerTracker[i].Count);
			
			for(int j = 0; j < GameManager.S.playerTracker[i].Count; ++j){
				float time = GameManager.S.playerTracker[i][j].time - startTime;
				float kills = GameManager.S.playerTracker[i][j].kills;
				
				float x, y;
				x = time/elapsedTime * 0.8f + 0.1f;
				
				y = kills / highestKills * 0.8f + 0.1f;
				
				pos = Camera.main.ViewportToWorldPoint(new Vector3(x, y));
				pos.z = 0;
				renderList[i].SetPosition(j, pos);
			}
			
			renderList[i].SetPosition(GameManager.S.playerTracker[i].Count, pos);
			
		}
	}

	void SwitchGraphs(int dir){
		graphToShow += dir;
		if(graphToShow < 0) graphToShow = 1;
		if(graphToShow > 1) graphToShow = 0;

		if(graphToShow == 0){
			ShowLevelGraph();
		}
		if(graphToShow == 1){
			ShowKillGraphs();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(InputManager.ActiveDevice.MenuWasPressed){
			Application.LoadLevel("WinScreen");
		}
		if(InputManager.ActiveDevice.RightBumper.WasPressed){
			SwitchGraphs(1);
		}
		if(InputManager.ActiveDevice.LeftBumper.WasPressed){
			SwitchGraphs(-1);
		}
	}
}

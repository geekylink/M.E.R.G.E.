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
	public Text winOrLoseText;
	int graphToShow = 2;

	// Use this for initialization
	void Start () {
		startTime = GameManager.S.startTime;
		elapsedTime = GameManager.S.endTime - startTime;
		ShowWinOrLose();

	}

	IEnumerator WaitForEndOfFrameToShow(){
		yield return new WaitForEndOfFrame();
	}

	void ShowWinOrLose(){
		headerText.text = "";


		foreach(LineRenderer lr in graphBars){
			Destroy(lr);
		}
		graphBars.RemoveRange (0, graphBars.Count);

		foreach(LineRenderer lr in renderList){
			Destroy(lr.gameObject);
		}
		
		renderList.RemoveRange(0, renderList.Count);


		if(GameManager.S.playersWin){
			winOrLoseText.text = "WIN";
			winOrLoseText.color = Color.blue;
		}
		else{
			winOrLoseText.text = "LOSE";
			winOrLoseText.color = Color.red;

		}

	}

	void DrawGraphLines(){
		winOrLoseText.text = "";
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
		DrawGraphLines();
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
			renderList[i].SetWidth(0.05f, 0.05f);
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

	void ShowKillGraph(){
		DrawGraphLines();
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
			renderList[i].SetWidth(0.05f, 0.05f);
			Vector3 pos = Vector3.zero;
			
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

	void ShowMergeGraph(){
		headerText.text = "Player Merges";

		foreach(LineRenderer lr in renderList){
			Destroy(lr.gameObject);
		}
		renderList.RemoveRange(0, renderList.Count);

		float mergePos = 0;
		for(int i = 0; i < GameManager.S.playerTracker.Count; ++i){
			GameObject tempObj = Instantiate(linePrefab) as GameObject;
			renderList.Add(tempObj.GetComponent<LineRenderer>());
			
			renderList[i].SetVertexCount(GameManager.S.playerTracker[i].Count + 1);
			renderList[i].SetColors(GameManager.S.playerColors[i], GameManager.S.playerColors[i]);
			renderList[i].SetWidth(0.01f, 0.01f);
			Vector3 pos = Vector3.zero;

			for(int j = 0; j < GameManager.S.playerTracker[i].Count; ++j){
				float time = GameManager.S.playerTracker[i][j].time - startTime;
				List<int> merge = GameManager.S.playerTracker[i][j].shipsMergedWith;
				
				float x, y;
				x = time/elapsedTime * 0.8f + 0.1f;
				y = 0;

				if(merge.Count == 0){
					y = 0.11f + 0.05f * i;
				}
				if(merge.Count == 1){
					mergePos = 0;
					if(i > merge[0]) mergePos = 0.05f;

					y = 0.35f + i*merge[0] / 12.0f + mergePos;
				}
				if(merge.Count == 2){
					mergePos = 0;
					if(i > merge[0] && i > merge[1]) mergePos = 0.05f;
					if(i < merge[0] && i < merge[1]) mergePos = -0.05f;

					y = 0.5f + mergePos;
				}
				if(merge.Count == 3){
					mergePos = 0;
					mergePos = i * 0.05f;

					y = 0.5f + mergePos;
				}
				
				pos = Camera.main.ViewportToWorldPoint(new Vector3(x, y));
				pos.z = 0;
				renderList[i].SetPosition(j, pos);
			}
			
			renderList[i].SetPosition(GameManager.S.playerTracker[i].Count, pos);
			
		}
	}

	void SwitchGraphs(int dir){
		graphToShow += dir;
		if(graphToShow < 0) graphToShow = 2;
		if(graphToShow > 2) graphToShow = 0;

		if(graphToShow == 0){
			ShowLevelGraph();
		}
		if(graphToShow == 1){
			ShowKillGraph();
		}
		if(graphToShow == 2){
			ShowWinOrLose();
		}
		if(graphToShow == 3){
			ShowMergeGraph();
		}
	}

	// Update is called once per frame
	void Update () {

		for(int i = 0; i < InputManager.Devices.Count; ++i){
			if(InputManager.Devices[i].MenuWasPressed){
				Application.LoadLevel("WinScreen");
			}
			if(InputManager.Devices[i].RightBumper.WasPressed){
				SwitchGraphs(1);
			}
			if(InputManager.Devices[i].LeftBumper.WasPressed){
				SwitchGraphs(-1);
			}
		}


	}
}

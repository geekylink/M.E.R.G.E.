using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UpgradeSystem : MonoBehaviour {
	public static UpgradeSystem S;
	public Canvas canvas;
	public Slider linearBarPrefab;
	List<Slider> linearBars = new List<Slider>();
	public Text levelTextPrefab;
	List<Text> levels = new List<Text>();
	List<int> levelNums = new List<int>();
	List<bool> isDead = new List<bool>();

	bool firstTimeSetup = false;
	// Use this for initialization
	void Start () {
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

	public void Respawn(int playerNum){
		isDead[playerNum] = false;
	}

	public void Die(int playerNum){
		isDead[playerNum] = true;
		linearBars[playerNum].value = linearBars[playerNum].minValue;
		levelNums[playerNum] = 1;
		levels[playerNum].text = "LEVEL: " + levelNums[playerNum];
	}



	void IncreaseFireRate(float amtToIncrease, int playerNum){
		PlayerManager.S.players[playerNum].GetComponent<Player>().FireRate -= amtToIncrease;
		if(PlayerManager.S.players[playerNum].GetComponent<Player>().FireRate <= 0.1f){
			PlayerManager.S.players[playerNum].GetComponent<Player>().FireRate = 0.1f;
		}
	}
	void IncreaseBulletSize(float amtToIncrease, int playerNum){
		PlayerManager.S.players[playerNum].GetComponent<Player>().BulletSize += amtToIncrease;
		if(PlayerManager.S.players[playerNum].GetComponent<Player>().BulletSize >= 2.5f){
			PlayerManager.S.players[playerNum].GetComponent<Player>().BulletSize = 2.5f;
		}
	}
	void IncreaseTurnSpeed(float amtToIncrease, int playerNum){
		PlayerManager.S.players[playerNum].GetComponent<Player>().TurnSpeed -= amtToIncrease;
		if(PlayerManager.S.players[playerNum].GetComponent<Player>().TurnSpeed <= 0.1f){
			PlayerManager.S.players[playerNum].GetComponent<Player>().TurnSpeed = 0.1f;
		}
	}
	void IncreaseFlySpeed(float amtToIncrease, int playerNum){
		PlayerManager.S.players[playerNum].GetComponent<Player>().VelocityMult += amtToIncrease;
		if(PlayerManager.S.players[playerNum].GetComponent<Player>().VelocityMult >= 2){
			PlayerManager.S.players[playerNum].GetComponent<Player>().VelocityMult = 2;
		}
	}



	void LevelUp(int playerNum){
		levelNums[playerNum]++;
		levels[playerNum].text = "LEVEL: " + levelNums[playerNum];
	}

	public void AddScore(float scoreToAdd, int playerNum){
		linearBars[playerNum].value += scoreToAdd;
		if(linearBars[playerNum].value >= linearBars[playerNum].maxValue){
			LevelUp(playerNum);
			linearBars[playerNum].value = linearBars[playerNum].minValue;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!firstTimeSetup){
			if(PlayerManager.S){
				if(PlayerManager.S.players != null){
					if(PlayerManager.S.players.Length > 0){
						firstTimeSetup = true;
						for(int i = 0; i < PlayerManager.S.players.Length; ++i){
							Slider temp = Instantiate(linearBarPrefab) as Slider;
							temp.transform.SetParent(canvas.transform, false);

							Vector2 oldPos = temp.GetComponent<RectTransform>().anchoredPosition;
							oldPos.x = -Screen.width / 2 + Screen.width / 8 + Screen.width/4 * i;
							oldPos.y = -Screen.height / 2 + Screen.height / 16;
							temp.GetComponent<RectTransform>().anchoredPosition = oldPos;

							
							Vector3 oldScale = temp.GetComponent<RectTransform>().localScale;
							oldScale *= 2;
							temp.GetComponent<RectTransform>().localScale = oldScale;

							temp.transform.FindChild("Fill Area").FindChild("Fill").gameObject.GetComponent<Image>().color = PlayerManager.S.playerColors[i];
							temp.value = 0;
							linearBars.Add (temp);

							bool tempBool = false;
							isDead.Add (tempBool);


							Text levelText = Instantiate(levelTextPrefab) as Text;
							levelText.transform.SetParent(canvas.transform, false);

							Vector2 textPos = levelText.rectTransform.anchoredPosition;
							textPos.x = -Screen.width / 2 + Screen.width / 8 + Screen.width/4 * i;
							textPos.y = -Screen.height / 2 + Screen.height / 32;
							levelText.rectTransform.anchoredPosition = textPos;

							levelText.text = "LEVEL: 1";
							levelText.color = PlayerManager.S.playerColors[i];
							levels.Add (levelText);
							levelNums.Add (1);
						}
					}
				}
			}
		}

		for(int i = 0; i < linearBars.Count; ++i){
			if(isDead[i]) continue;

			Slider slide = linearBars[i];
			slide.value += Time.deltaTime;
			if(slide.value >= slide.maxValue){
				LevelUp(i);
				slide.value = slide.minValue;
			}
		}

	}
}

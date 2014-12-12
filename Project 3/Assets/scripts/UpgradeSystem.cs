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
	public Text hitTextPrefab;
	List<Text> levelsNumbersText = new List<Text>();
	List<Text> levelsJustText = new List<Text>();
	List<int> levelNums = new List<int>();
	List<bool> isDead = new List<bool>();

	bool firstTimeSetup = false;

	public float showLevelTextTime;
	List<Text> upgradeTexts = new List<Text>();
	List<Text> gotHitTexts = new List<Text>();
	List<bool> showingHit = new List<bool>();

	public struct LevelValues{
		public float fireRate;
		public float bulletSize;
		public float turnSpeed;
		public float flySpeed;
		public float bulletSpeed;
		public float burst;
	}

	List<List<LevelValues>> levelValuesList = new List<List<LevelValues>>();
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
		levelsNumbersText[playerNum].text = "" + levelNums[playerNum];
	}

	IEnumerator GotHit(int pNum){
		
		Text hitText = Instantiate(hitTextPrefab) as Text;
		gotHitTexts[pNum] = hitText;
		hitText.transform.SetParent(canvas.transform, false);
		
		hitText.text = "-1";
		hitText.alignment = TextAnchor.MiddleRight;
		hitText.fontSize = 45;
		
		
		
		Vector2 textPos = levelsNumbersText[pNum].rectTransform.anchoredPosition;
		float startingX = textPos.x;
		float endingX = startingX + Screen.width / 32;
		
		//Color color = PlayerManager.S.players[pNum].GetComponent<Player>().playerColor;
		Color color = Color.red;
		float origAlpha = color.a;
		color.a = 0;

		
		float t = 0;
		while(t < 1){
			t += Time.deltaTime * Time.timeScale / 0.15f;
			
			textPos.x = Mathf.Lerp (startingX, endingX, t);
			hitText.rectTransform.anchoredPosition = textPos;
			
			color.a = Mathf.Lerp (0, origAlpha, t);
			hitText.color = color;

			yield return 0;
		}
		
		float timer = showLevelTextTime;
		
		if(timer + 0.3f > Mathf.Sqrt (levelNums[pNum])){
			timer = Mathf.Sqrt (levelNums[pNum]) - 0.5f;
		}
		t = 0;
		while(t < 1){
			t += Time.deltaTime * Time.timeScale / (timer / 2);
			yield return 0;
		}
		
		t = 0;
		while(t < 1){
			t += Time.deltaTime * Time.timeScale / 0.15f;
			
			//textPos.y = Mathf.Lerp (endingY, startingY, t);
			//upgradeText.rectTransform.anchoredPosition = textPos;

			color.a = Mathf.Lerp (origAlpha, 0, t);
			hitText.color = color;
			yield return 0;
		}
		Destroy(hitText.gameObject);
	}

	IEnumerator ShowLevelUpText(string levelUpText, int pNum){
		if(upgradeTexts[pNum] != null) Destroy (upgradeTexts[pNum].gameObject);

		Text upgradeText = Instantiate(levelTextPrefab) as Text;
		upgradeTexts[pNum] = upgradeText;
		upgradeText.transform.SetParent(canvas.transform, false);
		
		upgradeText.text = levelUpText;

		
		
		Vector2 textPos = levelsNumbersText[pNum].rectTransform.anchoredPosition;
		float startingY = textPos.y;
		float endingY = startingY + Screen.height / 32;

		Color color = PlayerManager.S.playerColors[pNum];
		float startingAlpha = 0;
		float endingAlpha = color.a;


		float t = 0;
		while(t < 1){
			t += Time.deltaTime * Time.timeScale / 0.15f;
			if(upgradeText != null){
				textPos.y = Mathf.Lerp (startingY, endingY, t);
				upgradeText.rectTransform.anchoredPosition = textPos;
				
				color.a = Mathf.Lerp (startingAlpha, endingAlpha, t);
				upgradeText.color = color;

			}
			yield return 0;
		}

		float timer = showLevelTextTime;

		if(timer + 0.3f > Mathf.Sqrt (levelNums[pNum])){
			timer = Mathf.Sqrt (levelNums[pNum]) - 0.5f;
		}
		t = 0;
		while(t < 1){
			t += Time.deltaTime * Time.timeScale / timer;
			yield return 0;
		}

		t = 0;
		while(t < 1){
			t += Time.deltaTime * Time.timeScale / 0.15f;
			
			//textPos.y = Mathf.Lerp (endingY, startingY, t);
			//upgradeText.rectTransform.anchoredPosition = textPos;
			if(upgradeText != null){
				color.a = Mathf.Lerp (endingAlpha, startingAlpha, t);
				upgradeText.color = color;

			}
			yield return 0;
		}
		if(upgradeText != null){
			Destroy(upgradeText.gameObject);

		}
		/*c.a = oldAlpha;
		levels[pNum].color = c;*/

	}


	void IncreaseFireRate(float amtToIncrease, int playerNum, int level){
		StartCoroutine(ShowLevelUpText("Fire Rate Up!", playerNum));
		PlayerManager.S.players[playerNum].GetComponent<Player>().FireRate -= amtToIncrease;
		if(PlayerManager.S.players[playerNum].GetComponent<Player>().FireRate <= 0.1f){
			print ("AT MAX");
			PlayerManager.S.players[playerNum].GetComponent<Player>().FireRate = 0.1f;
		}

		LevelValues lv = levelValuesList[playerNum][level - 2];
		lv.fireRate = PlayerManager.S.players[playerNum].GetComponent<Player>().FireRate;
		levelValuesList[playerNum].Add (lv);
	}
	void IncreaseBulletSize(float amtToIncrease, int playerNum, int level){
		StartCoroutine(ShowLevelUpText("Bullet Size Up!", playerNum));
		PlayerManager.S.players[playerNum].GetComponent<Player>().BulletSize += amtToIncrease;
		if(PlayerManager.S.players[playerNum].GetComponent<Player>().BulletSize >= 2.5f){
			print ("AT MAX");
			PlayerManager.S.players[playerNum].GetComponent<Player>().BulletSize = 2.5f;
		}
		
		LevelValues lv = levelValuesList[playerNum][level - 2];
		lv.bulletSize = PlayerManager.S.players[playerNum].GetComponent<Player>().BulletSize;
		levelValuesList[playerNum].Add (lv);
	}
	void IncreaseTurnSpeed(float amtToIncrease, int playerNum, int level){
		StartCoroutine(ShowLevelUpText("Turn Speed Up!", playerNum));
		PlayerManager.S.players[playerNum].GetComponent<Player>().TurnSpeed += amtToIncrease;
		if(PlayerManager.S.players[playerNum].GetComponent<Player>().TurnSpeed >= 30){
			print ("AT MAX");
			PlayerManager.S.players[playerNum].GetComponent<Player>().TurnSpeed = 30;
		}
		
		LevelValues lv = levelValuesList[playerNum][level - 2];
		lv.turnSpeed = PlayerManager.S.players[playerNum].GetComponent<Player>().TurnSpeed;
		levelValuesList[playerNum].Add (lv);
	}
	void IncreaseFlySpeed(float amtToIncrease, int playerNum, int level){
		StartCoroutine(ShowLevelUpText("Fly Speed Up!", playerNum));
		PlayerManager.S.players[playerNum].GetComponent<Player>().VelocityMult += amtToIncrease;
		if(PlayerManager.S.players[playerNum].GetComponent<Player>().VelocityMult >= 2){
			print ("AT MAX");
			PlayerManager.S.players[playerNum].GetComponent<Player>().VelocityMult = 2;
		}
		
		LevelValues lv = levelValuesList[playerNum][level - 2];
		lv.flySpeed = PlayerManager.S.players[playerNum].GetComponent<Player>().VelocityMult;
		levelValuesList[playerNum].Add (lv);
	}
	void IncreaseBulletSpeed(float amtToIncrease, int playerNum, int level){
		StartCoroutine(ShowLevelUpText("Bullet Speed Up!", playerNum));
		PlayerManager.S.players[playerNum].GetComponent<Player>().bulletVelocity += amtToIncrease;
		if(PlayerManager.S.players[playerNum].GetComponent<Player>().bulletVelocity >= 30){
			print ("AT MAX");
			PlayerManager.S.players[playerNum].GetComponent<Player>().bulletVelocity = 30;
		}
		
		LevelValues lv = levelValuesList[playerNum][level - 2];
		lv.bulletSpeed = PlayerManager.S.players[playerNum].GetComponent<Player>().bulletVelocity;
		levelValuesList[playerNum].Add (lv);
	}
	void IncreaseBurst(int playerNum, int level){
		StartCoroutine(ShowLevelUpText("Burst Fire Up!", playerNum));
		PlayerManager.S.players[playerNum].GetComponent<Player>().NumBurstFire += 1;
		if(PlayerManager.S.players[playerNum].GetComponent<Player>().NumBurstFire >= 5){
			print ("AT MAX");
			PlayerManager.S.players[playerNum].GetComponent<Player>().NumBurstFire = 5;
		}
		
		LevelValues lv = levelValuesList[playerNum][level - 2];
		lv.burst = PlayerManager.S.players[playerNum].GetComponent<Player>().NumBurstFire;
		levelValuesList[playerNum].Add (lv);
	}



	void LevelUp(int playerNum){
		levelNums[playerNum]++;
		GameManager.S.UpdateLevelTracking(levelNums[playerNum], playerNum);


		levelsNumbersText[playerNum].text = "" + levelNums[playerNum];
		levelsNumbersText[playerNum].fontSize = (int)(levelNums[playerNum]*2 + 20);

		linearBars[playerNum].maxValue = Mathf.Sqrt (levelNums[playerNum]);

		int ran = Random.Range (0, 21);
		if(ran <= 3){
			IncreaseFireRate(0.05f, playerNum, levelNums[playerNum]);
		}
		else if(ran <= 7){
			IncreaseBulletSize(0.1f, playerNum, levelNums[playerNum]);
		}
		else if(ran <= 11){
			IncreaseTurnSpeed(0.5f, playerNum, levelNums[playerNum]);
		}
		else if(ran <= 15){
			IncreaseFlySpeed(0.05f, playerNum, levelNums[playerNum]);
		}
		else if(ran <= 19){
			IncreaseBulletSpeed(0.5f, playerNum, levelNums[playerNum]);
		}
		else if(ran == 20){
			IncreaseBurst(playerNum, levelNums[playerNum]);
		}

	}

	public void LoseLevels(int playerNum, int levelsLost){
		if(upgradeTexts[playerNum] != null) Destroy(upgradeTexts[playerNum].gameObject);
		//if(gotHitTexts[playerNum] != null) Destroy(gotHitTexts[playerNum].gameObject);

		//StopAllCoroutines();
		levelsNumbersText[playerNum].fontSize = (int)(levelNums[playerNum]*1.5 + 20);

		StartCoroutine(GotHit(playerNum));
		levelNums[playerNum]-= levelsLost;
		if(levelNums[playerNum] < 1) levelNums[playerNum] = 1;

		levelsNumbersText[playerNum].text = "" + levelNums[playerNum];
		
		linearBars[playerNum].maxValue = Mathf.Sqrt (levelNums[playerNum]);
		linearBars[playerNum].value = linearBars[playerNum].minValue;


		LevelValues lv = levelValuesList[playerNum][levelNums[playerNum] - 1];
		levelValuesList[playerNum].RemoveRange(levelNums[playerNum], levelValuesList[playerNum].Count - levelNums[playerNum]);

		PlayerManager.S.players[playerNum].GetComponent<Player>().FireRate = lv.fireRate;
		PlayerManager.S.players[playerNum].GetComponent<Player>().BulletSize = lv.bulletSize;
		PlayerManager.S.players[playerNum].GetComponent<Player>().bulletVelocity = lv.bulletSpeed;
		PlayerManager.S.players[playerNum].GetComponent<Player>().NumBurstFire = lv.burst;
		PlayerManager.S.players[playerNum].GetComponent<Player>().TurnSpeed = lv.turnSpeed;
		PlayerManager.S.players[playerNum].GetComponent<Player>().VelocityMult = lv.flySpeed;
	}

	public void AddScore(float scoreToAdd, int playerNum){
		linearBars[playerNum].value += scoreToAdd;
		if(linearBars[playerNum].value >= linearBars[playerNum].maxValue){
			LevelUp(playerNum);
			linearBars[playerNum].value = linearBars[playerNum].minValue;
		}
	}

	void FirstTimeSetup(){
		for(int i = 0; i < PlayerManager.S.players.Length; ++i){
			Slider temp = Instantiate(linearBarPrefab) as Slider;
			temp.transform.SetParent(canvas.transform, false);
			
			Vector2 oldPos = temp.GetComponent<RectTransform>().anchoredPosition;
			oldPos.x = -Screen.width / 2 + Screen.width / 8 + Screen.width/4 * i;
			oldPos.y = -Screen.height / 2 + Screen.height / 8;
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

			textPos.y = -Screen.height / 2 + 5*Screen.height / 32;
			levelText.rectTransform.anchoredPosition = textPos;
			
			levelText.text = "LEVEL: ";
			levelText.alignment = TextAnchor.MiddleLeft;
			levelText.color = PlayerManager.S.playerColors[i];
			levelsJustText.Add (levelText);


			Text levelTextNum = Instantiate(levelTextPrefab) as Text;
			levelTextNum.transform.SetParent(canvas.transform, false);
			
			textPos = levelTextNum.rectTransform.anchoredPosition;
			textPos.x = -Screen.width / 2 + Screen.width / 8 + Screen.width/4 * i;
			
			textPos.y = -Screen.height / 2 + 5*Screen.height / 32;
			levelTextNum.rectTransform.anchoredPosition = textPos;
			levelTextNum.text = "1";
			levelTextNum.alignment = TextAnchor.MiddleRight;
			levelTextNum.color = PlayerManager.S.playerColors[i];
			levelTextNum.fontSize = (int)(22);
			levelsNumbersText.Add (levelTextNum);

			levelNums.Add (1);
			linearBars[i].maxValue = Mathf.Sqrt (levelNums[i]);
			
			LevelValues lv;
			
			Player p = PlayerManager.S.players[i].GetComponent<Player>();
			
			lv.fireRate = p.FireRate;
			lv.bulletSize = p.BulletSize;
			lv.bulletSpeed = p.bulletVelocity;
			lv.burst = p.NumBurstFire;
			lv.flySpeed = p.VelocityMult;
			lv.turnSpeed = p.TurnSpeed;
			
			List<LevelValues> newList = new List<LevelValues>();
			newList.Add (lv);
			levelValuesList.Add (newList);
			
			upgradeTexts.Add (null);
			gotHitTexts.Add (null);
			showingHit.Add (false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!firstTimeSetup){
			if(PlayerManager.S){
				if(PlayerManager.S.players != null){
					if(PlayerManager.S.players.Length > 0){
						firstTimeSetup = true;
						FirstTimeSetup();
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

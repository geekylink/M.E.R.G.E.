using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class PlayerManager : MonoBehaviour {
	public static PlayerManager S;
	public GameObject[] players;

	public float playerSpeed = 30;

	public GameObject playerPrefab;

	public float respawnTime;
	public float respawnInvunerablityTime;

	public List<Color> playerColors;
	public List<UnityEngine.UI.Text> playerResourceTexts;

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

	void InitialSpawn(){

		players = new GameObject[InputManager.Devices.Count];

		for(int i = 0; i < InputManager.Devices.Count; ++i){
			SpawnPlayer(i, playerColors[i], i, false);
		}


	}

	IEnumerator SpawnAtEndOfFrame(){
		yield return new WaitForEndOfFrame();
		InitialSpawn();
	}

	void Awake(){
		StartCoroutine(SpawnAtEndOfFrame());
	}

	// Update is called once per frame
	void FixedUpdate () {
		// Cycle through the controllers and players
		for (int i = 0; i < InputManager.Devices.Count; i++) {
			if (i < players.Length) {
				if(players[i]){
					Player p = players[i].GetComponent ("Player") as Player;
					UpdatePlayer (InputManager.Devices[i], p);
				}
			}
		}
	}

	public GameObject[] getPlayers() {
		return players;
	}

	private void UpdatePlayer(InputDevice device, Player player) {
		float leftX = device.LeftStickX;
		float leftY = device.LeftStickY;
		float rightX = device.RightStickX;
		float rightY = device.RightStickY;
		//Player p = player.GetComponent ("Player") as Player;

		float leftAngle = Mathf.Atan2 (leftY, leftX)*Mathf.Rad2Deg;
		float rightAngle = Mathf.Atan2 (rightY, rightX)*Mathf.Rad2Deg;

		if(Mathf.Abs (leftY) < 0.3f && Mathf.Abs(leftX) < 0.3f){
			leftX = leftY = 0;
			leftAngle = 0;
		}
		if(Mathf.Abs (rightY) < 0.3f && Mathf.Abs(rightX) < 0.3f){
			rightAngle = 0;
		}

		if (device.DPadDown.WasPressed) {
			player.SpawnTurret(BaseSatellite.SatelliteType.TURRET);
		}

		if (device.DPadUp.WasPressed) {
			player.SpawnTurret(BaseSatellite.SatelliteType.HEALER);
		}

		if (device.DPadRight.WasPressed) {
			player.SpawnTurret(BaseSatellite.SatelliteType.MINER);
		}

		// Updates angles

		//if (device.LeftTrigger)		player.FireLeftTurret(); 
		//if (device.RightTrigger)	player.FireRightTurret();

		//Check if A button is pushed (currently for merge/unmerge purposes
		//Comment out this line if testing with keyboard
		player.CheckMerge (device.Action1, device.Action2);



		#region Aim with sticks, fly with bumpers
		/*player.UpdateTurrets(leftAngle, rightAngle);
		player.FireEngines (device.LeftBumper, device.RightBumper);*/
		#endregion


		#region Turn left/right with stick, fly forward with bumper
		/*player.UpdateTurrets (0, rightAngle);
		player.Turn(leftX, -10);
		
		if(device.LeftBumper){
			player.FlyForward(0.2f);
		}
		else{
			player.useBreaks();
		}*/
		#endregion

		#region Aim in desired flying direction with left stick, fly forward with bumper
		/*player.UpdateTurrets (0, rightAngle);
		player.TurnTowards(leftAngle);
		if(device.LeftBumper){
			player.FlyForward(0.2f);
		}
		else{
			player.useBreaks();
		}*/
		#endregion

		#region Fly in the direction of the left stick
		player.UpdateTurrets (rightAngle);
		player.TurnTowards(leftAngle);
		Vector3 rot = Vector3.zero;
		rot.z = leftAngle;
		float speed = Mathf.Sqrt(leftX * leftX + leftY * leftY);
		player.ApplyFly(speed, playerSpeed);
		/*if(speed == 0){
			player.useBreaks();
		}*/
		#endregion


		//if (device.Action3)	player.useBreaks ();

		//

	}

	void SpawnPlayer(int arrayPos, Color playerColor, int mergeIndex, bool isInvulnerable){
		GameObject playerGO = Instantiate(playerPrefab) as GameObject;

		Vector2 pos = camera.transform.position;
		if(arrayPos == 0){
			pos.x += 1.5f;
			pos.y += 1.5f;
		}
		if(arrayPos == 1){
			pos.x += -1.5f;
			pos.y += 1.5f;
		}
		if(arrayPos == 2){
			pos.x += -1.5f;
			pos.y += -1.5f;
		}
		if(arrayPos == 3){
			pos.x += 1.5f;
			pos.y += -1.5f;
		}

		playerGO.transform.position = pos;
		
		Player pScript = playerGO.GetComponent<Player>();
		pScript.gtRes = playerResourceTexts[arrayPos];

		pScript.ChangeColor(playerColor);
		pScript.playerManagerArrayPos = arrayPos;
		
		players[arrayPos] = playerGO;

		MergeManager.S.AddPlayer(pScript, mergeIndex);

		if(isInvulnerable){
			StartCoroutine(RespawnInvulnerability(playerGO));
		}

	}

	IEnumerator RespawnInvulnerability(GameObject player){
		Player pScript = player.GetComponent<Player>();
		pScript.isInvulnerable = true;
		player.collider2D.enabled = false;

		float t = 0;

		float flashingSprintTimer = 0;

		while(t < 1){
			t += Time.deltaTime * Time.timeScale / respawnInvunerablityTime;

			flashingSprintTimer += 0.1f;
			if(flashingSprintTimer > 1){
				flashingSprintTimer = 0;
				pScript.body.GetComponent<SpriteRenderer>().enabled = !pScript.body.GetComponent<SpriteRenderer>().enabled;
			}

			yield return 0;
		}

		
		player.collider2D.enabled = true;
		pScript.body.GetComponent<SpriteRenderer>().enabled = true;
		pScript.isInvulnerable = false;
	}

	IEnumerator Respawn(int arrayPos, Color playerColor, int mergeIndex){
		float t = 0;
		while(t < 1){
			t += Time.deltaTime * Time.timeScale / respawnTime;
			yield return 0;
		}

		SpawnPlayer(arrayPos, playerColor, mergeIndex, true);

	}


	public void PlayerDied(Player player, int posInArray){
		Color playerColor = player.playerColor;
		if(posInArray == -1){
			Debug.LogError("Error, cannot find player to respawn");
		}

		int mergeIndex = MergeManager.S.players.IndexOf (player);

		StartCoroutine(Respawn(posInArray, playerColor, mergeIndex));

	}
}

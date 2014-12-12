using UnityEngine;
using System.Collections;
using InControl;

public class EndScreen : MonoBehaviour {


	void Start(){
		if(GameManager.S){
			Destroy(GameManager.S.gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
		if(InputManager.ActiveDevice.MenuWasPressed){
			Application.LoadLevel("SplashScreen");
		}

		if(InputManager.ActiveDevice.Action1){
			Application.LoadLevel("Credits");
		}
	}
}

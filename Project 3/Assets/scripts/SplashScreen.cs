using UnityEngine;
using System.Collections;
using InControl;

public class SplashScreen : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if(InputManager.ActiveDevice.Action1){
			Application.LoadLevel("dom-dev-temp");
		}
	}
}

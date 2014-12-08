using UnityEngine;
using System.Collections;
using InControl;

public class ControlsScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(InputManager.ActiveDevice.Action1){
			Application.LoadLevel("PlayerSelectionScreen");
		}
	}
}

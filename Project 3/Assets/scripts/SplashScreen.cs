using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class SplashScreen : MonoBehaviour {
	public List<GameObject> thingsToDestroy;
	public List<GameObject> thingsToEnable;
	public List<GameObject> sprites = new List<GameObject>();
	int currSprite = 0;
	bool alreadySelection = false;

	// Update is called once per frame
	void Update () {
		if(InputManager.ActiveDevice.Action1.WasPressed){
			if(currSprite <= sprites.Count - 1){
				sprites[currSprite].GetComponent<SpriteRenderer>().enabled = true;
				if(currSprite > 0){
					sprites[currSprite - 1].GetComponent<SpriteRenderer>().enabled = false;
				}
				else{
					foreach(GameObject go in thingsToDestroy){
						Destroy(go);
					}
				}
				currSprite++;
			}
			else{
				if(alreadySelection) return;
				sprites[currSprite - 1].GetComponent<SpriteRenderer>().enabled = false;
				foreach(GameObject go in thingsToEnable){
					go.GetComponent<UnityEngine.UI.Text>().enabled = true;
				}
				SelectionScreen.S.StartSelection();
				alreadySelection = true;

			}
			//Application.LoadLevel("InfoScreen");
		}
	}
}

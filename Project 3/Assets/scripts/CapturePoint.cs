using UnityEngine;
using System.Collections;

public class CapturePoint : MonoBehaviour {

	public SpriteRenderer sr;
	public Sprite enemySprite;
	public Sprite playerSprite;
	bool playerControlled = false;
	bool allDestroyed = false;
	SubCapturePoint[] subPoints;

	// Use this for initialization
	void Start () {
		sr = this.GetComponent<SpriteRenderer> ();
		if (playerControlled) {
			sr.sprite = playerSprite;
		} else {
			sr.sprite = enemySprite;
		}
	}

	void Awake(){
		subPoints = GetComponentsInChildren<SubCapturePoint> ();
	}
	
	// Update is called once per frame
	void Update () {
		allDestroyed = true;
		foreach (SubCapturePoint point in subPoints) {
			if (point.sr.sprite != point.destroyedSprite){
				allDestroyed = false;
				break;
			}
		}
		if (allDestroyed) {
			if(playerControlled){
				playerControlled = false;
				sr.sprite = enemySprite;
			} else {
				playerControlled = true;
				sr.sprite = playerSprite;
			}
			foreach (SubCapturePoint point in subPoints){
				point.Reset();
			}
		}
	}
}

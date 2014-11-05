using UnityEngine;
using System.Collections;

public class ViewportManager : MonoBehaviour {

	public Camera P1Cam;
	public Camera P2Cam;
	public Camera P3Cam;
	public Camera P4Cam;
	
	// Use this for initialization
	void Start () {
	
	}

	void Awake(){
		GameObject[] cameras = GameObject.FindGameObjectsWithTag ("Camera");
		foreach (GameObject cam in cameras) {

			if(cam.camera.rect.x == 0 && cam.camera.rect.y == 0.5){
				P1Cam = cam.camera;
			} else if(cam.camera.rect.x == 0.5 && cam.camera.rect.y == 0.5){
				P2Cam = cam.camera;
			} else if(cam.camera.rect.x == 0 && cam.camera.rect.y == 0){
				P3Cam = cam.camera;
			} else if(cam.camera.rect.x == 0.5 && cam.camera.rect.y == 0){
				P4Cam = cam.camera;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("1")) {
			Rect temp = new Rect (0,0, 0, 0);
			P2Cam.rect = temp;
			temp.width = 1;
			temp.height = 0.5f;
			P1Cam.rect = temp;
		}
		if (Input.GetKeyDown ("2")) {
			Rect temp = new Rect (0,0, 0, 0);
			P3Cam.rect = temp;
			temp.width = 0.5f;
			temp.height = 1;
			P1Cam.rect = temp;
		}
	}
}

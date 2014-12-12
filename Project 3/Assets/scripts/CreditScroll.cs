using UnityEngine;
using System.Collections;

public class CreditScroll : MonoBehaviour {

	public float scrollSpeed = 50f;
	public float stopHeight = -1000f;
	bool atEnd = false;
	public float waitTime = 0f;
	float timePassed = 0f;

	// Use this for initialization
	void Start () {
				
	}
	
	// Update is called once per frame
	void Update () {
		if(timePassed > waitTime && atEnd == false){
			Vector3 pos = Camera.main.transform.position + Vector3.down * scrollSpeed * Time.deltaTime;
			if(pos.y < stopHeight){
				pos.y = stopHeight;
				atEnd = true;
			}
			Camera.main.transform.position = pos;
		}
		timePassed += Time.deltaTime;
	}
}

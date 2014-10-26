using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{

	public float xBound = 7.5f;
	public float yTopBound = 5f;
	public float yLowBound = -4f;
	public Vector3 initTransform;
	Vector3 pos;
	Vector3 addVec;

	// Use this for initialization
	void Start ()
	{
		//transform.position = initTransform;
		pos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown ("l")) {
			Application.LoadLevel(Application.loadedLevelName);
		}
		if (Input.GetKeyDown ("down")) {
			if (transform.position.y > yLowBound) {
				pos = transform.position;
				pos.y -= 1f;
			}
		} else if (Input.GetKeyDown ("up")) {
			if (transform.position.y < yTopBound) {
				pos = transform.position;
				pos.y += 1f;
			}
		} else if (Input.GetKeyDown ("left")) {
			if (transform.position.x > -xBound) {
				pos = transform.position;
				pos.x -= 1f;
			}
		} else if (Input.GetKeyDown ("right")) {
			if (transform.position.x < xBound) {
				pos = transform.position;
				pos.x += 1f;
			}
		}
		transform.position = pos;
	}
}

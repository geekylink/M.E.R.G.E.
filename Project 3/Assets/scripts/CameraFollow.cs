using UnityEngine;
using System.Collections;

//Slightly adapted from amaranth's code at
//http://answers.unity3d.com/questions/29183/2d-camera-smooth-follow.html

public class CameraFollow : MonoBehaviour {
		
	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public GameObject target;

	void Awake(){
		target = GameObject.FindGameObjectWithTag ("Target");
	}

	// Update is called once per frame
	void Update () 
	{
		if (target.transform)
		{
			Vector3 point = camera.WorldToViewportPoint(target.transform.position);
			Vector3 delta = target.transform.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}

	}
}
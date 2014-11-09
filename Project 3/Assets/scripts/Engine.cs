using UnityEngine;
using System.Collections;

public class Engine : MonoBehaviour {

	public float force = 1;
	public bool shouldForceOnTurn = true;
	bool on = false;

	// Use this for initialization
	void Start () {
	
	}

	void Update(){
		if(on){
			transform.root.rigidbody2D.AddForceAtPosition(-transform.forward * force, transform.position);
		}
	}

	public void TurnOn(bool turning = false){
		if(turning && shouldForceOnTurn){
			on = true;
		}
		if(!turning){
			on = true;
		}
		if(particleSystem != null){
			particleSystem.enableEmission = true;
		}
	}

	public void TurnOff(){
		on = false;
		if(particleSystem != null){
			particleSystem.enableEmission = false;
		}
	}
}

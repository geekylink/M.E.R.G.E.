using UnityEngine;
using System.Collections;

public class StarPause : MonoBehaviour {


    float warmup = 5;
	// Use this for initialization
	void Start () {
        //for(float s = 0; s < 5; s+=0.1f)
        //    this.GetComponent<ParticleSystem>().Simulate(0.1f);
	}
	
	// Update is called once per frame
	void Update () {
        if (warmup > 0)
            warmup -= Time.deltaTime;
        else
        { 
            this.GetComponent<ParticleSystem>().Pause();
        }
	}
}

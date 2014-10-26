using UnityEngine;
using System.Collections;

public class ParticleDestroyer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Invoke("Destroy", this.GetComponent<ParticleSystem>().duration);
	}
	
    void Destroy()
    {
        GameObject.Destroy(this.gameObject);
    }
	// Update is called once per frame
	void Update () {
	
	}
}

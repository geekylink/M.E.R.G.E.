using UnityEngine;
using System.Collections;

public class ParticleLayers : MonoBehaviour {

    public string sortingLayer = "Background";
    public int sortingOrder = 1;
	// Use this for initialization
	void Start () {
        particleSystem.renderer.sortingLayerName = sortingLayer;
        particleSystem.renderer.sortingOrder = sortingOrder;
	}
	
	
}

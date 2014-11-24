using UnityEngine;
using System.Collections;
using System;

public class ParticleLayers : MonoBehaviour {

    public string sortingLayer = "Background";
    public int sortingOrder = 1;
	// Use this for initialization
	void Start () {
        try
        {
            particleSystem.renderer.sortingLayerName = sortingLayer;
            particleSystem.renderer.sortingOrder = sortingOrder;
        }
        catch(Exception e)
        {
            this.renderer.sortingLayerName = sortingLayer;
            this.renderer.sortingOrder = sortingOrder;
        }
	}
	
	
}

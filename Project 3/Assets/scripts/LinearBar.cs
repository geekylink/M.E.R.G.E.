using UnityEngine;
using System.Collections;

public class LinearBar : MonoBehaviour {

    public float percent;
    public Vector3 offset;
    public GameObject bar;
	void Start () {

        Transform sp = bar.transform;
        Vector3 scale = sp.localScale;
        scale.x *= percent;
        sp.localScale = scale;

	}
	
	// Update is called once per frame
	void Update () {
	
        
	}

    public void SetPercent(float percent)
    {
        this.percent = percent;
    }

    void OnDrawGizmosSelected ()
    {
        //Gizmos.DrawLine(transform.position + offset, transform.position + offset + new Vector3(width, 0));
    }
}

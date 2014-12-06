using UnityEngine;
using System.Collections;

public class PlanetOrbitLine : MonoBehaviour {

	public int segments;
	float radius;
	LineRenderer line;
	
	void Awake ()
	{
		line = gameObject.GetComponent<LineRenderer>();
		line.sortingLayerID = 1;
		line.sortingOrder = -10;

		Color color = gameObject.GetComponent<CapturePoint>().color;
		color.a = .3f;
		line.SetColors(color, color);

		radius = gameObject.GetComponent<BaseSatellite>().orbitRadius;
		line.SetVertexCount (segments + 1);
		line.useWorldSpace = true;
		CreatePoints ();
	}
	
	
	void CreatePoints ()
	{
		float x;
		float y;
		float z = 30f;
		
		float angle = 20f;
		
		for (int i = 0; i < (segments + 1); i++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * angle) * radius;
			y = Mathf.Cos (Mathf.Deg2Rad * angle) * radius;
			
			line.SetPosition (i,new Vector3(x,y,z) );
			
			angle += (360f / segments);
		}
	}

	public void UpdateOrbitLine(){
		radius = gameObject.GetComponent<BaseSatellite>().orbitRadius;
		
		Color color = gameObject.GetComponent<CapturePoint>().color;
		color.a = .3f;
		line.SetColors(color, color);
		CreatePoints();
	}
}

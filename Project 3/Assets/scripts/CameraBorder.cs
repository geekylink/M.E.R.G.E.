using UnityEngine;
using System.Collections;

public class CameraBorder : MonoBehaviour {

	public GUIStyle myStyle;

	void OnGUI(){
		//myStyle.normal.background.alphaIsTransparency = true;
		myStyle.normal.textColor = Color.white;
		myStyle.alignment = TextAnchor.MiddleCenter;


		/*top left point of rectangle
		Vector3 boxPosTopLeft = new Vector3(this.camera.rect.xMin, this.camera.rect.yMax, 0);
		//bottom right point of rectangle
		Vector3 boxPosBottomRight = new Vector3(this.camera.rect.xMax, this.camera.rect.yMin, 0);
		
		Vector3 boxPosHiLeftCamera = Camera.main.WorldToScreenPoint(boxPosTopLeft);
		Vector3 boxPosLowRightCamera = Camera.main.WorldToScreenPoint(boxPosBottomRight);
		
		float width = boxPosHiLeftCamera.x - boxPosLowRightCamera.x;
		float height = boxPosHiLeftCamera.y - boxPosLowRightCamera.y;*/

		GUI.Box(new Rect(this.gameObject.camera.rect.xMax, this.gameObject.camera.rect.yMin, this.camera.rect.width, this.camera.rect.height),"BOX GOES HERE", myStyle);

	}
}

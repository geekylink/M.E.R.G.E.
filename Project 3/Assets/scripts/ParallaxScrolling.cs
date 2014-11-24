using UnityEngine;
using System.Collections;

public class ParallaxScrolling : MonoBehaviour {

    public float scrollSpeed;



    void Update()
    {
        //float sizeScalar = (Camera.main.orthographicSize - 20f) / 20f;
        float sizeScalar = 1f;
        float y = Mathf.Repeat(Camera.main.transform.position.y * sizeScalar * scrollSpeed, 1f);
        float x = Mathf.Repeat(Camera.main.transform.position.x * sizeScalar * scrollSpeed, 1f);
        Vector2 offset = new Vector2(x, y);
        renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }


}

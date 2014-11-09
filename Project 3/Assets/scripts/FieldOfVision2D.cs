using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class FieldOfVision2D : MonoBehaviour
{
	public float fieldAngle;
	public float fieldDepth;
	public float nearLimit;
	public bool showInScene = true;
	public bool showNearLimit = true;
	public LayerMask layersToDetect = Physics2D.DefaultRaycastLayers;
	public bool panning = false;
	public float panTime;
	public float degreesToPan;
	public float pauseTime;
	public Vector2 panCenter;
	bool clockwise = false;
	float cwLimit;
	float ccwLimit;
	float degrees = 0f;
	float timePaused = 0f;
	bool paused;
	Vector3 initDir;
	Vector3 dir = new Vector3(0f,1f,0f);

	void Start () {
		initDir = new Vector3 (panCenter.x, panCenter.y, 0);
		if (panning) {
			dir = initDir;
		}
		cwLimit = Vec2Deg (initDir) - degreesToPan / 2;
		ccwLimit = Vec2Deg (initDir) + degreesToPan / 2;
		degrees = ccwLimit;
	}
	
	void FixedUpdate(){
		float degreesPerUpdate = degreesToPan / panTime * Time.fixedDeltaTime;
		if (panning) {
			if (paused) {
				timePaused += Time.fixedDeltaTime;
				if (timePaused >= pauseTime) {
					timePaused = 0f;
					paused = false;
				}
			} 
			if(!paused) {
				if (clockwise) {
					degrees -= degreesPerUpdate;
					if (degrees < cwLimit) {
						degrees = cwLimit;
						clockwise = false;
						if (pauseTime > 0) {
							paused = true;
						}
					}
					dir = Deg2Vec (degrees);			       
				} else {
					degrees += degreesPerUpdate;
					if (degrees > ccwLimit) {
						degrees = ccwLimit;
						clockwise = true;
						if (pauseTime > 0) {
							paused = true;
						}
					}
					dir = Deg2Vec (degrees);			       
				}
			}
		}
	}
	
	public List<Collider2D> DetectAllWithTag (Vector3 facing, string[] tags){
		if (panning) {
			facing = dir;
		}
		dir = facing;
		float initAngle = Vec2Deg (facing);
		RaycastHit2D [] hits;
		List<Collider2D> collisions = new List<Collider2D>();
		for (float i = initAngle - fieldAngle/2; i < initAngle + fieldAngle/2; i += 1) {
			hits = Physics2D.RaycastAll (transform.position, Deg2Vec (i), fieldDepth);
			foreach(string tag in tags){
				foreach(RaycastHit2D hit in hits){
					if (hit.collider.tag == tag && hit.distance >= nearLimit) {
						collisions.Add (hit.collider);
					}
				}
			}
		}
		return collisions;
	}
	
	private Collider2D CheckByTags(Vector3 facing, string[] tags, bool checkAny = false){
		if (panning) {
			facing = dir;
		}
		dir = facing;
		float initAngle = Vec2Deg (facing);
		RaycastHit2D hit;
		for (float i = initAngle - fieldAngle/2; i < initAngle + fieldAngle/2; i += 1) {
			hit = Physics2D.Raycast (transform.position, Deg2Vec (i), fieldDepth);
			foreach( string tag in tags){
				if (hit.collider != null && hit.collider.tag == tag && hit.distance >= nearLimit) {
					return hit.collider;
				}
			}
		}
		return null;
	}
	
	public Collider2D DetectFirstWithTag(Vector3 facing, string[] tags){
		return CheckByTags (facing, tags);
	}

	public bool DetectAnyWithTag (Vector3 facing, string[] tags){
		if (CheckByTags (facing, tags, true) == null) {
			return false;
		}
		return true;
	}

	public List<Collider2D> DetectAllByLayer(Vector3 facing){
		if (panning) {
			facing = dir;
		}
		dir = facing;
		float initAngle = Vec2Deg (facing);
		List<Collider2D> collisions = new List<Collider2D>();
		RaycastHit2D [] hits;
		for (float i = initAngle - fieldAngle/2; i < initAngle + fieldAngle/2; i += 1) {
			hits = Physics2D.RaycastAll (transform.position, Deg2Vec (i), fieldDepth, layersToDetect);
			foreach (RaycastHit2D hit in hits) {
				if (hit.collider != null && hit.distance >= nearLimit) {
					collisions.Add(hit.collider);
				}
			}
		}
		return collisions;
	}
		
	private Collider2D CheckByLayers (Vector3 facing, bool checkAny = false)
	{
		if (panning) {
			facing = dir;
		}
		dir = facing;
		float initAngle = Vec2Deg (facing);
		RaycastHit2D hit;
		for (float i = initAngle - fieldAngle/2; i < initAngle + fieldAngle/2; i += 1) {
			hit = Physics2D.Raycast (transform.position, Deg2Vec (i), fieldDepth, layersToDetect);
			if (hit.collider != null && hit.distance >= nearLimit) {
				return hit.collider;
			}
		}
		return null;
	}

	public Collider2D DetectFirstByLayer(Vector3 facing){
		return CheckByLayers (facing);
	}

	public bool DetectAnyByLayer (Vector3 facing){
		if (CheckByLayers (facing, true) == null) {
			return false;
		}
		return true;
	}

	void OnDrawGizmos(){
		float initAngle = Vec2Deg (dir);
		if (showInScene) {
			Handles.color = Color.yellow;
			Handles.DrawSolidArc (transform.position, new Vector3 (0, 0, 1), Deg2Vec (initAngle - fieldAngle / 2), fieldAngle, fieldDepth);
		}
		if(showNearLimit){
			Handles.color = Color.red;
			Handles.DrawSolidArc (transform.position, new Vector3 (0, 0, 1), Deg2Vec (initAngle - fieldAngle / 2), fieldAngle, nearLimit);
		}

	}

	Vector3 Deg2Vec (float degree)
	{
		float rad = degree * Mathf.Deg2Rad;
		Vector3 vec = new Vector3 (Mathf.Cos (rad), Mathf.Sin (rad), 0);
		return vec;
	}

	float Vec2Deg (Vector3 vec)
	{
		Vector3 norm = vec.normalized;
		float result;
		if (norm.x >= 0 && norm.y >= 0) {
			result = Mathf.Asin (norm.y);
		} else if (norm.x <= 0 && norm.y >= 0) {
			result = Mathf.Acos (norm.x);
		} else if (norm.x <= 0 && norm.y >= 0) {
			result = Mathf.Acos (norm.x);
			result += 2f * Mathf.Abs(Mathf.PI - result);
		} else {
			result = Mathf.Acos (norm.x);
			result = Mathf.PI * 2f - result;
		}

	if (result > 2f * Mathf.PI) {
			return Mathf.Rad2Deg * result - 360;
		} else if (result < 0) {
			return Mathf.Rad2Deg * result + 360;
		} else {
			return Mathf.Rad2Deg * result;
		}
	}
}
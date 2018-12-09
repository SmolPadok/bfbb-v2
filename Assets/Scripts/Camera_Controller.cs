using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Camera_Controller : MonoBehaviour {

    public Camera cam;
	public List<Transform> targets;
	public Vector3 offset;
	public float smoothTime, maxZoom, minZoom, zoomLimiter;

	private Vector3 velocity;

	void Start(){

	}

	void FixedUpdate(){

		if (targets.Count == 0) {
			return;
		}
		Move ();
		Zoom ();
	
	}
	void Zoom(){

		float newZoom = Mathf.Lerp (maxZoom, minZoom, GetGreatestDistance ()/ zoomLimiter);
		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
	}

	void Move(){

		Vector3 centerPoint = GetCenterPoint();
		Vector3 newPosition = centerPoint + offset;
		transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);

	}

	float GetGreatestDistance(){

		var bounds = new Bounds (targets [0].position, Vector3.zero);
		for (int i = 0; i < targets.Count; i++) {

			bounds.Encapsulate (targets [i].position);
		
		}

		return bounds.size.x;

	}

	Vector3 GetCenterPoint(){

		if (targets.Count == 1) {
			return targets [0].position;
		}

		var bounds = new Bounds (targets [0].position, Vector3.zero);
		for (int i = 0; i < targets.Count; i++) {

			bounds.Encapsulate (targets [i].position);

		}
		return bounds.center;
	}
	public void ZoomPunch(){

		transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 1f);

	}
	public void ZoomSpecial(){

		transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 3f);

	}

	public void SmolShake(){

		CameraShaker.GetInstance("Cam").ShakeOnce(1f, 3f, 0f, 0.5f);

	}
	
	public void BigShaq(){

		CameraShaker.GetInstance("Cam").ShakeOnce(2f, 3f, 0f, 3f);

	}
}

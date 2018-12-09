using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerIndicator_Controller : MonoBehaviour {

	public Transform playerToFollow;
	public Vector3 offset;
	public float smoothTime;
	TextMeshPro text;

	Vector3 currentVelocity;

	// Use this for initialization
	void Start () {
		
		text = transform.Find("Title").gameObject.GetComponent<TextMeshPro>();
		text.text = playerToFollow.gameObject.name;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(playerToFollow == null){

			Debug.Log("No player selected on Indicator you DUMMY!");
			return;
	
		}

		Vector3 newPosition = new Vector3(playerToFollow.position.x + offset.x, playerToFollow.position.y + offset.y, playerToFollow.position.z + offset.z);
		transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref currentVelocity , smoothTime);

	}
}

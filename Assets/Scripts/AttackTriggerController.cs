using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerController : MonoBehaviour {

	GameObject parent;
	Player_Controller playerControl;
	BoxCollider collide;
	Vector3 colliderPosition;
	int combo;
	float comboTime;
	public float comboSetTime;

	bool flipped;
	public bool heavy;


	// Use this for initialization
	void Start () {
		
		parent = transform.parent.gameObject;
		playerControl = parent.GetComponent<Player_Controller>();
		collide = GetComponent<BoxCollider>();
		colliderPosition = playerControl.characterID.TriggerPosition;

	}
	
	// Update is called once per frame
	void Update () {

		flipped = playerControl.flipped;
		if(flipped){

			collide.center = new Vector3(colliderPosition.x,colliderPosition.y, colliderPosition.z);

		}else{

			collide.center = new Vector3(-colliderPosition.x, colliderPosition.y, colliderPosition.z);

		}

	}

	void OnTriggerEnter(Collider c){

		if(c.gameObject.tag == "Player"){

			if(heavy){
			Debug.Log("HEAVY OOF");
			}else{
			Debug.Log("OOF");
			}

		}

	}
}

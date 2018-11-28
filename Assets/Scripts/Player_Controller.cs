using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour {

	public Character_ID characterID;
	bool jumped;

	Rigidbody rb;
	CapsuleCollider playerCollider;
	Vector3 moveDirection;

	// Use this for initialization
	void Awake () {
		
		rb = GetComponent<Rigidbody>();
		playerCollider = GetComponent<CapsuleCollider>();

	}

	void Start(){

		playerCollider.material = characterID.playerPhysic;
		rb.mass = characterID.mass;
		rb.drag = characterID.drag;

	}
	
	// Update is called once per frame
	void Update () {

		if(characterID == null){

			Debug.Log("No CharacterID!");

		}
		
		float horizontalMovement = Input.GetAxis("Horizontal");

		moveDirection = (horizontalMovement * transform.right);

		if (Input.GetButtonDown("Jump")){

			Jump();

		}

	}

	void FixedUpdate () {

		Move();

	}

	void Move(){

        Vector3 yVelFix = new Vector3(0, rb.velocity.y, 0);
        rb.velocity = moveDirection * characterID.speed * Time.deltaTime;
        rb.velocity += yVelFix;

	}

	void Jump(){

		if(!jumped){

			rb.AddForce(Vector3.up * characterID.jumpPower, ForceMode.Impulse);
			jumped = true;

		}

	}

	void OnCollisionEnter(Collision c){

		jumped = false;

	}
}

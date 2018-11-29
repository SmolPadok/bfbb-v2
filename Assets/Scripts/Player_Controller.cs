using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour {

	public Character_ID characterID;
    public float limitMoveOnAttack;

	bool jumped;
    bool flipped;
    bool limitMove;

	Rigidbody rb;
    SpriteRenderer sprite;
	CapsuleCollider playerCollider;
	Vector3 moveDirection;
    Animator anim;

	// Use this for initialization
	void Awake () {
		
		rb = GetComponent<Rigidbody>();
		playerCollider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();

	}

	void Start(){

		playerCollider.material = characterID.playerPhysic;
		rb.mass = characterID.mass;
		rb.drag = characterID.drag;
        sprite = GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {

		if(characterID == null){

			Debug.Log("No CharacterID you dummy!");

		}
        //--------------------------

        float horizontalMovement = Input.GetAxis("Horizontal");

        if (limitMove == false)
        {
            moveDirection = (horizontalMovement * transform.right);
        }else{
            moveDirection = (horizontalMovement * transform.right / limitMoveOnAttack);
        }

        anim.SetInteger("MoveHorizontal", Mathf.RoundToInt(horizontalMovement));

        //--------------------------

        if(horizontalMovement < 0f){
            flipped = false;
        }else if (horizontalMovement > 0f){
            flipped = true;
        }

        //--------------------------

        if(flipped){
            sprite.flipX = true;
        }else{
            sprite.flipX = false;
        }

		if (Input.GetButtonDown("Jump")){
			Jump();
        }

        //--------------------------

        if (Input.GetButtonDown("Punch"))
        {
            Punch();
        }

        //--------------------------

        if (Input.GetButtonDown("Kick"))
        {
            Kick();
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
            anim.SetTrigger("Jump");
            anim.SetBool("Grounded", false);
		}

	}

    void Punch(){

        anim.SetTrigger("Punch");

    }

    void Kick()
    {

        anim.SetTrigger("Kick");

    }

	void OnCollisionEnter(Collision c){
        
        jumped = false;
        anim.SetBool("Grounded", true);

	}

    public void LimitMoveOnAttack(){

        limitMove = true;

    }

    public void FreeMovement(){

        limitMove = false;

    }
}

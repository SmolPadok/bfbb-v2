using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour {

    GameObject manager;
    public int playerNumber;
	public Character_ID characterID;
    public float limitMoveOnAttack;
    public float doubleKeySetTime;
    float doubleKeyTime = 0;
    int axisPresses;
    GameObject attackTrigger;
    BoxCollider trigger;
    GameObject parent, dashParticle, diveParticle, impactParticle;
    Transform effectStash;
    GameObject stat;
    Camera_Controller cam;

	bool jumped;
    bool dived;
    bool mobility = true;
    public bool flipped;
    bool limitMove;
    bool dashed;
    public bool debugDisable;

	Rigidbody rb;
    SpriteRenderer sprite;
	CapsuleCollider playerCollider;
	Vector3 moveDirection;
    Animator anim;

	//FLIPPED = FACING RIGHT

	void Awake () {
		
        manager = GameObject.Find("GameManager");
        cam = GameObject.Find("Main Camera").GetComponent<Camera_Controller>();
		rb = GetComponent<Rigidbody>();
		playerCollider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        anim.runtimeAnimatorController = characterID.animatorController;
        attackTrigger = transform.GetChild(0).gameObject;
        trigger = attackTrigger.GetComponent<BoxCollider>();
        parent = transform.parent.parent.gameObject;
        effectStash = parent.transform.Find("EffectsStash");
        dashParticle = parent.transform.Find("DashSmoke").gameObject;
        diveParticle = parent.transform.Find("DiveSmoke").gameObject;
        impactParticle = parent.transform.Find("ImpactDebris").gameObject;

        if(gameObject.name == "Player 1"){
            stat = GameObject.Find("Stats1");
        }else if (gameObject.name == "Player 2"){
            stat = GameObject.Find("Stats2");
        }else if (gameObject.name == "Player 3"){
            stat = GameObject.Find("Stats3");
        }else if (gameObject.name == "Player 4"){
            stat = GameObject.Find("Stats4");
        }

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
            return;
		}
        //--------------------------
        if(mobility && !debugDisable){

        float horizontalMovement = Input.GetAxis("Horizontal");

        if (limitMove == false)
        {
            moveDirection = (horizontalMovement * transform.right);
        }else{
            moveDirection = (horizontalMovement * transform.right / limitMoveOnAttack);
        }

        if(dashed){
            moveDirection.x = moveDirection.x * characterID.dashPower;
        }

        if(Input.GetButtonDown("Horizontal")){
            if(axisPresses == 0){
                axisPresses = 1;
            }
            else if(axisPresses == 1){
                axisPresses = 2;
            }
        }
        else if(axisPresses == 2){

            Dash();
            axisPresses = 0;
            doubleKeyTime = 0;

        }

        if(axisPresses == 1){

            doubleKeyTime += Time.deltaTime;

        }
        if(doubleKeyTime > doubleKeySetTime){

            axisPresses = 0;
            doubleKeyTime = 0;

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

		if (Input.GetAxisRaw("Vertical") > 0){
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

        if(Input.GetAxisRaw("Vertical") < 0){
            Duck();
        }

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

    void Duck(){

        if(jumped && !dived){
            rb.AddForce(Vector3.down * characterID.divePower, ForceMode.Impulse);
            GameObject DiveSmoke = Instantiate(diveParticle, transform.position, diveParticle.transform.rotation, transform);
            DiveSmoke.SetActive(true);
            dived = true;
            anim.SetTrigger("Dive");
        }

    }
    void Dash(){

        dashed = true;
        GameObject DashSmoke = Instantiate(dashParticle, transform.position, Quaternion.identity, effectStash);
        DashSmoke.SetActive(true);
        StartCoroutine(DashPeriod());

    }

	void OnCollisionEnter(Collision c){
        
        jumped = false;
        dived = false;
        anim.SetBool("Grounded", true);

	}

    private void OnTriggerEnter(Collider c){

        if(c.gameObject.tag == "AttackTrigger"){

            bool heaviness = c.gameObject.GetComponent<AttackTriggerController>().heavy;
            Hurt(heaviness);

        }
        
        if(c.gameObject.tag == "Hazard"){

            Debug.Log("Ouch! A hazard!");

        }

    }

    public void LimitMoveOnAttack(){

        limitMove = true;

    }

    public void FreeMovement(){

        limitMove = false;

    }

    public void Immobilize(){

        mobility = false;

    }

    public void Mobilize(){

        mobility = true;

    }

    public void ImpactBounce(){

		rb.AddForce(Vector3.up * characterID.jumpPower / 1.5f, ForceMode.Impulse);
        GameObject impact = Instantiate(impactParticle, transform.position, impactParticle.transform.rotation, effectStash);
        impact.SetActive(true);

    }

    public void EnableLightAttackTrigger(){

        attackTrigger.GetComponent<AttackTriggerController>().heavy = false;
        trigger.enabled = true;

    }

    public void DisableAttackTrigger(){

        trigger.enabled = false;

    }

    public void EnableHeavyAttackTrigger(){

        attackTrigger.GetComponent<AttackTriggerController>().heavy = true;
        trigger.enabled = true;

    }

    void Hurt(bool heavy){

        if(!heavy){
        cam.SmolShake();
        }else{
        cam.BigShaq();
        }
        anim.SetTrigger("Hurt");

    }

    IEnumerator DashPeriod(){

        yield return new WaitForSeconds(characterID.dashPeriod);

        dashed = false;

    }
}

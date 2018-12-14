using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour {

    GameManager manager;
    public int playerNumber;
	public Character_ID characterID;
    public float limitMoveOnAttack;
    public float doubleKeySetTime;
    float doubleKeyTime = 0;
    int axisPresses;
    GameObject attackTrigger;
    public GameObject stat, indicator;
    BoxCollider trigger;
    GameObject parent, dashParticle, diveParticle, impactParticle, lavaSplash;
    public GameObject deathConfetti;
    public Transform effectStash;
    Camera_Controller cam;
    float knockback = 20;
    float stunChance;
    public float damage = 0;

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
		
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        lavaSplash = parent.transform.Find("LavaSplash").gameObject;
        deathConfetti = parent.transform.Find("DeathConfetti").gameObject;

        if(playerNumber == 1){
		stat = GameObject.Find("Stats1");
        indicator = GameObject.Find("Indicator1");
        }else if(playerNumber == 2){
		stat = GameObject.Find("Stats2");
        indicator = GameObject.Find("Indicator2");
        }else if(playerNumber == 3){
		stat = GameObject.Find("Stats3");
        indicator = GameObject.Find("Indicator3");
        }else if(playerNumber == 4){
		stat = GameObject.Find("Stats4");
        indicator = GameObject.Find("Indicator4");
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

        if (Input.GetButtonDown("Attack"))
        {
            Attack();
        }

        //--------------------------

        if (Input.GetButtonDown("Special"))
        {
            Special();
        }

        if(Input.GetAxisRaw("Vertical") < 0){
            Duck();
        }

        }



	}

	void FixedUpdate () {

        if(mobility){
		Move();
        }
        

	}

	void Move(){

        Vector3 VelFix = new Vector3(rb.velocity.x / 2, rb.velocity.y, 0f);
        rb.velocity = moveDirection * characterID.speed * Time.deltaTime;
        rb.velocity += VelFix;

	}

	void Jump(){

		if(!jumped){
			rb.AddForce(Vector3.up * characterID.jumpPower, ForceMode.Impulse);
			jumped = true;
            anim.SetTrigger("Jump");
            anim.SetBool("Grounded", false);
		}

	}

    void Attack(){

        anim.SetTrigger("Attack");

    }

    void Special()
    {

        anim.SetTrigger("Special");

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
            bool flipped = c.gameObject.GetComponent<AttackTriggerController>().flipped;
            float velX = c.transform.parent.GetComponent<Rigidbody>().velocity.x;
            float velY = c.transform.parent.GetComponent<Rigidbody>().velocity.y;
            int hitPlayer = c.transform.parent.GetComponent<Player_Controller>().playerNumber;
            Hurt(heaviness, flipped, velX, velY, hitPlayer);

        }
        
        if(c.gameObject.tag == "Hazard"){

            Debug.Log("Ouch! A hazard!");
            GameObject LavaSplash = Instantiate(lavaSplash, transform.position, lavaSplash.transform.rotation, effectStash);
            LavaSplash.SetActive(true);

            manager.HazardDeath(this);
            gameObject.SetActive(false);

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

    void Hurt(bool heavy, bool flipped, float velX, float velY, int attackingPlayerNumber){

        float damageDone = damage / 50;
        float multiplyBySpeed = (Mathf.Abs(velX) + Mathf.Abs(velY) / 2) / 10;
        Debug.Log("This is Player " + attackingPlayerNumber + " attacking Player " + playerNumber + ". Damage done = " + damageDone + ", Multiply by speed: " + multiplyBySpeed);

        if(!heavy){
        cam.SmolShake();
        damage += (1f + multiplyBySpeed);
        stunChance = 1;
        }else{
        cam.BigShaq();
        damage += (2f + multiplyBySpeed);
        stunChance = 30;
        }

        if(multiplyBySpeed < 1){
        if(!heavy){
            multiplyBySpeed = 0.5f;
        }else{
            multiplyBySpeed = 1f;
        }
        }

        stunChance += multiplyBySpeed + (damage / 10);

        if(!jumped){
        float randomizeStun = Random.Range(0,100);
        if(randomizeStun <= stunChance){
            anim.SetBool("Grounded", false);
            anim.SetTrigger("Stun");
            mobility = false;
        }else{
            anim.SetTrigger("Hurt");
        }
        }else{
            anim.SetBool("Grounded", false);
            anim.SetTrigger("Stun");
            mobility = false;
        }

        if(flipped){
            rb.AddForce(new Vector3(1f,0.5f,0f) * knockback * damageDone * multiplyBySpeed, ForceMode.Impulse);
        }else{
            rb.AddForce(new Vector3(-1f,0.5f,0f) * knockback * damageDone * multiplyBySpeed, ForceMode.Impulse);
        }

        manager.UpdateDamage(playerNumber);

    }

    IEnumerator DashPeriod(){

        yield return new WaitForSeconds(characterID.dashPeriod);

        dashed = false;

    }
}

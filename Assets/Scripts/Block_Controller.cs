using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Controller : MonoBehaviour
{
    public bool isBlocking = false;
    SpriteRenderer sprite;
    public Animator anim;
    public Transform playerRef;
    public float currentCooldown, maxCooldown;

    void Start(){
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {

        if(isBlocking){
            sprite.enabled = true;
            anim.enabled = true;
            currentCooldown -= Time.deltaTime;
            if(currentCooldown < 0){
                isBlocking = false;
                anim.SetTrigger("Exit");
                playerRef.gameObject.GetComponent<Player_Controller>().BlockStun();
            }
        }else{
            if(currentCooldown < maxCooldown){
            currentCooldown += Time.deltaTime;
            }
        }
    }

    public void endBlock(){
        anim.enabled = false;
        sprite.enabled = false;
    }
}

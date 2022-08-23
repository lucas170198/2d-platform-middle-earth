using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIA : MonoBehaviour
{
    //Components
    private Animator anim;
    private Rigidbody2D rb;

    //Player State
    private enum EnemyState {idle, patrol, chasing};
    private enum AnimState {idle, walk, die};
    // private enum Dir2D {left, right};

    private EnemyState enemyState = EnemyState.idle;
    private AnimState animState = AnimState.idle;
    private bool isAttacking = false;
    private bool isTakingDamage = false;
    [SerializeField] private int health = 2;
    // private Dir2D currentDirection = Dir2D.left;
    // private bool playerIsNear = false;
    // private float actualPlayerDist;
    // private Dir2D playerDir = Dir2D.left;

    // //Serializers
    // [SerializeField] private float leftWalkLimit; // Limit to walk left without fall
    // [SerializeField] private float rightWalkLimit; // Limit to walk right without fall
    // [SerializeField] private float speed = 3f;
    // [SerializeField] private float distanceToAttack = 1f; //Distance to try attack the player

    // private void OnCollisionEnter2D(Collision2D other) {
    // }

    // private void OnTriggerEnter2D(Collider2D other) {
    //     // Near to player
    //     if(other.tag == "Player"){
    //         playerIsNear = true;
    //         actualPlayerDist = Vector2.Distance((Vector2)other.transform.position, (Vector2)transform.position);
    //         playerDir = GetDir(other.transform.position, transform.position);
    //     }
        
    // }

    // private void OnTriggerStay2D(Collider2D other) {
    //     //  Keep distance to player update
    //     if(other.tag == "Player"){
    //         actualPlayerDist = Vector2.Distance((Vector2)other.transform.position, (Vector2)transform.position);
    //         playerDir = GetDir(other.transform.position, transform.position);
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D other) {
    //     if(other.tag == "Player"){
    //         playerIsNear = false;
    //     }
    // }

    // Start is called before the first frame update
    void Start(){
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update(){
        UpdateStateMachine();
        AnimationController();
        Act();
       
    }

    private void UpdateStateMachine(){
        // if((enemyState == EnemyState.idle || enemyState == EnemyState.patrol)
        //     && playerIsNear){
        //     enemyState = EnemyState.trackling;
        // }
        // else if(enemyState == EnemyState.trackling){
        //     if(!playerIsNear){
        //         enemyState = EnemyState.patrol;
        //     }
        //     else if(actualPlayerDist <= distanceToAttack
        //             && Random.Range(1, 3) == 2){ // Eventualy attack
        //         enemyState = EnemyState.attacking;

        //     }
        // }
        // else if((enemyState == EnemyState.attacking) 
        //     && actualPlayerDist > distanceToAttack){
        //         enemyState = EnemyState.trackling;
        // }
    }

    private void Act(){
        // if(enemyState == EnemyState.idle) return;

        // // Just Walk in both directions
        // if(enemyState == EnemyState.patrol){
        //     if(ShouldMoveLeft()){
        //         MoveLeft();
        //     }
        //     else if(ShouldMoveRight()){
        //         MoveRight();
        //     }
        // }
        // // Chase the player
        // else if(enemyState == EnemyState.trackling){
        //     if(playerDir == Dir2D.left && (transform.position.x > leftWalkLimit)){
        //         MoveLeft();
        //     }
        //     else if(playerDir == Dir2D.right && transform.position.x < rightWalkLimit){
        //         MoveRight();
        //     }
        // }
        // // Try to hit the enemy
        // else if(enemyState == EnemyState.attacking){


        // }
    }

    // private void MoveRight(){
    //     rb.velocity = new Vector2(speed, rb.velocity.y);
    //     transform.localScale = new Vector2(1, 1); //Face right
    // }

    // private void MoveLeft(){
    //     rb.velocity = new Vector2(-speed, rb.velocity.y);
    //     transform.localScale = new Vector2(-1, 1);
    // }

    // private bool ShouldMoveLeft(){
    //     if(currentDirection == Dir2D.left){
    //         if(transform.position.x > leftWalkLimit) return true;

    //         currentDirection = Dir2D.right;
    //     }
    //     return false;
    // }

    // private bool ShouldMoveRight(){
    //     if(currentDirection == Dir2D.right){
    //         if(transform.position.x < rightWalkLimit) return true;

    //         currentDirection = Dir2D.left;
    //     }
    //     return false;
    // }

    // private Dir2D GetDir(Vector2 otherPos, Vector2 pos){
    //     return otherPos.x > pos.x ? Dir2D.right : Dir2D.left;
    // }


    /**
    * Animation related functions
    */
    private void AnimationController(){
        if(health == 0){
            animState = AnimState.die;
        }
        else if(isTakingDamage){
            anim.SetTrigger("damage");
            return;
        }
        else if(isAttacking){
            anim.SetTrigger("attack");
            return;
        }
        else if(enemyState == EnemyState.patrol || enemyState == EnemyState.chasing){
            animState = AnimState.walk;
        }
        else{
            animState = AnimState.idle;

        }
        anim.SetInteger("state", (int) animState);
    }

    //Animation events
    public void EndAttacking(){
        animState = AnimState.idle;
        anim.ResetTrigger("attack");
        isAttacking = false;
    }

    public void EndDamage(){
        animState = AnimState.idle;
        anim.ResetTrigger("damage");
        health -= 1;
        isTakingDamage = false;
    }

    public void Die(){
        Destroy(this.gameObject);
    }
}

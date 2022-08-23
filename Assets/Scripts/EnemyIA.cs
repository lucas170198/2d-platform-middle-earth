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
    private enum Dir2D {left, right};

    //Animation 
    private AnimState animState = AnimState.idle;
    private bool isAttacking = false;
    private bool isTakingDamage = false;

    //States
    [SerializeField] private EnemyState enemyState = EnemyState.idle;
    private bool playerIsNear = false; //Indicates if the player is on the sensor zone
    private bool playerIsAhead = false; //Indicates if the player is touching the enemy
    private int health = 2;
    private Dir2D enemyDirection = Dir2D.right;
    private Dir2D playerDirection = Dir2D.left;

    // //Serializers
    [SerializeField] private float leftWalkLimit; // Limit to walk left without fall
    [SerializeField] private float rightWalkLimit; // Limit to walk right without fall
    [SerializeField] private float speed = 2f;

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

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Player"){
            playerIsAhead = true; //Is in front of the player
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject.tag == "Player"){
            playerIsAhead = false;
        }
    }

    //AI Brain: Process Inputs to decide the current player state
    private void UpdateStateMachine(){
        if(playerIsNear){
            enemyState = EnemyState.chasing;
            Debug.Log("chasing");
        }
        else if(enemyState == EnemyState.chasing || enemyState == EnemyState.patrol){
            enemyState = EnemyState.patrol;
            Debug.Log("patrol");
        }
        else{
            enemyState = EnemyState.idle;
            Debug.Log("idle");
        }
    }

    // Take actions acording with current state
    private void Act(){
        if(enemyState == EnemyState.idle) return;

        // Just Walk in both directions
        if(enemyState == EnemyState.patrol){
            PatrolMovment();
        }
        else if(enemyState == EnemyState.chasing){
            ChasingPlayer();
        }
    }

    private void ChasingPlayer(){
        if(playerDirection == Dir2D.right){
            transform.localScale = new Vector2(1, 1); //Face right
            if(playerIsAhead && !isAttacking){// Attack when player is ahead
                StartCoroutine(Attack());
            }
            else if(CanMoveRight() && !isAttacking){ // chasing to player
                MoveRight();
            }
        }
        else{
            transform.localScale = new Vector2(-1, 1); //Face left
            if(playerIsAhead && !isAttacking){// Attack when player is ahead
                StartCoroutine(Attack());
            }
            else if(CanMoveLeft()){
                MoveLeft();
            }
        }
    }

    private void PatrolMovment(){
        if(enemyDirection == Dir2D.right){
            if(CanMoveRight()){
                MoveRight();
            }
            else{
                enemyDirection = Dir2D.left;
            }
        }
        else{
            if(CanMoveLeft()){
                MoveLeft();
            }
            else{
                enemyDirection = Dir2D.right;
            }
        }
    }

    private void MoveRight(){
        rb.velocity = new Vector2(speed, rb.velocity.y);
        transform.localScale = new Vector2(1, 1); //Face right
    }

    private void MoveLeft(){
        rb.velocity = new Vector2(-speed, rb.velocity.y);
        transform.localScale = new Vector2(-1, 1);
    }

    private bool CanMoveRight(){
        return transform.position.x <= rightWalkLimit;
    }

    private bool CanMoveLeft(){
        return transform.position.x >= leftWalkLimit;
    }

    //Play animations
    private void AnimationController(){
        if(health == 0){
            animState = AnimState.die;
        }
        else if(enemyState == EnemyState.patrol || enemyState == EnemyState.chasing){
            animState = AnimState.walk;
        }
        else{
            animState = AnimState.idle;

        }
        anim.SetInteger("state", (int) animState);
    }

    private IEnumerator Attack(){
        yield return new WaitForSeconds(0.05f); // TODO: Move it to a variable to level control
        isAttacking = true;
        anim.SetTrigger("attack");
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

    // External Event
    public void PlayerEnterZone(){
        playerIsNear = true;
    }

    public void PlayerExitZone(){
        playerIsNear = false;
    }

    public void UpdatePlayerPosition(Vector2 playerPostion){
        // Find out if the player is either at left or rigth
        if(playerPostion.x < transform.position.x){
            Debug.Log("Player on left");
            playerDirection = Dir2D.left;
        }else{
            playerDirection = Dir2D.right;
            Debug.Log("Player on right");
        }
    }
}

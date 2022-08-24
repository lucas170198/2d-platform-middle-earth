using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIA : MonoBehaviour
{
    //Components
    private Animator anim;
    private Rigidbody2D rb;

    //Player State
    private enum EnemyState {idle, patrol, chasing, damage, attack, die};
    private enum AnimState {idle, walk, die};
    private enum Dir2D {left, right};

    //Animation 
     [SerializeField] private AnimState animState = AnimState.idle;
    private bool canDie = true;
    private bool canAttack = true;
    private bool canTakeDamage = true;
    //States
    [SerializeField] private EnemyState enemyState = EnemyState.idle;
    private bool playerIsNear = false; //Indicates if the player is on the sensor zone
    private bool playerIsAhead = false; //Indicates if the player is touching the enemy
    private bool playerIsInAttackRange = false; //Indicates if the attack will hit the player
    private bool isDeath = false;
    private Dir2D enemyDirection = Dir2D.right;
    private Dir2D playerDirection = Dir2D.left;
    private int health = 3;

    // //Serializers
    [SerializeField] private float leftWalkLimit; // Limit to walk left without fall
    [SerializeField] private float rightWalkLimit; // Limit to walk right without fall
    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackDistance = 1f; //Range of the attack

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
        if(health <= 0){
            enemyState = EnemyState.die;
        }

        if(enemyState == EnemyState.attack 
            || enemyState == EnemyState.damage
            || enemyState == EnemyState.die){
            return;
        }
        if(playerIsNear){
            enemyState = EnemyState.chasing;
        }
        else if(enemyState == EnemyState.chasing || enemyState == EnemyState.patrol){
            enemyState = EnemyState.patrol;
        }
        else{
            enemyState = EnemyState.idle;
        }
    }

    // Take actions acording with current state
    private void Act(){
        if(enemyState == EnemyState.idle 
            || enemyState == EnemyState.attack 
            || enemyState == EnemyState.damage 
            || enemyState == EnemyState.die) return;

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
            if(playerIsAhead){// Attack when player is ahead
                Attack();
            }
            else if(CanMoveRight()){ // chasing to player
                MoveRight();
            }
        }
        else{
            transform.localScale = new Vector2(-1, 1); //Face left
            if(playerIsAhead){// Attack when player is ahead
                Attack();
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
        if(enemyState == EnemyState.die && canDie){
            anim.SetTrigger("die");
            canDie = false;
        }
        else if(enemyState == EnemyState.attack && canAttack){
            anim.SetTrigger("attack");
            canAttack = false;
        }
        else if(enemyState == EnemyState.damage && canTakeDamage){
            anim.SetTrigger("damage");
            canTakeDamage = false;
        }
        if(enemyState == EnemyState.patrol || enemyState == EnemyState.chasing){
            animState = AnimState.walk;
            anim.SetInteger("state", (int) animState);

        }
        else if(enemyState == EnemyState.idle){
            animState = AnimState.idle;
            anim.SetInteger("state", (int) animState);
        }
        else{
            Debug.Log("No animation trigger");
        }
    }

    private void Attack(){
        enemyState = EnemyState.attack;
    }

    //Animation events
    public void AttackHitPoint(){
        if(playerIsInAttackRange){
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().EnemyAttack();
        }
    }

    public void EndAttacking(){
        enemyState = EnemyState.chasing;
        canAttack = true;
        canTakeDamage = true;
    }

    public void TakeHit(){
        enemyState = EnemyState.damage;
    }

    public void EndDamage(){
        enemyState = EnemyState.chasing;
        health -= 1;
        canTakeDamage = true;
        canAttack = true;
    }

    public void Die(){
        enemyState = EnemyState.die;
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
            playerDirection = Dir2D.left;
        }else{
            playerDirection = Dir2D.right;
        }

        //See if it's in the attack range zone
        playerIsInAttackRange = Vector2.Distance(playerPostion, transform.position) < attackDistance;
    }
}

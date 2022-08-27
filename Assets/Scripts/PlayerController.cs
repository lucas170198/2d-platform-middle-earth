using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    // Components
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    // Serializers
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float leftLimit = -20f;
    [SerializeField] private float rightLimit = 20f;

    //UI hearts
    [SerializeField] private Image heart1;
    [SerializeField] private Image heart2;
    [SerializeField] private Image heart3;

    //UI Gems
    [SerializeField] private Image gem1;
    [SerializeField] private Image gem2;
    [SerializeField] private Image gem3;


    // Player State
    private enum AnimState {idle, walking, rolling};

    private AnimState animState = AnimState.idle;
    private EnemyIA enemy;
    private bool isRolling = false;
    private bool isAttacking = false;
    private bool isTakingDamage = false;
    private bool enemyIsNear = false;
    private bool isJumping = false;
    private bool isFalling = false;
    private bool isDeath = false;
    private bool inSecondJump = false;
    private Stack<Image> hearts;
    private Queue<Image> gemsImg;

    // Game State
    private int gems = 0;
    private int health = 3;


    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();

        //Initizaling hearts
        hearts = new Stack<Image>();
        hearts.Push(heart1);
        hearts.Push(heart2);
        hearts.Push(heart3);

        //Initializing gems
        gem1.enabled = false;
        gem2.enabled = false;
        gem3.enabled = false;
        gemsImg = new Queue<Image>();
        gemsImg.Enqueue(gem1);
        gemsImg.Enqueue(gem2);
        gemsImg.Enqueue(gem3);
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Getting a gem
        if(other.tag == "Gem"){
            Destroy(other.gameObject);
            gems += 1;
            Image gem = gemsImg.Dequeue();
            gem.enabled = true;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        // Colliding with enemies
        if(other.gameObject.tag == "Enemy"){
            enemyIsNear = true;
            enemy = other.gameObject.GetComponent<EnemyIA>();
            if(isFalling && enemy != null){
                Jump();
                MoveLeft();
            }  
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject.tag == "Enemy"){
            enemyIsNear = false;
            enemy = null;
        }
    }

    // Update is called once per frame
    void Update(){
        if(isDeath) return;
        ReadInputMov();
        PlayerStateMachine();
        AnimationController();
    }

    private void Jump(){
        anim.SetTrigger("jump");
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isJumping = true;
    }

    private void MoveRigth(){
        if(transform.position.x <= rightLimit){
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }
    }

    private void MoveLeft(){
        if(transform.position.x >= leftLimit){
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        } 
    }

    private void Attack(){
        anim.SetTrigger("attack");
        isAttacking = true;
        if(enemyIsNear && enemy != null){
            enemy.TakeHit();
        }
    }

    private void TakeHit(){
        Image h = hearts.Pop();
        Destroy(h.gameObject);
        health -= 1;

        //Trigger die animation
        anim.SetTrigger("damage");
        isTakingDamage = true;
    }

    //External events
    public void EndAttacking(){
        isAttacking = false;
    }

    public void EnemyAttack(){
        if(!isTakingDamage && !isRolling && !isJumping && !isFalling){
            TakeHit();
        }
    }

    public void EndDamage(){
        isTakingDamage = false;

        if(health <= 0){
            isDeath = true;
            anim.SetTrigger("die");
        }
    }

    public void GameOver(){
        Destroy(this.gameObject);
        SceneManager.LoadScene("GameOver");
    }

    public void TakeHitBySpines(){
        if(isTakingDamage && isDeath) return;
        TakeHit();
        if(health > 0){ // Dont jump on the last life
            Jump();
        }
    }

    private void PlayerStateMachine(){
        if(isJumping){
            //Get in the top, start to fall
            if(rb.velocity.y < Mathf.Epsilon){
                isJumping = false;
                isFalling = true;
            }
        }
        else if(isFalling){
            //Go back to idle after fall
            if(coll.IsTouchingLayers(ground)){
                isFalling = false;
                inSecondJump = false;
            }
        }
    }

    private void ReadInputMov(){
        // Do nothing during some movments
        if(isAttacking || isTakingDamage) return;

        float hDirection = Input.GetAxis("Horizontal");
        
        // Rolling
        if((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) 
            && Mathf.Abs(hDirection) > 0){
            isRolling = true;
        }

        if(hDirection > 0){
            MoveRigth();
        }

        else if(hDirection < 0){
            MoveLeft();
        }

        // Jump
        if(Input.GetButtonDown("Jump")){
            if(coll.IsTouchingLayers(ground)){ // first jump
                Jump();
            }
            else if(!inSecondJump && isFalling){
                inSecondJump = true;
                Jump();
            }

            
        }
        
        // Attack
        if(Input.GetKey(KeyCode.X) && coll.IsTouchingLayers(ground)){
            Attack();
        }

    }

    private void AnimationController(){
        if(Mathf.Abs(rb.velocity.x) > Mathf.Epsilon){
            if(isRolling){
                animState = AnimState.rolling;
            }
            else{
                animState = AnimState.walking;
            }
            
        }
        else{
            animState = AnimState.idle;
            isRolling = false;
        }
        anim.SetInteger("state", (int) animState);
    }
}

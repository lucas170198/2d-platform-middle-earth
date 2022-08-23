using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float damageIntensity = 1.5f;


    // Player State
    private enum AnimState {idle, walking, jumping, falling, takingdamage, rolling, attacking};
    private AnimState animState = AnimState.idle;
    private bool isRolling = false;
    private bool isAttacking = false;

    // Game State TODO move for a better place
    public int gems = 0;


    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Getting a gem
        if(other.tag == "Gem"){
            Destroy(other.gameObject);
            gems += 1;

        }
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        // Colliding with enemies
        if(other.gameObject.tag == "Enemy"){
            // Don't take hits while is running
            if(isRolling) return;
        

            if(animState == AnimState.falling){
                Destroy(other.gameObject);
                Jump();
            }
            // Take damage
            else {
                animState = AnimState.takingdamage;
                // Enemy is on my right
                if(other.gameObject.transform.position.x > transform.position.x){
                    rb.velocity = new Vector2(-damageIntensity, rb.velocity.y);
                }
                // Enemy on my left
                else{
                    rb.velocity = new Vector2(damageIntensity, rb.velocity.y);

                }
            }
        }
    }

    // Update is called once per frame
    void Update(){

        ReadInputMov();
        UpdateAnimStateMachine();
        
    }

    private void Jump(){
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        animState = AnimState.jumping;
    }

    private void ReadInputMov(){
        // Dont during some movments
        if(animState == AnimState.takingdamage || animState == AnimState.attacking) return;

        float hDirection = Input.GetAxis("Horizontal");
        
        // Rolling
        if((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) 
            && Mathf.Abs(hDirection) > 0){
            isRolling = true;
        }

        // Move foward
        if(hDirection > 0){
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }
        // Move back
        else if(hDirection < 0){
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }

        // Jump
        if(Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground)){
            Jump();
        }
        
        // Attack
        if(Input.GetKey(KeyCode.X) && coll.IsTouchingLayers(ground)){
            isAttacking = true;
        }

    }

    private bool isPlayingAnimation(string stateName) {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }

    private void UpdateAnimStateMachine(){
        if(animState == AnimState.jumping){
            // Get in the top, start to fall
            if(rb.velocity.y < Mathf.Epsilon){
                animState = AnimState.falling;
            }
            
        }
        else if(animState == AnimState.falling){
            // Go back to idle after fall
            if(coll.IsTouchingLayers(ground)){
                animState = AnimState.idle;
            }
        }
        else if(animState == AnimState.takingdamage){
            //almost stoping
            if(Mathf.Abs(rb.velocity.x) < Mathf.Epsilon){
                animState = AnimState.idle;
            }
        }
        else if(isAttacking){
            //stop the attack after animation ends
            if(animState == AnimState.attacking
              && !isPlayingAnimation("wizard_attack")){
                isAttacking = false;
                animState = AnimState.idle;
            }
            else{
                animState = AnimState.attacking;
            }
        }

        else if(Mathf.Abs(rb.velocity.x) > Mathf.Epsilon){
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

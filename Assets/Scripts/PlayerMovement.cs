using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
	private float walkSpeed = 10f;
    private float moveInput;
	private float jumpForce = 0f;
	private Rigidbody2D body;
    public Animator animator;
    public PhysicsMaterial2D bounceMat, normalMat;
    public LayerMask groundMask;
    public Vector2 boxSize;
    public float castDistance;
    private float jumpForceX = 0f;
    private bool startJump = false;
    private float posx, posy;
    private void Awake(){
		body = GetComponent<Rigidbody2D>();
        posx = body.transform.position.x;
        posy = body.transform.position.y;
	}

    private void Update()
    {	  

        if(Input.GetKeyUp(KeyCode.R)){
            body.position = new Vector2(posx,posy);
    
        }

        float camPosY = 20*(int)(transform.position.y/20)+8;

        Camera.main.transform.position = new Vector3(15,camPosY,-10);

        moveInput = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("speed", Math.Abs(body.velocity.x));
        animator.SetBool("startJump", startJump);
        animator.SetFloat("yVel", body.velocity.y);
        animator.SetBool("isGrounded", isGrounded());

        if(!startJump && isGrounded()){
            body.velocity = new Vector2(moveInput * walkSpeed, body.velocity.y);
            if(body.velocity.x<0){
                transform.localScale = new Vector3(-7,7,1);
            }
            else if (body.velocity.x>0) transform.localScale = new Vector3(7,7,1);
            if (Input.GetKeyDown(KeyCode.Space)){
                body.velocity = new Vector2(0f, 0f);
                startJump = true;
            }
        }

        if (startJump && isGrounded()){
            jumpForce += 0.15f;
            if (Input.GetKeyUp(KeyCode.Space)||jumpForce >= 75f){
                //print(moveInput*jumpForce);
                if(jumpForce < 25f ){
                    jumpForce=25f;
                    jumpForceX = transform.localScale.x/7*jumpForce/2.3f;
                }
                else jumpForceX = transform.localScale.x/7*jumpForce/2.7f;
                startJump = false;
                body.velocity = new Vector2(0f, jumpForce);
                body.AddForce(new Vector2(moveInput, 0f)*jumpForce, ForceMode2D.Impulse);
                Invoke("FuncionDespuesDeEsperar", 0.03f);
                jumpForce = 0f;
            }
        }

        if (body.velocity.y <= 0&&body.velocity.y>=-1){
            //fallAndAwake();
        }
        print(isGrounded());

        if (body.velocity.y >= 8f){
            body.sharedMaterial = bounceMat;
        }
        else {
            body.sharedMaterial = normalMat;
        }

    }

    void printValues(){
        
    }

private void fallAndAwake() {
    if (isGrounded()){
        animator.SetBool("fallen", true);
        Invoke("afterFallAwake", 5.0f);
    }
}

private void afterFallAwake() {
    animator.SetBool("fallen", false);
}

    public bool isGrounded(){
        if(Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundMask)/*||(body.velocity.y <= 0&&body.velocity.y>=-1)*/){
            body.sharedMaterial = normalMat;
            return true;
        }
        else return false;
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position-transform.up * castDistance, boxSize);
    }

    void OnCollisionEnter2D(Collision2D other) {
        
    }

    void FuncionDespuesDeEsperar(){
        body.velocity = new Vector2(jumpForceX, body.velocity.y);
    }

}

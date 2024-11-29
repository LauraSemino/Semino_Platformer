using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D SKRigidBody;
    public FacingDirection currentDirection;
    public float acceleration;
    public int maxSpeed;
    public float distanceGroundL;
    public float distanceGroundR;
    
    public float apexHeight;
    public float apexTime;
    public float gravity;
    public float initJumpVel;

    public float terminalVel;
    public float coyoteTime;

    public float friction;

    public int health = 10;

    public bool isSliding;

    public enum FacingDirection
    {
        left, right
    }
    public enum CharacterState
    {
        idle, walk, jump, die
    }
    public CharacterState currentCharacterState = CharacterState.idle;
    public CharacterState previousCharacterState = CharacterState.idle;

    // Start is called before the first frame update
    void Start()
    {
        initJumpVel = (2 * apexHeight / apexTime);
        SKRigidBody = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {   
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2();
        
        if (Input.GetKey(KeyCode.A))
        {
            playerInput.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerInput.x = 1;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerInput.y = 1;
        }
        if (coyoteTime > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isSliding = true;
            }
            else
            {
                isSliding = false;
            }
        }
        
        previousCharacterState = currentCharacterState;

        

        switch(currentCharacterState)
        {
            case CharacterState.die:

                break;
            case CharacterState.jump:
                if(IsGrounded())
                {
                    if(IsWalking())
                    {
                        currentCharacterState = CharacterState.walk;
                    }
                    else
                    {
                        currentCharacterState = CharacterState.idle;
                    }
                }
                break;
            case CharacterState.walk:
                if (!IsWalking())
                {
                    currentCharacterState = CharacterState.idle;
                }
                if (coyoteTime <= 0)
                {
                    currentCharacterState = CharacterState.jump;
                }
                break;
            case CharacterState.idle:
                if(IsWalking())
                {
                    currentCharacterState = CharacterState.walk;
                }
                if(coyoteTime <= 0)
                {
                    currentCharacterState = CharacterState.jump;
                }
                break;
            
        }
        if (IsDead() == true)
        {
            currentCharacterState = CharacterState.die;
        }

        MovementUpdate(playerInput);
       
        //Debug.Log(playerInput.x);

        if (IsGrounded() == false && coyoteTime <= 0)
        {
            coyoteTime = 0;
        }
        if (SKRigidBody.velocity.y > 0.1)
        {
            coyoteTime = 0;
        }
        
    }
    public bool IsDead()
    {
        return health <= 0;
    }    
    private void onDeathAnimationDone()
    {
        gameObject.SetActive(false);
    }
   
    private void MovementUpdate(Vector2 playerInput)
    {
        Vector2 currentVelocity = SKRigidBody.velocity;
        gravity = (-2 * apexHeight / (Mathf.Pow(apexTime, 2)));
        currentVelocity += gravity*Time.deltaTime * Vector2.up;

        


        //SKRigidBody.AddForce(new Vector2(0, gravity*Time.deltaTime), ForceMode2D.Force);
        if (coyoteTime > 0)
        {
            acceleration = 200;
            friction = 30;
        }
        else
        {
            friction = 0;
            acceleration = 20;
        }
        if (playerInput.x < 0)
        {
            currentVelocity += acceleration * Vector2.left * Time.deltaTime;
            
           
        }
        if(playerInput.x > 0)
        {
            currentVelocity -= acceleration * Vector2.left * Time.deltaTime;
            
        }
        if(playerInput.y > 0 && coyoteTime > 0)
        {
            if (isSliding == false)
            {
                currentVelocity += initJumpVel * Vector2.up;
            }
            if (isSliding == true)
            {
                currentVelocity += initJumpVel/2 * Vector2.up;
            }
            
            //SKRigidBody.AddForce(new Vector2(0, initJumpVel), ForceMode2D.Impulse);
        }


        if (currentVelocity.x > 0.1)
        {
            currentVelocity.x -= friction * Time.deltaTime;
            currentDirection = FacingDirection.right;

        }
        if (currentVelocity.x < -0.1)
        {
            currentVelocity.x += friction * Time.deltaTime;
            currentDirection = FacingDirection.left;
        }

        if (isSliding == true)
        {
            //gives the player a minimum speed so they are always moving while sliding
            if (currentDirection == FacingDirection.right && coyoteTime > 0 && currentVelocity.x < 10)
            {
                currentVelocity.x = 10;
               
            }
            if(currentDirection == FacingDirection.left && coyoteTime > 0 && currentVelocity.x > -10)
            {
                currentVelocity.x = -10;
                
            }
            
         }



        //SKRigidBody.velocity *= new Vector2(0.9f, 0);

        //SKRigidBody.AddForce(new Vector2(acceleration, 0), ForceMode2D.Force);
        if (SKRigidBody.velocity.x > 7 && isSliding == false)
        {
            currentVelocity.x = 7;
            
        }
        if (SKRigidBody.velocity.x < -7 && isSliding == false)
        {
            currentVelocity.x = -7;
            
        }
        if (SKRigidBody.velocity.x > 15 && isSliding == true)
        {
            currentVelocity.x = 15;

        }
        if (SKRigidBody.velocity.x < -15 && isSliding == true)
        {
            currentVelocity.x = -15;

        }
        if (SKRigidBody.velocity.y < terminalVel)
        {
            currentVelocity.y = terminalVel;
        }

       
        RaycastHit2D lfGroundL = Physics2D.Raycast(new Vector2 (transform.position.x + 0.4f, transform.position.y), -Vector2.up);
        RaycastHit2D lfGroundR = Physics2D.Raycast(new Vector2(transform.position.x - 0.4f, transform.position.y), -Vector2.up);
        
        if (lfGroundL && lfGroundR)
        {
            
            distanceGroundL = Mathf.Abs(lfGroundL.point.y - transform.position.y);
            distanceGroundR = Mathf.Abs(lfGroundR.point.y - transform.position.y);

        }
        //Debug.Log(currentVelocity);
        SKRigidBody.velocity = currentVelocity;

    }

    public bool IsWalking()
    {
        //Debug.Log(IsGrounded());
        if (coyoteTime > 0 && (SKRigidBody.velocity.x > 0.2 || SKRigidBody.velocity.x < -0.2))
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
    public bool IsGrounded()
    {
       
        if (distanceGroundL < 0.05 || distanceGroundR < 0.05)
        {
            coyoteTime = 0.2f;
            return true;
    
        }
        else
        {
            coyoteTime -= Time.deltaTime;
            return false;
        }
    }

    public FacingDirection GetFacingDirection()
    {
        if (currentDirection == FacingDirection.right)
        {
            return FacingDirection.right;
        }
        else
        {
            return FacingDirection.left;
        }
        
            

    }
}

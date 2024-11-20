using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D SKRigidBody;
    public FacingDirection currentDirection;
    public float acceleration;
    public int maxSpeed;
    public float distanceGround;
    public float apexHeight;
    public float apexTime;
    public float gravity;
    public float initJumpVel;

    public float terminalVel;
    public float coyoteTime;

    public float proxyTime;

    public enum FacingDirection
    {
        left, right
    }

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

    private void MovementUpdate(Vector2 playerInput)
    {

        Vector2 currentVelocity = SKRigidBody.velocity;

        gravity = (-2 * apexHeight / (Mathf.Pow(apexTime, 2)));
        currentVelocity += gravity*Time.deltaTime * Vector2.up;
        //SKRigidBody.AddForce(new Vector2(0, gravity*Time.deltaTime), ForceMode2D.Force);
        if (coyoteTime > 0)
        {
            acceleration = 200;
        }
        else
        {
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
            currentVelocity += initJumpVel * Vector2.up;
            //SKRigidBody.AddForce(new Vector2(0, initJumpVel), ForceMode2D.Impulse);
        }


        if (currentVelocity.x > 0.1)
        {
            currentDirection = FacingDirection.right;

        }
        if (currentVelocity.x < -0.1)
        {
            currentDirection = FacingDirection.left;
        }
        if (currentVelocity.y < 0 && coyoteTime <= 0)
        {
            proxyTime -= Time.deltaTime;
            if (proxyTime <= 0)
            {
                currentVelocity += initJumpVel * Vector2.up;
            }
        }
        else
        {
            proxyTime = 5f;
        }
        
        

        //SKRigidBody.velocity *= new Vector2(0.9f, 0);

        //SKRigidBody.AddForce(new Vector2(acceleration, 0), ForceMode2D.Force);
        if (SKRigidBody.velocity.x > 7)
        {
            currentVelocity.x = 7;
            
        }
        if (SKRigidBody.velocity.x < -7)
        {
            currentVelocity.x = -7;
            
        }
        if (SKRigidBody.velocity.y < terminalVel)
        {
            currentVelocity.y = terminalVel;
        }

        RaycastHit2D lfGround = Physics2D.Raycast(transform.position, -Vector2.up);
        if (lfGround)
        {
            distanceGround = Mathf.Abs(lfGround.point.y - transform.position.y);

        }
        Debug.Log(currentVelocity);
        SKRigidBody.velocity = currentVelocity;

    }

    public bool IsWalking()
    {
        Debug.Log(IsGrounded());
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
       
        if (distanceGround < 0.66)
        {
            coyoteTime = 0.3f;
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

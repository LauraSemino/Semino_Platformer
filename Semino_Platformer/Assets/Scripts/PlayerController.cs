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


    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {

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
        MovementUpdate(playerInput);
        Debug.Log(playerInput.x);

    }

    private void MovementUpdate(Vector2 playerInput)
    {
        
        acceleration = 0;
        if (playerInput.x < 0)
        {
            acceleration = -4f;
            currentDirection = FacingDirection.left;
            
        }
        if(playerInput.x > 0)
        {
            acceleration = 4f;
            currentDirection = FacingDirection.right;
        }
        
        //SKRigidBody.velocity *= new Vector2(0.9f, 0);
        
        SKRigidBody.AddForce(new Vector2(acceleration, 0), ForceMode2D.Force);
        if (SKRigidBody.velocity.x > 5)
        {
            SKRigidBody.velocity = new Vector2(5,SKRigidBody.velocity.y);
            
        }
        if (SKRigidBody.velocity.x < -5)
        {
            SKRigidBody.velocity = new Vector2(-5, SKRigidBody.velocity.y);
            
        }

        RaycastHit2D lfGround = Physics2D.Raycast(transform.position, -Vector2.up);
        if (lfGround)
        {
            distanceGround = Mathf.Abs(lfGround.point.y - transform.position.y);

        }
    }

    public bool IsWalking()
    {
        if (IsGrounded() == true && (SKRigidBody.velocity.x > 0 || SKRigidBody.velocity.x < 0))
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
            return true;
        }
        else
        {
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

using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class JumpingEnemy : EnemyMovements
{
    public JumpPoint jumpPoint;
    private bool isJumping = false;
    private bool isSetDirection = false;
    private CostumeTrigger jumpFromTrigger;

    public event Action jumpOnEntry;
    private float xVelocity;



    protected override void virtualStart()
    {
        jumpFromTrigger = jumpPoint.GetJumpFromPointTrigger();
        jumpFromTrigger.EnterTrigger += onJumpPointEnter;
        jumpOnEntry += jump;
        xVelocity = GetXDirection(this.gameObject.transform.position, direction).x;
    }
    protected override void CostumVirtualMovementUpdate()
    {

        if (!playerInRange)
        {
                if (!isSetDirection)
                {
                    Debug.Log("Set Direction");
                    xVelocity = GetXDirection(this.gameObject.transform.position, direction).x;
                    rb.linearVelocity =
                        new Vector2( xVelocity* movementSpeed, 0);
                    isSetDirection = true;
                }

        }else{}
    }

    Vector2 GetXDirection(Vector2 pointA, Vector2 pointB)
    {
        float directionf = pointB.x - pointA.x;
        if (directionf > 0) return Vector2.right;
        if (directionf < 0) return Vector2.left;
        return Vector2.zero;
    }

    public void jump()
    {

        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float initialJumpVelocity = Mathf.Sqrt(2*gravity*jumpPoint.GetJumpHeight()*this.gameObject.GetComponent<Rigidbody2D>().mass);
        rb.linearVelocity=new Vector2(xVelocity*1.2f,initialJumpVelocity*1.1f);
        isJumping = true;
        Debug.Log(rb.linearVelocity);
        Debug.Log(jumpPoint.GetJumpHeight());
    }

    void onJumpPointEnter(Collider2D collider)
    {

        if (collider.gameObject.Equals(hitboxTrigger.gameObject))
        {
            jumpOnEntry?.Invoke();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isJumping)
        {
            isJumping = false;
            isSetDirection = false;
        }
    }


    protected override void SetDirection() => isSetDirection = false;
}

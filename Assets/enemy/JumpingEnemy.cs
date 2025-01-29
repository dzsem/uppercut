using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class JumpingEnemy : EnemyMovements
{
    public JumpPoint jumpPoint;
    private bool isJumping = false;
    private bool isSetDirection = false;

    protected override void CostumVirtualMovementUpdate()
    {
        if (!playerInRange)
        {
            if (!isJumping)
            {
                if (!isSetDirection)
                {
                    rb.linearVelocity =
                        new Vector2(GetXDirection(this.gameObject.transform.position, direction).x * movementSpeed, 0);
                    isSetDirection = true;
                }
            }
        }
    }

    Vector2 GetXDirection(Vector2 pointA, Vector2 pointB)
    {
        float direction = pointB.x - pointA.x;
        if (direction > 0) return Vector2.right;  // (1,0)
        if (direction < 0) return Vector2.left;   // (-1,0)
        return Vector2.zero; // Ha a pontok x koordinátája megegyezik
    }

    public void jump()
    {
        var jumpToPoint = jumpPoint.GetJumpToPoint();
        var jumpFromPoint = jumpPoint.GetJumpFromPointTrigger().gameObject;
        var jumpFromTrigger = jumpPoint.GetJumpFromPointTrigger();

        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float time = Mathf.Abs(jumpPoint.GetJumpWidth() / movementSpeed);
        float initialJumpVelocity = (jumpPoint.GetJumpHeight()-0.5f*gravity*Mathf.Sqrt(time))/time;
        rb.linearVelocity=new Vector2(movementSpeed*Mathf.Sign(jumpPoint.GetJumpWidth()),initialJumpVelocity);
        isJumping = true;
    }

    void OnCollisionEnter2D(Collision2D collision) => isJumping = false;

    protected override void TurningHandler(Collider2D collider)
    {
        base.TurningHandler(collider);
        isSetDirection = false;
    }

}

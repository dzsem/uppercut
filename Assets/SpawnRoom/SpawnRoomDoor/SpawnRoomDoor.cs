using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public BoxCollider2D physicsCollider;
    public Animator animator;
    public bool isBroken = false;

    public void OnHit(int damage)
    {
        isBroken = true;
        animator.SetBool("isDoorBroken", true);
    }
}

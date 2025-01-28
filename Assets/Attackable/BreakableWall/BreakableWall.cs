using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public BoxCollider2D physicsCollider;
    public Animator animator;

    public bool isBroken = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnHit(int damage)
    {
        isBroken = true;
        animator.SetBool("isDoorBroken", true);
    }
}

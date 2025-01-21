using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    public GameObject ground;
    public bool touchesGround;
    public Rigidbody2D rb;
    public float jumpForce;
    public float walksSpeed;
    public BoxCollider2D col;

    /// <summary>
    /// Azt tárolja, melyik vízszintes irányba néz a player. -1: balra, 1: jobbra, 0: nem def.
    /// Ez alapján lesz meghatározva a dash iránya.
    /// </summary>
    private int _facingDirection = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        // down-ra vagy s-re átesik, ha földön van
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && ground.tag == "openFloor" && touchesGround)
        {
            col.isTrigger = true;
        }

        // space-re uppercut-ol, ha földön van
        // TODO: damage
        if (Input.GetKeyDown(KeyCode.Space) && touchesGround)
        {
            rb.linearVelocityY = 0f;
            rb.AddForce(Vector2.up * jumpForce);
            touchesGround = false;
            GetComponent<Animator>().SetBool("isPerformingUppercut", true);
        }

        rb.linearVelocityX = Input.GetAxis("Horizontal") * walksSpeed;

        UpdateMovementStatus();

        // TODO: dash

        UpdateAnimation();
    }

    /// <summary>
    /// Frissíti a belső mozgással kapcsolatos változókat a physics engine alapján.
    /// </summary>
    private void UpdateMovementStatus()
    {
        _facingDirection = Math.Sign(rb.linearVelocityX);

        GetComponent<SpriteRenderer>().flipX = (_facingDirection == (-1));
    }

    /// <summary>
    /// Frissíti az animátor állapotát.
    /// </summary>
    private void UpdateAnimation()
    {
        // isWalking igaz lesz, ha a vízszintes sebesség nem nulla
        GetComponent<Animator>().SetBool("isWalking", (Mathf.Abs(rb.linearVelocityX) > 0f));

        // szűnjön meg az uppercut animáció, amikor elkezdünk esni, vagy földet érünk
        if (rb.linearVelocityY < 0f || touchesGround)
        {
            GetComponent<Animator>().SetBool("isPerformingUppercut", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != gameObject)
        {
            //GetComponent<Animator>().SetBool("groundTouch", true); //will be important if we have jump animation.
            touchesGround = true;
            col.sharedMaterial.friction = 0.6f;
            ground = collision.gameObject;
            col.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != gameObject)
        {
            touchesGround = false;
            //GetComponent<Animator>().SetBool("groundTouch", false); //will be important if we have jump animation.
            col.sharedMaterial.friction = 0f;
        }

    }
}

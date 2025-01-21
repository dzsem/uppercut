using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    public GameObject ground;
    public bool touchesGround;
    public Rigidbody2D rb;
    public float jumpForce;
    public float dashForce;
    public float walksSpeed;
    public BoxCollider2D col;
    public BoxCollider2D dashPunchBox;
    private float _dashPunchBoxOffsetX;

    /// <summary>
    /// Azt tárolja, melyik vízszintes irányba néz a player. -1: balra, 1: jobbra, 0: nem def.
    /// Ez alapján lesz meghatározva a dash iránya.
    /// </summary>
    private int _facingDirection = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _dashPunchBoxOffsetX = dashPunchBox.offset.x;
    }

    // Update is called once per frame
    void Update()
    {
        // down-ra vagy s-re átesik, ha földön van
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && ground.tag == "openFloor" && touchesGround)
        {
            col.isTrigger = true;
        }

        rb.linearVelocityX = Input.GetAxis("Horizontal") * walksSpeed;

        UpdateMovementStatus();

        ProcessDashInput();

        UpdateAnimation();
    }

    private void ProcessDashInput()
    {
        if (Input.GetKeyDown(KeyCode.X) && Input.GetAxis("Horizontal") != 0f)
        {
            rb.AddForce(Vector2.right * dashForce * Math.Sign(Input.GetAxis("Horizontal")));
        }
        // space-re uppercut-ol, ha földön van és nyomod a fel inputot
        // TODO: damage
        // TODO: dash lekódolása időre (FixedUpdate impl.)
        else if (Input.GetKeyDown(KeyCode.Space) && touchesGround)
        {
            rb.linearVelocityY = 0f;
            rb.AddForce(Vector2.up * jumpForce);
            touchesGround = false;
            GetComponent<Animator>().SetBool("isPerformingUppercut", true);
        }
    }

    /// <summary>
    /// Frissíti a belső mozgással kapcsolatos változókat a physics engine alapján.
    /// </summary>
    private void UpdateMovementStatus()
    {
        // 0 kizárása
        _facingDirection = Math.Sign(rb.linearVelocityX) == (-1) ? (-1) : 1;

        GetComponent<SpriteRenderer>().flipX = (_facingDirection == (-1));

        dashPunchBox.offset = new Vector2(
            _facingDirection * _dashPunchBoxOffsetX,
            dashPunchBox.offset.y
        );
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

    /// <summary>
    /// Megállapítja, hogy otherObject lehet-e platform. Mivel a punchboxok ugyanabban a gameobjecten belül
    /// vannak, és nincsen felettük rigidbody, ezért összevonódnak; ekkor minden megütött objektum a 3-as (Hitbox)
    /// layeren belül lesz. Ezeket ki kell zárni:(
    /// </summary>
    /// <param name="otherObject">A collision-on belüli másik gameobject.</param>
    /// <returns></returns>
    private bool IsGameObjectGround(GameObject otherObject)
    {
        return otherObject != gameObject && otherObject.layer == gameObject.layer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsGameObjectGround(collision.gameObject))
        {
            //Debug.Log($"Touched ground at {collision.gameObject.name}, {collision.gameObject.layer}");
            //GetComponent<Animator>().SetBool("groundTouch", true); //will be important if we have jump animation.
            touchesGround = true;
            col.sharedMaterial.friction = 0.6f;
            ground = collision.gameObject;
            col.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsGameObjectGround(collision.gameObject))
        {
            //Debug.Log($"Left ground at {collision.gameObject.name}, {collision.gameObject.layer}");

            touchesGround = false;
            //GetComponent<Animator>().SetBool("groundTouch", false); //will be important if we have jump animation.
            col.sharedMaterial.friction = 0f;
        }

    }
}

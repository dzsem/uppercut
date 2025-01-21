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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.DownArrow) || (Input.GetKeyDown(KeyCode.S)) && ground.tag == "openFloor" && touchesGround))
        {
            col.isTrigger = true;

        }
        if (Input.GetKeyDown(KeyCode.Space) && touchesGround)
        {
            rb.linearVelocityY = 0f;
            rb.AddForce(Vector2.up * jumpForce);
            touchesGround = false;
        }
        rb.linearVelocityX = Input.GetAxis("Horizontal") * walksSpeed;
        if (rb.linearVelocityX < 0f)
        {

            GetComponent<SpriteRenderer>().flipX = true;
        }
        if (rb.linearVelocityX > 0f)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        if (Mathf.Abs(rb.linearVelocityX) > 0f)
        {
            GetComponent<Animator>().SetBool("isWalking", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("isWalking", false);
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

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    private enum EActionStatus { AVAILABLE, PERFORMING, COOLDOWN };
    public enum EAttackType { UPPERCUT, PUNCH, SMASH, NONE };

    [Header("Other Player Components")]
    public BoxCollider2D col;
    public Rigidbody2D rb;
    public BoxCollider2D dashPunchBox;

    [Header("Walking and jumping")]
    public GameObject ground;
    public bool touchesGround;
    public float jumpForce;
    public float walksSpeed;

    /// <summary>
    /// A Friction nevű material súrlódása állítódik erre az értékre, amikor a játékos a földön van.
    /// </summary>
    public float groundFriction;

    /// <summary>
    /// A Friction nevű material súrlódása állítódik erre az értékre, amikor a játékos a levegőben van.
    /// Ez valamilyen platform oldalába beakadós bugot küszöböl ki (kérdezd Miklóst) ~Tamás
    /// </summary>
    public float airFriction;

    [Header("Dash")]
    public float dashDuration;
    public float dashCooldown;
    public float dashForce;

    [Header("Belső State")]
    [SerializeField]
    private EActionStatus _dashStatus = EActionStatus.AVAILABLE;

    [SerializeField]
    private bool _uppercutActive = false;

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
        ProcessInput();

        UpdateMovementStatus();

        UpdateAnimation();
    }

    /// <summary>
    /// Visszaadja, hogy milyen típusú támadást végez jelenleg a player.
    /// Ha semmilyet, akkor EAttackType.NONE.
    /// Ha azt ellenőrzöd, hogy támad-e jelenleg a player, akkor használd az IsAttacking() függvényt.
    /// </summary>
    public EAttackType GetAttackType()
    {
        // prioritások is vannak: uppercut > smash > punch

        if (_uppercutActive)
            return EAttackType.UPPERCUT;

        // TODO: smash attack

        if (_dashStatus == EActionStatus.PERFORMING)
            return EAttackType.PUNCH;

        return EAttackType.NONE;
    }

    /// <summary>
    /// Visszaadja, hogy végez-e valamilyen attack-et jelenleg a játékos. (convenience fn.)
    /// Részletesebben, hogy mit végez pontosan, használd a GetAttackType() függvényt.
    /// </summary>
    public bool IsAttacking()
    {
        return GetAttackType() != EAttackType.NONE;
    }

    /// <summary>
    /// A dash 3 fázisát (aktív - cooldown - kész) kódolja. Ez egy korutin, azaz külön fut a fő kódtól.
    /// Használat: (MonoBehaviour.)StartCoroutine(DoDash())
    /// TODO: működik web buildben?
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoDash()
    {
        _dashStatus = EActionStatus.PERFORMING;

        yield return new WaitForSeconds(dashDuration);

        _dashStatus = EActionStatus.COOLDOWN;

        yield return new WaitForSeconds(dashCooldown);

        _dashStatus = EActionStatus.AVAILABLE;
    }

    private void ProcessInput()
    {
        bool hasHorizontalInput = Math.Abs(Input.GetAxis("Horizontal")) > 0.001f;

        // FALLTHROUGH //
        // down-ra vagy s-re átesik, ha földön van
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && ground.tag == "openFloor" && touchesGround)
        {
            col.isTrigger = true;
        }

        // Horizontal movement + dash //
        if (hasHorizontalInput)
        {
            // TODO: valamilyen input manageren keresztül kezeljük a bindokat
            if (Input.GetKeyDown(KeyCode.X) && _dashStatus == EActionStatus.AVAILABLE)
            {
                rb.AddForce(Vector2.right * dashForce * Math.Sign(Input.GetAxis("Horizontal")));
                StartCoroutine(DoDash());
            }
            else
            {
                rb.AddForce(Vector2.right * walksSpeed * Input.GetAxis("Horizontal"));
            }
        }

        // Uppercut //
        // space-re uppercut-ol, ha földön van és nyomod a fel inputot
        if (Input.GetKeyDown(KeyCode.Space) && touchesGround)
        {
            rb.linearVelocityY = 0f;
            rb.AddForce(Vector2.up * jumpForce);
            touchesGround = false;
            _uppercutActive = true;
            GetComponent<Animator>().SetBool("isPerformingUppercut", true);
        }

        // ha nincs aktív dash, és vízszintes input sincs, és földön vagyunk, megállítjuk a játékost
        // ez akadályozza meg hogy túl "csúszós" legyen a control ~Tamás
        if (!hasHorizontalInput && touchesGround && _dashStatus != EActionStatus.PERFORMING)
        {
            rb.linearVelocityX = 0;
        }
    }

    /// <summary>
    /// Frissíti a belső mozgással kapcsolatos változókat és a megjelenést a physics engine alapján.
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
        if (_uppercutActive && (rb.linearVelocityY < 0f || touchesGround))
        {
            _uppercutActive = false;
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
            col.sharedMaterial.friction = groundFriction;
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
            col.sharedMaterial.friction = airFriction;
        }

    }
}

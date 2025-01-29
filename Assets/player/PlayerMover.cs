using System;
using System.Collections;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public enum EAttackType { UPPERCUT, PUNCH, SMASH, NONE };

    [Header("Other Player Components")]
    public BoxCollider2D col;
    public Rigidbody2D rb;
    public BoxCollider2D dashPunchBox;
    public BoxCollider2D uppercutPunchBox;
    public CostumeTrigger groundTouchTrigger;

    [Header("Misc beállítások")]
    public bool disableInput = false;

    [Header("Walking and jumping")]
    public GameObject ground;

    [SerializeField] private int _groundCollidersTouched = 0;

    private int GroundCollidersTouched
    {
        get => _groundCollidersTouched;
        set
        {
            bool newTouchesGround = value > 0;

            if (newTouchesGround != TouchesGround)
            {
                _touchesGround = newTouchesGround;
                GetComponent<Animator>().SetBool("groundTouch", TouchesGround);
            }

            _groundCollidersTouched = value;
        }
    }

    [SerializeField] private bool _touchesGround = false;

    public bool TouchesGround
    {
        get => _touchesGround;
    }

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

    [Header("Uppercut")]
    public float jumpForce;

    /// <summary>
    /// Az a függőleges sebesség, ami alatt az uppercut-nak vége lesz
    /// (amikor már ennél a sebességnél lassabban halad *felfelé*)
    /// </summary>
    public float uppercutVelocityThreshold;

    /// <summary>
    /// Az az időtartam másodpercben, ameddig az uppercut legalább tart.
    /// (velocityThreshold érzékelését segíti, mert van gyorsulási fázis is)
    /// </summary>
    public float uppercutMinimalDuration;

    [Header("Dash")]
    public float dashDuration;
    public float dashCooldown;
    public float dashForce;

    [Header("Punchdown")]
    public float punchdownForce;


    [Header("Belső State")]
    [SerializeField] private bool _dashRefreshed = false;
    [SerializeField] private bool _uppercutRefreshed = false;
    [SerializeReference] private AnimatedActionStatus _dashStatus = new AnimatedActionStatus("isPerformingDash");
    [SerializeReference] private AnimatedActionStatus _punchdownStatus = new AnimatedActionStatus("isPerformingPunchdown");
    [SerializeReference] private AnimatedActionStatus _uppercutStatus = new AnimatedActionStatus("isPerformingUppercut");

    /// <summary>
    /// Azt tárolja, melyik vízszintes irányba néz a player. -1: balra, 1: jobbra, 0: nem def.
    /// Ez alapján lesz meghatározva a dash iránya.
    /// </summary>
    [SerializeField][Range(-1, 1)] private int _facingDirection = 1;


    // CACHELT VÁLTOZÓK //
    private float _dashPunchBoxOffsetX;
    private float _uppercutPunchBoxOffsetY;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _dashPunchBoxOffsetX = dashPunchBox.offset.x;
        _uppercutPunchBoxOffsetY = uppercutPunchBox.offset.y;

        _dashStatus.animator = GetComponent<Animator>();
        _punchdownStatus.animator = GetComponent<Animator>();
        _uppercutStatus.animator = GetComponent<Animator>();

        groundTouchTrigger.EnterTrigger += OnGroundEnter;
        groundTouchTrigger.ExitTrigger += OnGroundExit;
    }

    // Update is called once per frame
    void Update()
    {
        if (!disableInput)
        {
            ProcessInput();
        }

        UpdateMovementStatus();

        UpdateColliders();

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

        if (_uppercutStatus.IsPerforming())
            return EAttackType.UPPERCUT;

        if (_punchdownStatus.IsPerforming())
            return EAttackType.SMASH;

        if (_dashStatus.IsPerforming())
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

    public void RefreshUppercut()
    {
        if (_uppercutStatus.Status == EActionStatus.READY_TO_REFRESH)
            _uppercutStatus.Status = EActionStatus.AVAILABLE;
        else if (_uppercutStatus.Status != EActionStatus.AVAILABLE)
            _uppercutRefreshed = true;
    }

    public void RefreshDash()
    {
        if (_dashStatus.Status == EActionStatus.READY_TO_REFRESH)
            _dashStatus.Status = EActionStatus.AVAILABLE;
        else if (_dashStatus.Status != EActionStatus.AVAILABLE)
            _dashRefreshed = true;
    }

    public void RefreshMovementAbilities()
    {
        RefreshUppercut();
        RefreshDash();

        if (_punchdownStatus.IsPerforming() && (rb.linearVelocityY > 0f || TouchesGround))
        {
            _punchdownStatus.Status = EActionStatus.AVAILABLE;
        }
    }

    private bool CanDash()
    {
        return _dashStatus.Status == EActionStatus.AVAILABLE
            && !_uppercutStatus.IsPerforming()
            && !_punchdownStatus.IsPerforming();
    }

    private bool CanPunchDown()
    {
        return _punchdownStatus.Status == EActionStatus.AVAILABLE
            && !_uppercutStatus.IsPerforming()
            && !_dashStatus.IsPerforming();
    }

    /// <summary>
    /// A dash 3 fázisát (aktív - cooldown - kész) kódolja. Ez egy korutin, azaz külön fut a fő kódtól.
    /// Használat: (MonoBehaviour.)StartCoroutine(DoDash())
    /// TODO: működik web buildben?
    /// </summary>
    private IEnumerator DoDash()
    {
        _dashStatus.Status = EActionStatus.PERFORMING;

        yield return new WaitForSeconds(dashDuration);

        _dashStatus.Status = EActionStatus.COOLDOWN;

        yield return new WaitForSeconds(dashCooldown);

        if (!_dashRefreshed)
        {
            // akkor lesz újra available, ha meghívjuk a RefreshMovementAbilities()-t
            _dashStatus.Status = EActionStatus.READY_TO_REFRESH;
        }
        else
        {
            _dashStatus.Status = EActionStatus.AVAILABLE;
            _dashRefreshed = false;
        }
    }

    private IEnumerator DoUppercut()
    {
        _uppercutStatus.Status = EActionStatus.PERFORMING;

        yield return new WaitForSeconds(uppercutMinimalDuration);

        _uppercutStatus.Status = EActionStatus.READY_TO_REFRESH;

        if (!_uppercutRefreshed)
        {
            // akkor lesz újra available, ha meghívjuk a RefreshMovementAbilities()-t
            _uppercutStatus.Status = EActionStatus.READY_TO_REFRESH;
        }
        else
        {
            _uppercutStatus.Status = EActionStatus.AVAILABLE;
            _uppercutRefreshed = false;
        }
    }

    private void ProcessVerticalInput()
    {
        // BUG: egyszerre két GetKeyDown() nem működik, egyre égetőbb az input manager
        // nem működik de ugyanaz: Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)
        // ~Tamás
        if (Input.GetAxis("Vertical") < 0f)
        {
            // FALLTHROUGH //
            // down-ra vagy s-re átesik, ha földön van
            if (ground.tag == "openFloor" && TouchesGround)
            {
                col.isTrigger = true;
            }

            // Punchdown //
            if (Input.GetKeyDown(KeyCode.X) && !TouchesGround
                && CanPunchDown())
            {
                rb.linearVelocityY = 0.0f;
                rb.AddForce(Vector2.down * punchdownForce);

                _punchdownStatus.Status = EActionStatus.PERFORMING;
            }
        }


        // Uppercut //
        // space-re uppercut-ol, ha földön van és nyomod a fel inputot
        if (Input.GetKeyDown(KeyCode.Space) && TouchesGround)
        {
            rb.linearVelocityY = 0f;
            rb.AddForce(Vector2.up * jumpForce);

            StartCoroutine(DoUppercut());
        }

        // szűnjön meg az uppercut animáció, amikor elkezdünk esni, vagy földet érünk
        if (_uppercutStatus.Status != EActionStatus.PERFORMING
            && ((rb.linearVelocityY < uppercutVelocityThreshold) || TouchesGround))
        {
            RefreshUppercut();
        }
    }

    private void ProcessHorizontalInput()
    {
        bool hasHorizontalInput = Math.Abs(Input.GetAxis("Horizontal")) > 0.001f;

        // Horizontal movement + dash //
        if (hasHorizontalInput)
        {
            rb.AddForce(Vector2.right * walksSpeed * Input.GetAxis("Horizontal"));

            // 0 kizárása
            _facingDirection = Math.Sign(rb.linearVelocityX) == (-1) ? (-1) : 1;
        }

        if (Input.GetKeyDown(KeyCode.X) && CanDash())
        {
            rb.AddForce(Vector2.right * dashForce * _facingDirection);
            StartCoroutine(DoDash());
        }

        // ha nincs aktív dash, és vízszintes input sincs, és földön vagyunk, megállítjuk a játékost
        // ez akadályozza meg hogy túl "csúszós" legyen a control ~Tamás
        if (!hasHorizontalInput && TouchesGround && !_dashStatus.IsPerforming())
        {
            rb.linearVelocityX = 0;
        }
    }

    private void ProcessInput()
    {
        // WARN: sorrend fontos, nem lehet punchdown attackot csinálni ha a horizontal
        //       processing a vertical előtt van (dash előbb elindul mint ahogy punchdown-olni tudnánk)

        ProcessVerticalInput();
        ProcessHorizontalInput();

        if (TouchesGround)
        {
            RefreshMovementAbilities();
        }
    }

    private void UpdateColliders()
    {
        // NOTE: ezt nekem kell cachelni? van olyan okos az engine hogy rájön ha nincs változás?
        dashPunchBox.gameObject.SetActive(_dashStatus.IsPerforming());
        uppercutPunchBox.gameObject.SetActive(_uppercutStatus.IsPerforming() || _punchdownStatus.IsPerforming());
    }

    /// <summary>
    /// Frissíti a belső mozgással kapcsolatos változókat és a megjelenést a physics engine alapján.
    /// </summary>
    private void UpdateMovementStatus()
    {
        // GetComponent<SpriteRenderer>().flipX = (_facingDirection == (-1)) helyett:
        // (nem tudom miért kell, Ákos fix) ~Tamás
        if (_facingDirection == (-1))
        {
            GetComponent<SpriteRenderer>().transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            GetComponent<SpriteRenderer>().transform.rotation = Quaternion.Euler(0, 0, 0);
        }


        dashPunchBox.offset = new Vector2(
            _facingDirection * _dashPunchBoxOffsetX,
            dashPunchBox.offset.y
        );

        int uppercutBoxDirection = _punchdownStatus.IsPerforming() ? (-1) : 1;

        uppercutPunchBox.offset = new Vector2(
            uppercutPunchBox.offset.x,
            uppercutBoxDirection * _uppercutPunchBoxOffsetY
        );
    }

    /// <summary>
    /// Frissíti az animátor állapotát.
    /// </summary>
    private void UpdateAnimation()
    {
        // isWalking igaz lesz, ha a vízszintes sebesség nem nulla
        GetComponent<Animator>().SetBool("isWalking", (Mathf.Abs(rb.linearVelocityX) > 0f));

        // NOTE: a többi frissítést az AnimatedActionStatus-ok végzik automatikusan
    }

    /// <summary>
    /// Megállapítja, hogy otherObject lehet-e platform. Mivel a punchboxok ugyanabban a gameobjecten belül
    /// vannak, és nincsen felettük rigidbody, ezért összevonódnak; ekkor minden megütött objektum a 3-as (Hitbox)
    /// layeren belül lesz. Ezeket ki kell zárni:(
    /// 
    /// Update: már nem biztos hogy vannak layer problémák, külön hitbox trigger van
    /// </summary>
    /// <param name="otherObject">A collision-on belüli másik gameobject.</param>
    /// <returns></returns>
    private bool IsGameObjectGround(GameObject otherObject)
    {
        return otherObject != gameObject && otherObject.layer == gameObject.layer && otherObject.tag != "enemy";
    }

    // A GroundCheckHitbox CustomTrigger-éhez kapcsolódik. Lásd: Start()
    private void OnGroundEnter(Collider2D collision)
    {
        if (!IsGameObjectGround(collision.gameObject))
            return;

        // Debug.Log($"Touched ground at {collision.gameObject.name}, {collision.gameObject.layer}")

        GroundCollidersTouched += 1;
        col.sharedMaterial.friction = groundFriction;
        ground = collision.gameObject;
    }

    // A GroundCheckHitbox CustomTrigger-éhez kapcsolódik. Lásd: Start()
    private void OnGroundExit(Collider2D collision)
    {
        if (!IsGameObjectGround(collision.gameObject))
            return;

        // Debug.Log($"Left ground at {collision.gameObject.name}, {collision.gameObject.layer}")

        GroundCollidersTouched -= 1;
        col.sharedMaterial.friction = airFriction;
    }
}

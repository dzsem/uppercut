using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Misc")]
    public int hitboxLayerID = 3;

    [Header("Other Player Components")]
    public Rigidbody2D rb;
    public PlayerMover playerMover;

    [Header("HP/Max HP")]
    [SerializeField] private int _health;
    public int maxHealth;
    public float invulnerabilityDuration;
    public bool forceInvulnerability;

    [Header("Knockback")]
    public float knockbackForce;
    public float knockbackResistanceStrength;

    [Header("GameState eventek")]
    public UnityEvent deathEvent;

    public UnityEvent<int> damageEvent;

    [Header("Belső state")]
    [SerializeField] private bool _isInvulnerableByDmg = false;
    [SerializeField] private int _invulnerabilityCountByDmg = 0;


    public bool IsInvulnerable
    {
        get => _isInvulnerableByDmg || forceInvulnerability;
    }

    public int Health
    {
        get => _health;
        set
        {
            bool healthDecreased = value < _health;
            _health = value;

            if (healthDecreased)
            {
                damageEvent?.Invoke(_health);
            }

            if (_health <= 0)
            {
                deathEvent?.Invoke();
            }
        }
    }

    public void KnockbackInDirection(Vector2 dir, float strength)
    {
        float strengthMultiplier = Math.Max(0.0f, strength - knockbackResistanceStrength);

        if (strengthMultiplier > 0.0f)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(dir * knockbackForce * strengthMultiplier);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator DoInvulnerability()
    {
        // könyörgök legyél alapból atomi
        // nagyon fájna ha ez lenne bugos
        _invulnerabilityCountByDmg += 1;
        _isInvulnerableByDmg = true;

        yield return new WaitForSeconds(invulnerabilityDuration);

        _invulnerabilityCountByDmg -= 1;

        if (_invulnerabilityCountByDmg <= 0)
            _isInvulnerableByDmg = false;
    }

    private bool IsGameObjectHarmful(GameObject otherGameObject)
    {
        return otherGameObject != gameObject
            && otherGameObject.tag == "attackbox"
            && otherGameObject.layer == hitboxLayerID;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsGameObjectHarmful(collision.gameObject) && !playerMover.IsAttacking())
        {
            Attackable attackable = collision.gameObject.GetComponent<Attackable>()
                ?? collision.gameObject.GetComponentInParent<Attackable>();

            if (!attackable)
            {
                Debug.LogWarning($"Player collision \"{collision.gameObject.name}\" objektummal; amin nincs (és a szülőjén sincs) Attackable komponens.");
            }

            Vector2 dir = (gameObject.transform.position - collision.gameObject.transform.position).normalized;
            if (!IsInvulnerable)
            {
                Health -= attackable?.touchDamage ?? 0;
                KnockbackInDirection(dir, attackable?.knockbackStrength ?? 0.0f);
            }

            StartCoroutine(DoInvulnerability());
        }
    }
}

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

    /// <summary>
    /// A sebezhetetlenség időtartama, miután nekimész egy enemy-nek.
    /// </summary>
    public float invulnerabilityDuration;

    /// <summary>
    /// Programmatikusan beállítható biztosított invuln.
    /// </summary>
    public bool forceInvulnerability;

    [Header("Knockback")]

    /// <summary>
    /// Az 1-es erősségű knockback ereje. Kiszámításhoz lásd: README.md#Knockback
    /// </summary>
    public float knockbackForce;

    /// <summary>
    /// A jelenlegi knockback resistance értéke. Lásd: README.md#Knockback
    /// </summary>
    public float knockbackResistanceStrength;

    [Range(0.0f, 1.0f)]
    /// <summary>
    /// A legkisebb lehetséges knockback szorzó, ami a strength-knockbackResistance számításból kijöhet.
    /// Az ebből adódó erő ||knockbackForce|| * minimalKnockbackStrength nagyságú
    /// </summary>
    public float minimalKnockbackStrength;

    [Header("GameState eventek")]
    public UnityEvent onDeath;

    public UnityEvent<int> onDamage;

    [Header("Belső state")]
    [SerializeField] private bool _isInvulnerableByDmg = false;
    [SerializeField] private int _invulnerabilityCountByDmg = 0;

    /// <summary>
    /// Megadja, hogy akármilyen okból sebezhetetlen-e a player.
    /// </summary>
    public bool IsInvulnerable
    {
        get => _isInvulnerableByDmg || forceInvulnerability;
    }

    /// <summary>
    /// Visszaadja a játékos jelenlegi életét. Átállításkor az eventeket is elsüti, ha szükséges.
    /// </summary>
    public int Health
    {
        get => _health;
        set
        {
            int healthDiff = _health - value;
            bool healthDecreased = healthDiff > 0;
            _health = value;

            if (healthDecreased)
            {
                onDamage?.Invoke(healthDiff);
            }

            if (_health <= 0)
            {
                onDeath?.Invoke();
            }
        }
    }

    /// <summary>
    /// Visszalöki a játékost a megadott irányba.
    /// </summary>
    public void KnockbackInDirection(Vector2 dir, float strength)
    {
        float strengthMultiplier = Math.Max(minimalKnockbackStrength, strength - knockbackResistanceStrength);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce * strengthMultiplier);
    }

    /// <summary>
    /// Elindít egy invuln számlálót, amég megy, biztosan invulnerable lesz a játékos.
    /// </summary>
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

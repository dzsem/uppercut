using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    [Header("HP Vignette shader cuccok")]
    public ScriptableRendererFeature shader;
    public Material shaderMaterial;
    public const float vignetteIntensityMax = 1.4f;
    public const float vignetteIntensityStart = 0.4f;
    public const float vignettePowerStart = 5.3f;
    public const float vignettePowerMin = 0.3f;
    public const float vignetteSmoothing = 0.5f;
    private float _intensityStep;
    private float _powerStep;
    private int _vignetteIntensity = Shader.PropertyToID("_vignetteIntensity");
    private int _vignettePower = Shader.PropertyToID("_vignettePower");

    public void Start() {
        // _intensityStep = vignetteIntensityMax / (maxHealth);
        // _powerStep = vignettePowerMax / (maxHealth * 3f);
        _intensityStep = 0.3f;
        _powerStep = 1f;
        Debug.Log("intensity step: " + _intensityStep);
        Debug.Log("power step: " + _powerStep);

        shaderMaterial.SetFloat(_vignetteIntensity, vignetteIntensityStart);
        shaderMaterial.SetFloat(_vignettePower, vignettePowerStart);
        shader.SetActive(true);
        onDeath.AddListener(OnDeathCallback);
        onDamage.AddListener(OnDamageCallback);
    }

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
            Debug.Log($"Collision {collision.gameObject.name}");
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

    public void OnDeathCallback()
    {
        playerMover.disableInput = true;
    }

    public void OnDamageCallback(int hp)
    {
        Debug.Log("OnDamageCallback called");
        StartCoroutine(UpdateVignetteEffect());
        FindFirstObjectByType<MainCamera>().ShakeCamera(0.1f, 1f);
    }

    private IEnumerator UpdateVignetteEffect() {
        float intensity = shaderMaterial.GetFloat(_vignetteIntensity);
        float power = shaderMaterial.GetFloat(_vignettePower);
        
        float targetIntensity = Mathf.Clamp(intensity + _intensityStep, 0f, vignetteIntensityMax);
        float targetPower = Mathf.Clamp(power - _powerStep, vignettePowerMin, vignettePowerStart);
        
        float elapsed = 0f;

        while (elapsed < vignetteSmoothing) {
            elapsed += Time.deltaTime;
            float t = elapsed / vignetteSmoothing;

            intensity = Mathf.Lerp(intensity, targetIntensity, t);
            power = Mathf.Lerp(power, targetPower, t);

            shaderMaterial.SetFloat(_vignetteIntensity, intensity);
            shaderMaterial.SetFloat(_vignettePower, power);

            Debug.Log("Lerping: Intensity=" + intensity + " Power=" + power);
            yield return null;
        }

        shaderMaterial.SetFloat(_vignetteIntensity, targetIntensity);
        shaderMaterial.SetFloat(_vignettePower, targetPower);
    }
}

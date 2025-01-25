using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class Attackable : MonoBehaviour
{
    public CostumeTrigger solidRangetrigger;
    public Collider2DTrigger attackableRangetrigger;


    public UnityEvent<int> onHit;

    [Header("Player elleni knockback + damage")]
    /// <summary>
    /// Leírja a knockback erősségét abban az esetben, ha a player érint egy attackbox-ot
    /// Lásd: Player/README.md#Knockback
    /// </summary>
    public float knockbackStrength;

    /// <summary>
    /// A sebzés mértéke, amit a player kap, ha nekisétál egy attackbox-nak
    /// </summary>
    public int touchDamage;

    void Awake()
    {
        attackableRangetrigger.enterTrigger += OnAttackableRangeTriggerEntered;
        attackableRangetrigger.exitTrigger += OnAttackableRangeTriggerExited;
    }

    private bool IsGameObjectHarmful(GameObject otherGameObject)
    {
        return gameObject != otherGameObject
            && otherGameObject.tag == "attackbox";
        // && otherGameObject.layer == 3
        // a trigger szerkezetéből adódóan lehetetlen, hogy ne HitboxLayerről kerüljön ki a collision
    }

    void OnAttackableRangeTriggerEntered(Collider2D other)
    {
        if (IsGameObjectHarmful(other.gameObject))
        {
            Attacker attacker = other.gameObject.GetComponent<Attacker>()
                ?? other.gameObject.GetComponentInParent<Attacker>();

            int attackDamage = attacker?.attackDamage ?? 0;

            // TODO: log törlése
            Debug.Log($"Attackable \"{gameObject.name}\" hit by: \"{other.gameObject.name}\" for {attackDamage} dmg.");
            onHit?.Invoke(attackDamage);
        }
    }

    void OnAttackableRangeTriggerExited(Collider2D other)
    {
        if (IsGameObjectHarmful(other.gameObject))
        {
            // nothing
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
}

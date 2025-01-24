using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class Attackable : MonoBehaviour
{
    public CostumeTrigger solidRangetrigger;
    public CostumeTrigger attackableRangetrigger;


    public UnityEvent<int> OnHit;

    [Header("Player elleni knockback + damage")]
    public float knockbackStrength;
    public int touchDamage;

    void Awake()
    {
        attackableRangetrigger.EnterTrigger += OnAttackableRangetriggerEntered;
        attackableRangetrigger.ExitTrigger += OnAttackableRangetriggerExited;
    }

    void OnAttackableRangetriggerEntered(Collider other)
    {
    }

    void OnAttackableRangetriggerExited(Collider other)
    {
        Debug.Log("Attackable exit");
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

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

    void Awake()
    {
        attackableRangetrigger.EnterTrigger += OnAttackableRangetriggerEntered;
        attackableRangetrigger.ExitTrigger += OnAttackableRangetriggerExited;
    }

    void OnAttackableRangetriggerEntered(Collider2D other)
    {
    }

    void OnAttackableRangetriggerExited(Collider2D other)
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

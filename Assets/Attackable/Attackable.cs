using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class AttackableComponent : MonoBehaviour
{
    public Trigger solidRangetrigger;
    public Trigger attackableRangetrigger;


    public UnityEvent OnHit;

    void Awake()
    {
        attackableRangetrigger.EnterTrigger += OnAttackableRangetriggerEntered;
        attackableRangetrigger.ExitTrigger += OnAttackableRangetriggerExited;

    }

    void OnAttackableRangetriggerEntered(Collider other)
    {
        OnHit.Invoke();
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

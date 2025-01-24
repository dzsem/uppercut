using System;
using UnityEngine;
using UnityEngine.Events;
using Vector2 = System.Numerics.Vector2;

public class EnemyMovements : MonoBehaviour
{

    public CostumeTrigger startingPointTrigger;
    public CostumeTrigger endingPointTrigger;

    private GameObject pointStarting;
    private GameObject pointEnding;

    private Rigidbody2D rb;

    private bool playerInRange=false;
    private UnityEngine.Vector2 direction;
    public float movementSpeed = 10f;

    //public event Action OnDeath;
    public event Action onTurningToEnd;
    public event Action onTurningToStart;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        onTurningToStart += TurningHandlerToStart;
        onTurningToEnd += TurningHandlerToEnd;
        startingPointTrigger.EnterTrigger += EnteredIntoEnd;
        endingPointTrigger.EnterTrigger +=  EnteredIntoStart;
        pointStarting=startingPointTrigger.gameObject;
        pointEnding=endingPointTrigger.gameObject;
        rb=GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInRange)
        {
            rb.MovePosition(rb.position + direction * (movementSpeed * Time.deltaTime));
        }
    }

    void EnteredIntoStart(Collider collider)
    {
        onTurningToEnd?.Invoke();
    }

    void EnteredIntoEnd(Collider collider)
    {
        onTurningToStart?.Invoke();
    }

    void TurningHandlerToStart()
    {
        direction=pointStarting.transform.position - transform.position;
    }

    void TurningHandlerToEnd()
    {
        direction=pointEnding.transform.position - transform.position;

    }

}

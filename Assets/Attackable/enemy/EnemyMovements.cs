using System;
using UnityEngine;

public class EnemyMovements : MonoBehaviour
{

    public CostumeTrigger startingPointTrigger;
    public CostumeTrigger endingPointTrigger;

    private GameObject pointStarting;
    private GameObject pointEnding;

    [SerializeField]private Rigidbody2D rb;

    private bool playerInRange=false;
    [SerializeField]private Vector2 direction;
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
        direction = pointEnding.transform.position;
    }

    void Update()
    {
        if (!playerInRange)
        {
            Vector2.MoveTowards(this.transform.position.normalized, pointEnding.transform.position.normalized, movementSpeed * Time.deltaTime);
        }
    }

    void EnteredIntoStart(Collider collider) => onTurningToEnd?.Invoke();

    void EnteredIntoEnd(Collider collider) => onTurningToStart?.Invoke();

    void TurningHandlerToStart() => direction = pointStarting.transform.position;

    void TurningHandlerToEnd() => direction = pointEnding.transform.position;
}

using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovements : MonoBehaviour
{


    public CostumeTrigger hitboxTrigger;

    public GameObject pointStarting;
    public GameObject pointEnding;

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


        //startingPointTrigger.EnterTrigger += EnteredIntoEnd;
        //endingPointTrigger.EnterTrigger +=  EnteredIntoStart;
        hitboxTrigger.EnterTrigger += TurningHandler;


        pointStarting.transform.position.Set(pointStarting.transform.position.x, this.transform.position.y, pointStarting.transform.position.z);
        pointEnding.transform.position.Set(pointEnding.transform.position.x, this.transform.position.y, pointEnding.transform.position.z);


        direction = pointEnding.transform.position;
    }

    void Update()
    {
        if (!playerInRange)
        {
            // Convert transform.position to Vector2 for MoveTowards
            Vector2 currentPosition = transform.position;
            Vector2 targetPosition = direction;

            // Move towards the direction
            Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, movementSpeed * Time.deltaTime);

            // Convert back to Vector3 and update transform.position
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

        }
    }




    void TurningHandler(Collider2D collider)
    {
        if (!direction.Equals(pointEnding.transform.position))
        {
            onTurningToStart?.Invoke();
        }
        else
        {
            onTurningToEnd?.Invoke();
        }
    }

    void TurningHandlerToStart() => direction = pointStarting.transform.position;

    void TurningHandlerToEnd() => direction = pointEnding.transform.position;
}

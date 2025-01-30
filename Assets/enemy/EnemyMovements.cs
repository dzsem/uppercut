using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovements : MonoBehaviour
{


    public CostumeTrigger hitboxTrigger;

    public GameObject pointStarting;
    public GameObject pointEnding;

    [SerializeField]protected Rigidbody2D rb;

    protected bool playerInRange=false;
    [SerializeField]protected Vector2 direction;
    public float movementSpeed = 10f;

    //public event Action OnDeath;
    public event Action onTurningToEnd;
    public event Action onTurningToStart;

    //private float timeOfCollison = 0f;
    //private bool isWithinOneSecOfCollison=false;

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
        virtualStart();
    }

    protected virtual void virtualStart() { }

    void Update()
    {
        //if (isWithinOneSecOfCollison)
        //{
        //    if (Time.time - timeOfCollison < 1f)
        //    {
        //        transform.rotation = Quaternion.identity;
        //    }
        //    else
        //    {
        //        isWithinOneSecOfCollison = false;
        //    }
        //}
        transform.rotation = Quaternion.identity;

        CostumVirtualMovementUpdate();
    }

    protected virtual void CostumVirtualMovementUpdate()
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

    protected virtual void SetDirection() { return;}

    protected void TurningHandler(Collider2D collider)
    {

        if (collider.gameObject.Equals(pointEnding) || collider.gameObject.Equals(pointStarting))
        {Debug.Log(collider.gameObject.name);
            if (direction.x.Equals(pointEnding.transform.position.x))
            {
                onTurningToStart?.Invoke();
            }
            else
            {
                onTurningToEnd?.Invoke();
            }
            SetDirection();
        }
    }

    void TurningHandlerToStart() => direction = new Vector2 (pointStarting.transform.position.x,this.transform.position.y);

    void TurningHandlerToEnd() => direction =new Vector2 (pointEnding.transform.position.x,this.transform.position.y);

    private void OnDrawGizmos()
    {
        CircleCollider2D endCollider = pointEnding.GetComponent<CircleCollider2D>();
        CircleCollider2D startCollider = pointStarting.GetComponent<CircleCollider2D>();
        Gizmos.DrawWireSphere(pointEnding.transform.position, endCollider.radius);
        Gizmos.DrawWireSphere(pointStarting.transform.position, startCollider.radius);
        Gizmos.DrawLine(pointEnding.transform.position, pointStarting.transform.position);
    }
}

using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class fignerMover : MonoBehaviour
{
    bool shouldMove = false;
    public float maxDistance = 5;
    public GameObject body;
    public Vector3 elevationOffset;
    public float speed = 5;
    private float _directionalOffset;
    public float forwardOffset = -2;
    public float backOffset = 1;
    public RaycastHit2D firstHit;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        firstHit = Physics2D.Raycast((body.transform.position + elevationOffset), Vector2.down);


        if (Mathf.Sign(body.GetComponent<Rigidbody2D>().linearVelocityX) >= 0)
        {
            _directionalOffset = forwardOffset;
        }
        else
        {
            _directionalOffset = backOffset;
        }
        if (Mathf.Abs(transform.position.x - (body.transform.position.x + elevationOffset.x + _directionalOffset)) >= maxDistance)
        {
            shouldMove = true;
        }
        if (Vector3.Distance(transform.position, body.transform.position + elevationOffset + new Vector3(_directionalOffset, 0, 0)) < 0.1)
        {
            shouldMove = false;
        }
        if (shouldMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, body.transform.position + elevationOffset + new Vector3(_directionalOffset, 0, 0), speed *Time.deltaTime);
        }
        else
        {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, Vector2.down);
         
            
            if (hit.distance > 0.1)
            {
                transform.position = Vector2.MoveTowards(transform.position, hit.point, speed * Time.deltaTime);
                
            }
           
        }
       
        

    }
}

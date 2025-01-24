using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class fignerMover : MonoBehaviour
{
    bool shouldMove = false;
    public float maxDistance = 5;
    public GameObject body;
    public Vector3 elevationOffset;
    public float speed = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.x - (body.transform.position.x + elevationOffset.x)) >= maxDistance)
        {
            shouldMove = true;
        }
        if (Vector3.Distance(transform.position, body.transform.position + elevationOffset) < 0.5)
        {
            shouldMove = false;
        }
        if (shouldMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, body.transform.position + elevationOffset, speed *Time.deltaTime);
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down);
            hit = Physics2D.Raycast(transform.position, Vector2.down);
            if (hit.distance > 0.1)
            {
                transform.position = Vector2.MoveTowards(transform.position, hit.point, speed * Time.deltaTime);
            }
           
        }
       
        

    }
}

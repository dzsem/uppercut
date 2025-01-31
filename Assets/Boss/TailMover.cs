using UnityEngine;

public class TailMover : MonoBehaviour
{
    public GameObject player;
    public GameObject body;
    public float tailHitSpeed = 2;
    bool _shouldMove = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Mathf.Abs(transform.position.x - player.transform.position.x) < 4 && player.transform.position.y - transform.position.y > 0.5)
        {
            if (_shouldMove)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, tailHitSpeed * Time.deltaTime);
            }
            else
            {
                if (Vector2.Distance(transform.position, body.transform.position + new Vector3(3.8499999f, 2.32999992f)) > 0.01)
                {
                    transform.position = Vector2.MoveTowards(transform.position, body.transform.position + new Vector3(3.8499999f, 2.32999992f, -0.047343079f), 5 * Time.deltaTime);
                }
                else
                {
                    _shouldMove = true;
                }
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, body.transform.position + new Vector3(3.8499999f, 2.32999992f))> 0.01)
            {
                transform.position = Vector2.MoveTowards(transform.position, body.transform.position + new Vector3(3.8499999f, 2.32999992f, -0.047343079f), 5 * Time.deltaTime);
            }
            
            
        }  
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            
            _shouldMove = false;
        }
    }
}

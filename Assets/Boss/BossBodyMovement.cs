using UnityEngine;

public class BossBodyMovement : MonoBehaviour
{
    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector2.down);
        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2((player.transform.position.x - transform.position.x) * Mathf.Abs((player.transform.position.x - transform.position.x)) * 0.01f, (1.25f - hit.distance) * 2));
        
        
        
    }
}

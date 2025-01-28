using UnityEngine;

public class BossBodyMovement : MonoBehaviour
{
    public float followSpeed;
    public float elevationSpeed;
    public GameObject player;
    public float elevationLevel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector2.down);
        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2((player.transform.position.x - transform.position.x) * Mathf.Abs((player.transform.position.x - transform.position.x)) * 0.02f * followSpeed, (elevationLevel - hit.distance) * Mathf.Pow(Mathf.Abs(elevationLevel - hit.distance), 0.5f) * 2 * elevationSpeed));
        
        
        
    }
}

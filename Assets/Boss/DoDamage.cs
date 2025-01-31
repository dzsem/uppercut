using UnityEngine;

public class DoDamage : MonoBehaviour
{
    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            player.GetComponent<PlayerHealth>().Health -= 1;
            player.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position) * 2000);
        }
    }
}

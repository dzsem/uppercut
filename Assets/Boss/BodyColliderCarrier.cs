using UnityEngine;

public class BodyColliderCarrier : MonoBehaviour
{
    public GameObject player;
    public GameObject body;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject != player && collision.gameObject.transform.parent.gameObject != player && collision.gameObject.layer == 0)
        {
            body.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 1000);
        }
    }
}

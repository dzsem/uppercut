using UnityEngine;

public class OpenFloor : MonoBehaviour
{
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
        if (collision.gameObject.tag == "Player" && collision.isTrigger == false && collision.gameObject != gameObject  /*collision.gameObject.GetComponent<PlayerMover>().touchesGround == false*/)
        {
            collision.gameObject.GetComponent<PlayerMover>().col.isTrigger = true;
           
        
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "Player" && collision == collision.gameObject.GetComponent<PlayerMover>().col && collision.gameObject != gameObject && collision.enabled == true)
        {
            collision.gameObject.GetComponent<PlayerMover>().col.isTrigger = false;
            
        }
    }
}

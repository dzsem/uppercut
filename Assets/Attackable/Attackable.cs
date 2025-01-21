using Unity.VisualScripting;
using UnityEngine;

public class AttackableComponent : MonoBehaviour
{
    public BoxCollider2D hitboxCollider;

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
        if (collision.tag == "attackbox")
        {
            Debug.Log("Trigger enter");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "attackbox")
        {
            Debug.Log("Trigger exit");
        }
    }


}

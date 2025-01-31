using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    public GameObject finger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = finger.transform.position;
    }
}

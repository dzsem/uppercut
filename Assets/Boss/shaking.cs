using UnityEngine;

public class shaking : MonoBehaviour
{
    bool _shouldGrow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x > 1.01f)
        {
               _shouldGrow = false;
        }
        if (transform.localScale.x < 1f)
        {
            _shouldGrow = true;
        }
        if (_shouldGrow)
        {
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f)* Time.deltaTime * 0.5f;
        }
        else
        {
            transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f) * Time.deltaTime * 0.5f;
        }


    }
}

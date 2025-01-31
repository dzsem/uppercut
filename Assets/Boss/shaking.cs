using UnityEngine;

public class shaking : MonoBehaviour
{
    bool _shouldGrow;
    float _maxGrowth = 1.01f;
    float _speed = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x > _maxGrowth)
        {
               _shouldGrow = false;
        }
        if (transform.localScale.x < 1f)
        {
            _shouldGrow = true;
            _maxGrowth = 1.01f;
            _speed = 0.5f;
        }
        if (_shouldGrow)
        {
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f) * Time.deltaTime * _speed;
        }
        else
        {
            transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f) * Time.deltaTime * _speed;
        }


    }
    public void LargerGrowth()
    {
        transform.localScale = Vector3.one;
        _maxGrowth = 1.3f;
        _speed = 20;
        _shouldGrow = true;
    }
}

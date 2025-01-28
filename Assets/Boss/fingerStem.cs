using UnityEngine;

public class fingerStem : MonoBehaviour
{
    public fignerMover fingerScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 _diff = ( new Vector2 (transform.position.x, transform.position.y)- fingerScript.firstHit.point);

        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(_diff.y, _diff.x) * Mathf.Rad2Deg);
    }
}

using UnityEngine;
using UnityEngine.UIElements;

public class eyeScript : MonoBehaviour
{
    public GameObject player;
    public GameObject eye;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 _diff = (player.transform.position - transform.position);

        transform.rotation =  Quaternion.Euler(0, 0, Mathf.Atan2(_diff.y, _diff.x) * Mathf.Rad2Deg);
        eye.transform.localPosition = new Vector3(0.02f/Mathf.Sqrt(0.01f*Mathf.Pow(Mathf.Cos(transform.rotation.eulerAngles.z /Mathf.Rad2Deg), 2) + 0.04f * Mathf.Pow(Mathf.Sin(transform.rotation.eulerAngles.z /Mathf.Rad2Deg), 2)), 0, 0);
    }
}

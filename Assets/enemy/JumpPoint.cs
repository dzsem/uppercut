using System;
using Unity.Collections;
using UnityEngine;

public class JumpPoint : MonoBehaviour
{
    public CostumeTrigger jumpFromPointTrigger;
    public GameObject jumpToPoint;
    private float JUMP_HEIGHT;
    private float JUMP_WIDTH;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        JUMP_HEIGHT = jumpToPoint.transform.position.y - jumpFromPointTrigger.gameObject.transform.position.y;
        JUMP_WIDTH = Math.Abs(jumpToPoint.transform.position.x - jumpFromPointTrigger.gameObject.transform.position.x);
    }

    // Update is called once per frame
    void Update()
    {

    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        CircleCollider2D component = jumpFromPointTrigger.GetComponent<CircleCollider2D>();
        Gizmos.DrawWireSphere(jumpToPoint.transform.position, component.radius);
        Gizmos.DrawWireSphere(jumpFromPointTrigger.transform.position, component.radius);
    }

    public float GetJumpHeight() => JUMP_HEIGHT;
    public float GetJumpWidth() => JUMP_WIDTH;
    public CostumeTrigger GetJumpFromPointTrigger()=> jumpFromPointTrigger;
    public GameObject GetJumpToPoint()=> jumpToPoint;
}

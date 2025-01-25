using Unity.VisualScripting;
using UnityEngine;

public class CostumeTrigger : MonoBehaviour
{
    public event System.Action<Collider2D> EnterTrigger;
    public event System.Action<Collider2D> ExitTrigger;

    void OnTriggerEnter2D(Collider2D collider)
    {
        EnterTrigger?.Invoke(collider);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        ExitTrigger?.Invoke(collider);
    }
}

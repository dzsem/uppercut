using Unity.VisualScripting;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public event System.Action<Collider> EnterTrigger;
    public event System.Action<Collider> ExitTrigger;

    void OnTriggerEnter(Collider collider)
    {
        EnterTrigger?.Invoke(collider);
    }

    void OnTriggerExit(Collider collider)
    {
        ExitTrigger?.Invoke(collider);
    }
}

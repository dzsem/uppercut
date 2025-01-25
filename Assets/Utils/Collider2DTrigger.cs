using UnityEngine;

public class Collider2DTrigger : MonoBehaviour
{
	public event System.Action<Collider2D> enterTrigger;
	public event System.Action<Collider2D> exitTrigger;

	void OnTriggerEnter2D(Collider2D collider)
	{
		enterTrigger?.Invoke(collider);
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		exitTrigger?.Invoke(collider);
	}
}

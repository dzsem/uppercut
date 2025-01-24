using UnityEngine;

public class EnemyTargetSystem : MonoBehaviour
{

    public CostumeTrigger viewArea;

    public event System.Action playerInRange;
    public event System.Action playerOutOfRange;


    private void Awake()
    {
        viewArea.EnterTrigger += OnPlayerInViewArea;
        viewArea.ExitTrigger += OnPlayerOutOfViewArea;
    }

    void OnPlayerInViewArea(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange?.Invoke();
        }
    }

    void OnPlayerOutOfViewArea(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerOutOfRange?.Invoke();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

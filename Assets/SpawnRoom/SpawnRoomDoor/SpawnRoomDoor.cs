using UnityEngine;

public class SpawnRoomDoor : MonoBehaviour
{
    public BoxCollider2D physicsCollider;
    public Animator animator;
    public SpriteRenderer sprite;
    public AudioSource soundEmitter;
    public bool isBroken = false;

    [SerializeField] private Vector2 _openDoorTextureOffset = new Vector2(0.35f, 0.0f);

    public void OnHit(int damage)
    {
        PlayerMover player = FindFirstObjectByType<PlayerMover>();

        if (player?.GetAttackType() != PlayerMover.EAttackType.PUNCH)
            return;

        isBroken = true;
        soundEmitter.Play();
        transform.Translate(new Vector3(_openDoorTextureOffset.x, _openDoorTextureOffset.y, 0.0f));
        animator.SetBool("isDoorBroken", true);
    }
}

using UnityEngine;

/// <summary>
/// A játékos által kiadott hangokat kezelő interfész.
/// TODO: némítás
/// </summary>
public class PlayerSound : MonoBehaviour
{
    public AudioSource dashSoundEmitter;
    public AudioSource jumpSoundEmitter;
    public AudioSource punchdownSoundEmitter;
    public AudioSource damageSoundEmitter;
    public AudioSource deathSoundEmitter;
    public AudioSource groundTouchSoundEmitter;

    public void PlayDashSound()
    {
        dashSoundEmitter.Play();
    }
    public void PlayJumpSound()
    {
        jumpSoundEmitter.Play();
    }
    public void PlayPunchdownSound()
    {
        punchdownSoundEmitter.Play();
    }
    public void PlayDamageSound()
    {
        damageSoundEmitter.Play();
    }

    public void PlayDeathSound()
    {
        damageSoundEmitter.Play();
    }

    public void PlayGroundTouchSound()
    {
        groundTouchSoundEmitter.Play();
    }
}

using UnityEngine;
using UnityEngine.Audio;

public class SFXOneshotPlayer : MonoBehaviour
{
    public static SFXOneshotPlayer Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySFXOneshot(Vector3 position, AudioClip audioClip)
    {
        AudioSource.PlayClipAtPoint(audioClip, position);
    }
}
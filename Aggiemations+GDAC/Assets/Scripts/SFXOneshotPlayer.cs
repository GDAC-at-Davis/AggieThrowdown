using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class SFXOneshotPlayer : MonoBehaviour
{
    public static SFXOneshotPlayer Instance;

    [SerializeField]
    private int maxPoolSize;

    private ObjectPool<AudioSource> audioSourcePool;

    private void Awake()
    {
        Instance = this;
        audioSourcePool = new ObjectPool<AudioSource>(
            CreateAudioSource,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPooledObject,
            true,
            maxPoolSize);
    }

    private void OnDestroyPooledObject(AudioSource source)
    {
        Destroy(source.gameObject);
    }

    private void OnReturnedToPool(AudioSource source)
    {
        source.gameObject.SetActive(false);
    }

    private void OnTakeFromPool(AudioSource source)
    {
        source.gameObject.SetActive(true);
    }

    private AudioSource CreateAudioSource()
    {
        var go = new GameObject("Pooled Audio Source");
        go.transform.parent = transform;
        go.SetActive(false);
        var source = go.AddComponent<AudioSource>();
        return source;
    }

    public void PlaySFXOneshot(Vector3 position, AudioClip audioClip)
    {
        var source = audioSourcePool.Get();
        source.transform.position = position;
        source.clip = audioClip;
        source.Play();
        StartCoroutine(ReturnToPoolAfterDuration(source, audioClip.length + 0.1f));
    }

    private IEnumerator ReturnToPoolAfterDuration(AudioSource source, float audioClipLength)
    {
        yield return new WaitForSeconds(audioClipLength);
        audioSourcePool.Release(source);
    }
}
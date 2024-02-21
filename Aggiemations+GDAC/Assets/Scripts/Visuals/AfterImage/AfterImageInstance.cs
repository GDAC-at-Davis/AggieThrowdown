using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class AfterImageInstance : MonoBehaviour
{
    [SerializeField]
    private SimpleAnimator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private ObjectPool<AfterImageInstance> pool;

    public void Initialize(ObjectPool<AfterImageInstance> pool)
    {
        this.pool = pool;
    }

    public void Play(Transform sourceTransform, bool flipX, Sprite sprite)
    {
        transform.position = sourceTransform.position;
        transform.rotation = sourceTransform.rotation;
        transform.localScale = sourceTransform.lossyScale;

        animator.Play(true);
        StartCoroutine(ReturnCorout(animator.ClipLength()));
        spriteRenderer.sprite = sprite;
        spriteRenderer.flipX = flipX;
    }

    private IEnumerator ReturnCorout(float delay)
    {
        yield return new WaitForSeconds(delay);
        pool.Release(this);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AfterImageManager : MonoBehaviour
{
    [SerializeField]
    private ServiceContainerSO serviceContainer;

    [SerializeField]
    private ObjectPool<AfterImageInstance> afterImagePool;

    [SerializeField]
    private AfterImageInstance afterImagePrefab;

    private void Awake()
    {
        afterImagePool = new ObjectPool<AfterImageInstance>(
            CreateAfterImagePooledObject,
            OnAfterImageInstanceTaken,
            OnAfterImageInstanceReleased,
            OnDestroyAfterImageInstance);


        serviceContainer.EventManager.OnAfterImageRequested += ShowAfterImage;
    }

    private void OnDestroy()
    {
        serviceContainer.EventManager.OnAfterImageRequested -= ShowAfterImage;
    }

    public void ShowAfterImage(Transform transform, bool flipX, Sprite sprite)
    {
        var afterImage = afterImagePool.Get();
        // Automatically returns to pool after animation ends
        afterImage.Play(transform, flipX, sprite);
    }

    private AfterImageInstance CreateAfterImagePooledObject()
    {
        var instance = Instantiate(afterImagePrefab, transform);
        instance.Initialize(afterImagePool);
        instance.gameObject.SetActive(false);
        return instance;
    }

    private void OnAfterImageInstanceTaken(AfterImageInstance instance)
    {
        instance.gameObject.SetActive(true);
    }

    private void OnAfterImageInstanceReleased(AfterImageInstance instance)
    {
        instance.gameObject.SetActive(false);
    }

    private void OnDestroyAfterImageInstance(AfterImageInstance instance)
    {
        Destroy(instance.gameObject);
    }
}
using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
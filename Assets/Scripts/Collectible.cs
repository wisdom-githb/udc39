using UnityEngine;

public sealed class Collectible : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;

    public bool IsCollected { get; private set; }

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsCollected && other.CompareTag("Player"))
        {
            LevelGameManager.Instance.TryCollect(this);
        }
    }

    public void MarkCollected()
    {
        IsCollected = true;
        gameObject.SetActive(false);
    }

    public void ResetCollectible()
    {
        IsCollected = false;
        gameObject.SetActive(true);
    }
}

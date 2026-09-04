using UnityEngine;

public sealed class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelGameManager.Instance.TryCompleteLevel();
        }
    }
}

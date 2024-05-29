using UnityEngine;

public class StunTrapCollisionDetector : MonoBehaviour
{
    public float StunDuration = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.StunPlayer(StunDuration);
                Debug.Log("Player bị stun  " + StunDuration + " seconds.");
                DestroystunStrap();
            }
        }
    }
    private void DestroystunStrap()
    {
        Destroy(gameObject); // Hủy bẫy boom
    }
}
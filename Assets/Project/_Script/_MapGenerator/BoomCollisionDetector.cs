using UnityEngine;

public class BoomCollisionDetector : MonoBehaviour
{
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController.ReducePlayerHealth(10); // giảm 10 máu người 
            DestroyBoom(); // Gọi phương thức để hủy bẫy boom

        }
    }
    private void DestroyBoom()
    {
        Destroy(gameObject); // Hủy bẫy boom
    }
}
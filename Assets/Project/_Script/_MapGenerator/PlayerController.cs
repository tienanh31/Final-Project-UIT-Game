using System.Diagnostics;

public class PlayerController : MonoBehaviour
{
    private bool isPlayerStunned = false;
    private float playerStunTimer = 0f;
    private int playerHealth = 100;
    private CharacterController characterController;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    private void Update()
    {
        if (isPlayerStunned)
        {
            playerStunTimer -= Time.deltaTime;
            if (playerStunTimer <= 0f)
            {
                StopStun();
            }
        }
    }
    public void ReducePlayerHealth(int amount)
    {
        playerHealth -= amount;
        Debug.Log("Máu của người chơi bị giảm " + amount + ". Máu hiện tại: " + playerHealth);
        if (playerHealth <= 0)
        {
            Debug.Log("Player đã chêt .");
            // Add game over logic here
        }
    }
    public void StunPlayer(float duration)
    {
        if (!isPlayerStunned)
        {
            isPlayerStunned = true;
            playerStunTimer = duration;

            // Disable player movement and actions
            characterController.enabled = false;
            playerMovement.enabled = false;
            playerAttack.enabled = false;

            Debug.Log("Player bị stun  " + duration + " seconds.");
        }
    }

    private void StopStun()
    {
        isPlayerStunned = false;

        // Restore player movement and actions
        characterController.enabled = true;
        playerMovement.enabled = true;
        playerAttack.enabled = true;

        Debug.Log("Player hết hiệu ứng stun");
    }
}
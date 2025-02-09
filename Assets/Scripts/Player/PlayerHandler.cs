using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private PlayerCombat playerCombat;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        playerMovement.Update();
        playerHealth.Update();
        playerCombat.Update();
    }
}
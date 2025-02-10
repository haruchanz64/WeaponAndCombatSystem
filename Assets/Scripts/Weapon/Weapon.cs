using UnityEngine;

namespace Weapon
{
    public class Weapon : MonoBehaviour
    {
        [Header("Combat")]
        private int[] attackDamage = { 1, 3, 5 };
        private PlayerCombat player;

        private void Start()
        {
            player = GetComponentInParent<PlayerCombat>();
            if (player == null)
            {
                Debug.LogError("PlayerCombat component not found in parent!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                var target = other.GetComponent<Enemy>();

                if (target != null)
                {
                    // Get the current combo step from the player
                    int currentComboStep = player.GetCurrentComboStep();

                    if (currentComboStep > 0 && currentComboStep <= attackDamage.Length) // Check for valid combo step
                    {
                        int damage = attackDamage[currentComboStep - 1];
                        target.OnTakeDamage(damage);
                        Debug.Log($"Hit {target.name} for {damage} damage.");
                    }
                    else
                    {
                        Debug.LogWarning($"Invalid combo step: {currentComboStep}. No damage dealt.");
                    }
                }
                else
                {
                    Debug.LogWarning("No Enemy component found on the collided object.");
                }
            }
        }
    }
}
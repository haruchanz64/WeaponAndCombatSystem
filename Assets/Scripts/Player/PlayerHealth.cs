using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Health - Regeneration")]
    private float regenValue = 20f;
    [SerializeField] private Image regenIcon;
    private readonly float regenActivationCooldownUI = 10f;
    private bool isCooldown;

    [SerializeField] private ParticleSystem particle;
    private bool isHurt = false;
    private bool isDead = false;
    private Animator animator;
    private PlayerMovement playerMovement; // Reference to PlayerMovement

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>(); // Get the PlayerMovement component
    }

    public void Update()
    {
        OnUpdateHealth();
        OnHandleHealingCooldown(); // Move this to Update to handle cooldown properly

        // Check for health potion usage
        if (Input.GetKeyDown(KeyCode.Z) && !isCooldown && !isDead)
        {
            particle.Play();
            OnTakeRegenerate(regenValue);
            OnStartRegenCooldown();
            StartCoroutine(StopParticleAfterDelay(5f)); // Start coroutine to stop particle after 5 seconds
        }
    }

    private void OnStartRegenCooldown()
    {
        isCooldown = true;
        regenIcon.fillAmount = 0;
    }
    
    private IEnumerator StopParticleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        particle.Stop(); // Stop the particle system
    }

    private void OnHandleHealingCooldown()
    {
        if (!isCooldown) return;
        regenIcon.fillAmount += 1 / regenActivationCooldownUI * Time.deltaTime;

        if (regenIcon.fillAmount >= 1)
        {
            regenIcon.fillAmount = 0;
            isCooldown = false;
        }
    }

    private void OnUpdateHealth()
    {
        healthBarImage.fillAmount = currentHealth / maxHealth;
        healthText.SetText($"{currentHealth}/{maxHealth}");

        // Check for player death
        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }

    public void OnTakeDamage(float damage)
    {
        // Check if the player is already hurt
        if (isHurt) return;

        // Set hurt status and play hurt animation
        isHurt = true;
        animator.SetBool("Hurt", true); // Trigger the hurt animation
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Start coroutine to wait for the hurt animation to finish
        StartCoroutine(WaitForHurtAnimation());
    }

    private IEnumerator WaitForHurtAnimation()
    {
        // Wait until the hurt animation is finished
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt")); // Replace "Hurt" with the actual name of your hurt animation state
        isHurt = false; // Reset hurt status
        animator.SetBool("Hurt", false); // Reset the hurt animation
    }

    public void OnTakeRegenerate(float heal)
    {
        currentHealth += heal;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void OnDeath()
    {
        isDead = true;
        // Handle player death
        Debug.Log("Player has died.");
        playerMovement.enabled = false; // Disable movement
        animator.SetTrigger("Death"); // Trigger death animation
        // Optionally, you can add more logic here, like restarting the game or showing a game over screen
    }
}
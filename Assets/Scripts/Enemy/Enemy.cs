using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 6.0f;
    [SerializeField] private float attackDistance = 2.0f; // Distance to attack the player
    [SerializeField] private float chaseDistance = 10.0f; // Distance to start chasing the player
    [SerializeField] private float retreatDistance = 3.0f; // Distance to retreat from the player
    [SerializeField] private float sphereCastRadius = 0.5f; // Radius for the SphereCast

    [Header("Health")]
    private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private Image healthBarImage;

    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform; // Reference to the player's transform
    private PlayerHealth player; // Reference to the Player component

    private enum EnemyState { Idle, Chase, Attack, Retreat }
    private EnemyState currentState = EnemyState.Idle;

    private bool isAttacking = false; // Flag to prevent continuous attacking

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        characterController = GetComponentInChildren<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;

        // Get the Player component
        player = playerTransform.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        HandleAI();
        OnUpdateHealth();
    }

    private void OnUpdateHealth()
    {
        healthBarImage.fillAmount = currentHealth / maxHealth;

        // Check for enemy death
        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }

    public void OnTakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void HandleAI()
    {
        if (currentHealth <= 0) return; // Disable AI if dead

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                if (distanceToPlayer < chaseDistance)
                {
                    currentState = EnemyState.Chase;
                }
                break;

            case EnemyState.Chase:
                ChasePlayer(distanceToPlayer);
                if (distanceToPlayer <= attackDistance)
                {
                    currentState = EnemyState.Attack;
                }
                else if (distanceToPlayer > chaseDistance)
                {
                    currentState = EnemyState.Idle;
                }
                break;

            case EnemyState.Attack:
                if (distanceToPlayer > attackDistance)
                {
                    currentState = EnemyState.Chase;
                }
                else
                {
                    if (!isAttacking)
                    {
                        StartCoroutine(AttackPlayer());
                    }
                }
                break;
        }
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereCastRadius, directionToPlayer, out hit, chaseDistance))
        {
            if (hit.transform.CompareTag("Player"))
            {
                characterController.Move(directionToPlayer * Time.deltaTime * movementSpeed);
                animator.SetFloat("Movement", characterController.velocity.magnitude);

                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        animator.SetBool("Attacking", true);
        yield return new WaitForSeconds(1f);

        player.OnTakeDamage(Random.Range(1f, 3f));

        animator.SetBool("Attacking", false);
        isAttacking = false;
    }

    private void OnDeath()
    {
        // Handle enemy death
        Debug.Log($"{gameObject.name} has died.");
        // Disable AI and any other necessary components
        this.enabled = false; // Disable the enemy script
        animator.SetTrigger("Dying"); // Trigger death animation
        // Optionally, you can add more logic here, like dropping loot or playing a death sound
    }
}
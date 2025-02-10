using Cinemachine;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 8.0f;
    [SerializeField] private float dodgeDistance = 5.0f; // Distance to dodge forward
    [SerializeField] private float dodgeDuration = 0.5f; // Duration of the dodge

    private Vector3 velocity;
    private bool isPlayerGrounded;
    private bool isDodging = false;
    private bool isBlocking = false; // New variable to track blocking state

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        HandleMovement();
        HandleDodge();
        HandleBlocking(); // Handle blocking input
    }

    private void HandleMovement()
    {
        if (isDodging || isBlocking) return; // Prevent movement while dodging or blocking

        // Update grounded status
        isPlayerGrounded = IsPlayerGrounded();

        // Get input
        var move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        float movement = Mathf.Clamp01(move.magnitude);
        animator.SetFloat("Movement", movement);

        // Handle camera-based movement
        if (Camera.main != null)
        {
            Transform cameraTransform = Camera.main.transform;
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            // Flatten the camera vectors
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate movement direction relative to camera
            Vector3 moveDirection = (cameraForward * move.z + cameraRight * move.x).normalized;

            if (moveDirection.magnitude > 0) // Only rotate if there's input
            {
                transform.forward = moveDirection; // Face the movement direction
            }

            // Move the character
            characterController.Move(moveDirection * Time.deltaTime * movementSpeed);
        }
        else
        {
            Debug.LogWarning("Main Camera not assigned to PlayerMovement script!");
        }

        // Apply gravity
        if (!isPlayerGrounded)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime; // Apply gravity
        }
        else
        {
            velocity.y = 0f; // Reset vertical velocity when grounded
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    private bool IsPlayerGrounded()
    {
        return characterController.isGrounded;
    }

    private void HandleDodge()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDodging) // Check for dodge input
        {
            isDodging = true;
            animator.SetBool("Dodge", true); // Trigger dodge animation
            StartCoroutine(DodgeMovement());
        }
    }

    private IEnumerator DodgeMovement()
    {
        // Calculate the forward direction based on the player's current rotation
        Vector3 dodgeDirection = transform.forward * dodgeDistance;

        float elapsedTime = 0f;

        while (elapsedTime < dodgeDuration)
        {
            // Move the player forward during the dodge
            characterController.Move(dodgeDirection * (Time.deltaTime / dodgeDuration));

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        animator.SetBool("Dodge", false); // Reset dodge animation
        isDodging = false; // Reset dodging status
    }

    private void HandleBlocking()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isBlocking = true;
            animator.SetBool("Blocking", true); // Set blocking animation
        }
        else
        {
            isBlocking = false;
            animator.SetBool("Blocking", false); // Reset blocking animation
        }
    }

    public bool IsBlocking()
    {
        return isBlocking; // Method to check if the player is blocking
    }

    public bool IsDodging()
    {
        return isDodging;
    }
}
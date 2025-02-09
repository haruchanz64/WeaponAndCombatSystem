using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMovementAnimation(float movement)
    {
        animator.SetFloat("Movement", movement);
    }

    public void SetDodgeAnimation(bool isDodging)
    {
        animator.SetBool("Dodge", isDodging);
    }

    public void SetBlockAnimation(bool isBlocking)
    {
        animator.SetBool("Blocking", isBlocking);
    }

    public void SetHurtAnimation(bool isHurt)
    {
        animator.SetBool("Hurt", isHurt);
    }

    public void SetDeathAnimation()
    {
        animator.SetTrigger("Death");
    }

    public void PlayAttackAnimation(int step)
    {
        animator.SetBool("Hit1", false);
        animator.SetBool("Hit2", false);
        animator.SetBool("Hit3", false);

        switch (step)
        {
            case 1:
                animator.SetBool("Hit1", true);
                break;
            case 2:
                animator.SetBool("Hit2", true);
                break;
            case 3:
                animator.SetBool("Hit3", true);
                break;
        }
    }

    public void ResetAttackAnimations()
    {
        animator.SetBool("Hit1", false);
        animator.SetBool("Hit2", false);
        animator.SetBool("Hit3", false);
    }
}
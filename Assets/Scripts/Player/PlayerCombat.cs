using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combo System")]
    [SerializeField] private float comboWindow = 0.5f;
    private int currentComboStep = 0;
    private Coroutine currentComboCoroutine;
    private bool isAttacking = false;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        OnHandleWeapon();
    }

    private void OnHandleWeapon()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleCombo();
        }
    }

    private void HandleCombo()
    {
        if (isAttacking) return;

        if (currentComboCoroutine == null)
        {
            currentComboStep = 1;
            StartCoroutine(PerformAttack(currentComboStep));
        }
        else if (currentComboStep < 3)
        {
            currentComboStep++;
            StopCoroutine(currentComboCoroutine);
            StartCoroutine(PerformAttack(currentComboStep));
        }
        else
        {
            ResetCombo();
        }
    }

    private IEnumerator PerformAttack(int step)
    {
        isAttacking = true;

        PlayAttackAnimation(step);
        Debug.Log($"Performing attack step {step}");

        // Construct the animation name
        var animationName = "Great Sword Slash" + (step == 1 ? "" : step);

        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName(animationName));

        
        
        isAttacking = false;

        currentComboCoroutine = StartCoroutine(ComboTimer());
    }

    private void PlayAttackAnimation(int step)
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

    private IEnumerator ComboTimer()
    {
        yield return new WaitForSeconds(comboWindow);
        ResetCombo();
    }

    private void ResetCombo()
    {
        currentComboStep = 0;
        currentComboCoroutine = null;
        isAttacking = false;
        animator.SetBool("Hit1", false);
        animator.SetBool("Hit2", false);
        animator.SetBool("Hit3", false);
        Debug.Log("Combo reset.");
    }

    public int GetCurrentComboStep()
    {
        return currentComboStep;
    }
}
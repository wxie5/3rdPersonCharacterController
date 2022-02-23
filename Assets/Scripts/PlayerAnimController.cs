using UnityEngine;

public class PlayerAnimController : AnimController
{
    private bool isInputReceived = false;

    #region Bool Params
    private bool isEquAxe = false;
    private bool isEquSword = false;
    #endregion

    #region Script Reference
    private SimpleMovement playerMovement;
    #endregion

    protected override void Initialize()
    {
        base.Initialize();
        isInputReceived = false;
        playerMovement = GetComponent<SimpleMovement>();
    }

    protected override void AnimCtrlUpdate()
    {
        base.AnimCtrlUpdate();

        animator.SetFloat("Speed", playerMovement.CurMoveSpeed);

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Movement"))
        {
            playerMovement.IsMoveable(true);
        }
        else

        {
            playerMovement.IsMoveable(false);
        }
    }

    public void ResetInputReceived()
    {
        isInputReceived = false;
    }

    public void Attack(float normalizedTime)
    {
        if(isInputReceived || normalizedTime < 0.5f)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
            isInputReceived = true;
        }
    }

    public void Attack()
    {
        if (isInputReceived)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
            isInputReceived = true;
        }
    }

    public void SwitchAxe()
    {
        if (isInputReceived)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            isEquAxe = !isEquAxe;
            animator.SetBool("IsEquAxe", isEquAxe);
            isInputReceived = true;
        }
    }

    public void SwitchSword()
    {
        if (isInputReceived)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            isEquSword = !isEquSword;
            animator.SetBool("IsEquSword", isEquSword);
            isInputReceived = true;
        }
    }

    public void Roll()
    {
        if (isInputReceived)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Roll");
            isInputReceived = true;
        }
    }

    public void Roll(float normalizedTime)
    {
        if (isInputReceived || normalizedTime < 0.5f)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Roll");
            isInputReceived = true;
        }
    }

    public void FocusTarget()
    {
        if (playerMovement.IsCamLocked)
        {
            playerMovement.FocusOnTarget();
        }
    }

    public void UnFocus()
    {
        playerMovement.UnFocusTarget();
    }

    public void TurnDirection()
    {
        playerMovement.TurnDirection();
    }
}

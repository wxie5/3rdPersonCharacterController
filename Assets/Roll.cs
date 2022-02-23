using UnityEngine;

public class Roll : StateMachineBehaviour
{
    private PlayerAnimController playerAnimController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerAnimController == null)
        {
            playerAnimController = animator.GetComponent<PlayerAnimController>();
        }

        if (stateInfo.IsTag("Movement"))
        {
            animator.applyRootMotion = false;
        }
        else
        {
            animator.applyRootMotion = true;
        }

        playerAnimController.UnFocus();
        playerAnimController.TurnDirection();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerAnimController.Attack(stateInfo.normalizedTime);
        playerAnimController.Roll(stateInfo.normalizedTime);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerAnimController.ResetInputReceived();
    }
}

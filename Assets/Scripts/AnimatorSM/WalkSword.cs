using UnityEngine;

public class WalkSword : StateMachineBehaviour
{
    private PlayerAnimController playerAnimController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerAnimController == null)
        {
            playerAnimController = animator.GetComponent<PlayerAnimController>();
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerAnimController.Attack();
        playerAnimController.SwitchSword();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerAnimController.ResetInputReceived();
    }
}

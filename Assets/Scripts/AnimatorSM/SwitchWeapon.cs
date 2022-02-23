using UnityEngine;

public class SwitchWeapon : StateMachineBehaviour
{
    private PlayerAnimController playerAnimController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerAnimController == null)
        {
            playerAnimController = animator.GetComponent<PlayerAnimController>();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerAnimController.ResetInputReceived();
    }
}

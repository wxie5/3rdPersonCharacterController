using UnityEngine;

public class Idle : StateMachineBehaviour
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
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerAnimController.SwitchAxe();
        playerAnimController.SwitchSword();
    }
}

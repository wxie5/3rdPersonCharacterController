using UnityEngine;

public class Walk : StateMachineBehaviour
{
    private PlayerAnimController playerAnimController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerAnimController == null)
        {
            playerAnimController = animator.GetComponent<PlayerAnimController>();
        }
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerAnimController.SwitchAxe();
        playerAnimController.SwitchSword();
    }
}

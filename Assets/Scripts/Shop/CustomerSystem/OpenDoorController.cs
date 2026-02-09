using UnityEngine;

public class OpenDoorController : StateMachineBehaviour
{
    public override void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        var controller = animator.GetComponentInParent<CustomerAnimatorController>();
        
        if (controller == null) return;
        
        controller.NotifyDoorAnimationFinished();
        Debug.Log("Open Door triggered");
    }
}
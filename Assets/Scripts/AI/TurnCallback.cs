using System.Collections;
using UnityEngine;

public class TurnCallback : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Turn"))
        {
            animator.transform.parent.GetComponent<AIController>().StartRunning();
        }
    }

    
}

using System.Collections;
using UnityEngine;

public class TurnCallback : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Turn"))
        {
            Utils.RunDelay(() =>
            {
                animator.transform.parent.GetComponent<AIController>().StartRunning();
            }, 1.07f);
        }
    }
}

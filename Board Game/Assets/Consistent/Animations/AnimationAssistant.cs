using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AnimationAssistant : StateMachineBehaviour
{
    // HANDLES LOGIC FOR UNIT ANIMATIONS. PLACE ON ALL ANIMATIONS EXCEPT IDLE

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.GetComponent<UnitManager>().attacking = false;
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}

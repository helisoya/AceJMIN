using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runs when the startup animation ends
/// </summary>
public class CharacterStartAnimStateMachine : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.GetComponent<Character>().SetInStartupAnim(false);
    }
}

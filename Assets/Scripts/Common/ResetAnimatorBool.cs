using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimatorBool : StateMachineBehaviour {

    public string isUsingRightHand = "isUsingRightHand";
    public bool isUsingRightHandStatus = false;

    public string isUsingLeftHand = "isUsingRightHand";
    public bool isUsingLeftHandStatus = false;

    public string isInteractingBool = "isInteracting";
    public bool isInteractingStatus = false;

    public string isFiringSpellBool = "isFiringSpell";
    public bool isFiringSpellStatus = false;

    public string canRotateBool = "canRotate";
    public bool canRotateBoolStatus = true;

    public string isInvulnerableBool = "isInvulnerable";
    public bool isInvulnerableStatus = false;

    public string isRotatingWithRootMotion = "isRotatingWithRootMotion";
    public bool isRotatingWithRootMotionStatus = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool(isInteractingBool, isInteractingStatus);
        animator.SetBool(isFiringSpellBool, isFiringSpellStatus);
        animator.SetBool(canRotateBool, canRotateBoolStatus);
        animator.SetBool(isInvulnerableBool, isInvulnerableStatus);
        animator.SetBool(isRotatingWithRootMotion, isRotatingWithRootMotionStatus);
        animator.SetBool(isUsingRightHand, isUsingRightHandStatus);
        animator.SetBool(isUsingLeftHand, isUsingLeftHandStatus);
        //Debug.Log("애니메이션 종료");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimatorBoolAI : ResetAnimatorBool
{
    // ���� ResetAnimatorBool�� AI���� �ʿ��� ��� �߰�
    public string isPhaseShifting = "isPhaseShifting";
    public bool isPhaseShiftingStatus = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool(isPhaseShifting, isPhaseShiftingStatus);
    }
}

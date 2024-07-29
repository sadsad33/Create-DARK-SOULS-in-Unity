using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class ResetAnimatorBoolAI : ResetAnimatorBool {
        // 기존 ResetAnimatorBool에 AI에만 필요한 기능 추가
        public string isPhaseShifting = "isPhaseShifting";
        public bool isPhaseShiftingStatus = false;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            animator.SetBool(isPhaseShifting, isPhaseShiftingStatus);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class ResetAnimatorBool : StateMachineBehaviour {

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

            CharacterManager character = animator.GetComponent<CharacterManager>();

            // 피격 애니메이션 재생 후 이전에 받은 강인도 데미지를 초기화
            character.characterStatsManager.previousPoiseDamageTaken = 0;

            animator.SetBool(isInteractingBool, isInteractingStatus);
            animator.SetBool(isFiringSpellBool, isFiringSpellStatus);
            animator.SetBool(isRotatingWithRootMotion, isRotatingWithRootMotionStatus);
            animator.SetBool(canRotateBool, canRotateBoolStatus);
            animator.SetBool(isInvulnerableBool, isInvulnerableStatus);
            if (character.IsOwner) {
                character.characterNetworkManager.isUsingRightHand.Value = false;
                character.characterNetworkManager.isUsingLeftHand.Value = false;
            }
            //Debug.Log("애니메이션 종료");
        }
    }
}
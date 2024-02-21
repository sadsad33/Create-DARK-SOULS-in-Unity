using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace SoulsLike {
    public class CharacterAnimatorManager : MonoBehaviour {
        public Animator anim;
        protected CharacterManager characterManager;
        protected CharacterStatsManager characterStatsManager;
        public bool canRotate;

        protected RigBuilder rigBuilder;
        public TwoBoneIKConstraint leftHandConstraint;
        public TwoBoneIKConstraint rightHandConstraint;

        bool handIKWeightReset = false;
        protected virtual void Awake() {
            characterManager = GetComponent<CharacterManager>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            rigBuilder = GetComponent<RigBuilder>();
        }
        
        // �ش� �ִϸ��̼��� �����Ѵ�.
        public void PlayTargetAnimation(string targetAnim, bool isInteracting, bool canRotate = false) {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("canRotate", canRotate);
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f);
        }

        // �ִϸ��̼��� ȸ���� ����
        public void PlayTargetAnimationWithRootRotation(string targetAnim, bool isInteracting) {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("isRotatingWithRootMotion", true);
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f);
        }

        #region �ִϸ��̼� �̺�Ʈ
        public virtual void CanRotate() {
            anim.SetBool("canRotate", true);
        }

        public virtual void StopRotation() {
            anim.SetBool("canRotate", false);
        }

        public virtual void EnableCombo() {
            anim.SetBool("canDoCombo", true);
        }

        public virtual void DisableCombo() {
            anim.SetBool("canDoCombo", false);
        }

        public virtual void EnableIsInvulnerable() {
            anim.SetBool("isInvulnerable", true);
        }

        public virtual void DisableIsInvulnerable() {
            anim.SetBool("isInvulnerable", false);
        }

        public virtual void EnableIsParrying() {
            characterManager.isParrying = true;
        }

        public virtual void DisableIsParrying() {
            characterManager.isParrying = false;
        }

        public virtual void EnableCanBeRiposted() {
            characterManager.canBeRiposted = true;
        }

        public virtual void DisableCanBeRiposted() {
            characterManager.canBeRiposted = false;
        }

        public virtual void TakeCriticalDamageAnimationEvent() {
            characterStatsManager.TakeDamageNoAnimation(characterManager.pendingCriticalDamage); // ��� ��󿡰� ������
            characterManager.pendingCriticalDamage = 0; // ��� ������ �ʱ�ȭ
        }

        // ���� Ȥ�� ����Ʈ ������ ���ε� �ʱ�ȭ �̺�Ʈ
        public virtual void ResetPoiseValue() {
            characterStatsManager.isStuned = false;
            characterStatsManager.totalPoiseDefense = characterStatsManager.armorPoiseBonus;
            characterStatsManager.poiseResetTimer = 0;
        }

        public virtual void ReleaseFromGrab() {
            if (characterStatsManager.isDead) return;
            characterManager.isGrabbed = false;
        }

        #endregion

        // ������ HandIK ����
        public virtual void SetHandIKForWeapon(HandIKTarget rightHandTarget, HandIKTarget leftHandTarget, bool isTwoHandingWeapon) {
            if (isTwoHandingWeapon) {
                // ������ ���¶�� Hand IK ��� �ʿ�� ����
                // �� ���� ��ġ�� Hand IK �Ҵ�
                rightHandConstraint.data.target = rightHandTarget.transform;
                rightHandConstraint.data.targetPositionWeight = 1; // 0 ~ 1 ������ ���ϴ� �� �Ҵ�
                rightHandConstraint.data.targetRotationWeight = 1;

                leftHandConstraint.data.target = leftHandTarget.transform;
                leftHandConstraint.data.targetPositionWeight = 1;
                leftHandConstraint.data.targetRotationWeight = 1;

            } else {
                // �ƴ϶�� ����
                rightHandConstraint.data.target = null;
                leftHandConstraint.data.target = null;
            }
            rigBuilder.Build();
        }

        public virtual void CheckHandIKWeight(HandIKTarget rightHandIK, HandIKTarget leftHandIK, bool isTwoHandingWeapon) {
            if (characterManager.isInteracting) return;

            if (handIKWeightReset) {
                handIKWeightReset = !handIKWeightReset;
                if (rightHandConstraint.data.target != null) {
                    rightHandConstraint.data.target = rightHandIK.transform;
                    rightHandConstraint.data.targetPositionWeight = 1;
                    rightHandConstraint.data.targetRotationWeight = 1;
                }
                if (leftHandConstraint.data.target != null) {
                    leftHandConstraint.data.target = leftHandIK.transform;
                    leftHandConstraint.data.targetPositionWeight = 1;
                    leftHandConstraint.data.targetRotationWeight = 1;
                }
            }
        }

        public virtual void EraseHandIKForWeapon() {
            // Hand IK ����ġ���� ��� 0���� ����
            handIKWeightReset = true;
            if (rightHandConstraint.data.target != null) {
                rightHandConstraint.data.targetPositionWeight = 0;
                rightHandConstraint.data.targetRotationWeight = 0;
            }
            if (leftHandConstraint.data.target != null) {
                leftHandConstraint.data.targetPositionWeight = 0;
                leftHandConstraint.data.targetRotationWeight = 0;
            }
        }
    }
}
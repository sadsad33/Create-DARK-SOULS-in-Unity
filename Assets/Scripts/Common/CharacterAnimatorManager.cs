using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Unity.Netcode;

namespace SoulsLike {
    public class CharacterAnimatorManager : MonoBehaviour {
        //public Animator anim;
        protected CharacterManager character;
        protected CharacterStatsManager characterStatsManager;
        public bool canRotate;

        protected RigBuilder rigBuilder;
        public TwoBoneIKConstraint leftHandConstraint;
        public TwoBoneIKConstraint rightHandConstraint;

        bool handIKWeightReset = false;
        protected virtual void Awake() {
            character = GetComponent<CharacterManager>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            rigBuilder = GetComponent<RigBuilder>();
        }

        // 해당 애니메이션을 실행한다.
        public void PlayTargetAnimation(string targetAnim, bool isInteracting, bool canRotate = false) {
            if (character.IsOwner) {
                character.animator.applyRootMotion = isInteracting;
                character.animator.SetBool("canRotate", canRotate);
                character.animator.SetBool("isInteracting", isInteracting);
                character.animator.CrossFade(targetAnim, 0.2f);

                // 단말에서 애니메이션을 실행하면 내 클라이언트의id 와 애니메이션, 현재 행동정보를 서버에 알림
                character.characterNetworkManager.NotifyServerOfAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnim, isInteracting);
            }
        }

        // 애니메이션의 회전을 따라감
        public void PlayTargetAnimationWithRootRotation(string targetAnim, bool isInteracting) {
            character.animator.applyRootMotion = isInteracting;
            character.animator.SetBool("isRotatingWithRootMotion", true);
            character.animator.SetBool("isInteracting", isInteracting);
            character.animator.CrossFade(targetAnim, 0.2f);
        }

        #region 애니메이션 이벤트
        public virtual void CanRotate() {
            character.animator.SetBool("canRotate", true);
        }

        public virtual void StopRotation() {
            character.animator.SetBool("canRotate", false);
        }

        public virtual void EnableCombo() {
            character.animator.SetBool("canDoCombo", true);
        }

        public virtual void DisableCombo() {
            character.animator.SetBool("canDoCombo", false);
        }

        public virtual void EnableIsInvulnerable() {
            character.animator.SetBool("isInvulnerable", true);
        }

        public virtual void DisableIsInvulnerable() {
            character.animator.SetBool("isInvulnerable", false);
        }

        public virtual void EnableIsParrying() {
            character.isParrying = true;
        }

        public virtual void DisableIsParrying() {
            character.isParrying = false;
        }

        public virtual void EnableCanBeRiposted() {
            character.canBeRiposted = true;
        }

        public virtual void DisableCanBeRiposted() {
            character.canBeRiposted = false;
        }



        public virtual void TakeCriticalDamageAnimationEvent() {
            characterStatsManager.TakeDamageNoAnimation(character.pendingCriticalDamage); // 잡기 대상에게 데미지
            character.pendingCriticalDamage = 0; // 잡기 데미지 초기화
        }

        // 보스 혹은 엘리트 몬스터의 강인도 초기화 이벤트
        public virtual void ResetPoiseValue() {
            characterStatsManager.isStuned = false;
            characterStatsManager.totalPoiseDefense = characterStatsManager.armorPoiseBonus;
            characterStatsManager.poiseResetTimer = 0;
        }

        public virtual void ReleaseFromGrab() {
            if (characterStatsManager.isDead) return;
            character.isGrabbed = false;
        }
        
        #endregion

        // 무기의 HandIK 설정
        public virtual void SetHandIKForWeapon(HandIKTarget rightHandTarget, HandIKTarget leftHandTarget, bool isTwoHandingWeapon) {
            if (isTwoHandingWeapon) {
                // 양손잡기 상태라면 Hand IK 기능 필요시 적용
                // 양 손의 위치에 Hand IK 할당
                rightHandConstraint.data.target = rightHandTarget.transform;
                rightHandConstraint.data.targetPositionWeight = 1; // 0 ~ 1 사이의 원하는 값 할당
                rightHandConstraint.data.targetRotationWeight = 1;

                leftHandConstraint.data.target = leftHandTarget.transform;
                leftHandConstraint.data.targetPositionWeight = 1;
                leftHandConstraint.data.targetRotationWeight = 1;

            } else {
                // 아니라면 해제
                rightHandConstraint.data.target = null;
                leftHandConstraint.data.target = null;
            }
            rigBuilder.Build();
        }

        public virtual void CheckHandIKWeight(HandIKTarget rightHandIK, HandIKTarget leftHandIK, bool isTwoHandingWeapon) {
            if (character.isInteracting) return;

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
            // Hand IK 가중치값을 모두 0으로 리셋
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

        // Root Motion이 적용된 애니메이션이 재생중에 Root Transform이 이동되면 호출
        // OnAnimatorMove 함수를 구현하게 되면 Animator 컴포넌트의 ApplyRootMotion이 HandledByScript가 되어 OnAnimatorMove 함수내에서 이동 관련 로직을 구현해 주어야 함
        public virtual void OnAnimatorMove() {
            if (!character.isInteracting) return;
            #region 리지드 바디를 이용한 움직임 제어를 할 경우
            //float delta = Time.deltaTime;
            //player.playerLocomotion.GetComponent<Rigidbody>().drag = 0;
            //Vector3 deltaPosition = player.anim.deltaPosition; // ���� ���������� ���� �����ӿ��� �ƹ�Ÿ�� ��ǥ ��ȭ��
            //deltaPosition.y = 0;
            //Vector3 velocity = deltaPosition / delta; // �Ÿ��� ��ȭ���� �ð����� ���� �ӵ��� ���Ѵ�.
            //player.playerLocomotion.GetComponent<Rigidbody>().velocity = velocity; // Rigidbody�� �ӵ��� ����
            #endregion

            Vector3 velocity = character.animator.deltaPosition;
            character.characterController.Move(velocity);
            character.transform.rotation *= character.animator.deltaRotation;
        }
    }
}
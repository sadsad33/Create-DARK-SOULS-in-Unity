using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class AnimatorHandler : MonoBehaviour {
        PlayerManager playerManager;
        InputHandler inputHandler;
        PlayerLocomotion playerLocomotion;
        public Animator anim;

        int vertical;
        int horizontal;
        public bool canRotate;
        
        public void Initialize() {
            playerManager = GetComponentInParent<PlayerManager>();
            anim = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        // BlendTree�� �̿��� �ܼ� �̵�����
        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting) {
            #region Vertical
            float v = 0;

            if (verticalMovement > 0 && verticalMovement < 0.55f) v = 0.5f;
            else if (verticalMovement > 0.55f) v = 1;
            else if (verticalMovement < 0 && verticalMovement > -0.55f) v = -0.5f;
            else if (verticalMovement < -0.55f) v = -1;
            else v = 0;
            #endregion

            #region Horizontal
            float h = 0;
            if (horizontalMovement > 0 && horizontalMovement < 0.55f) h = 0.5f;
            else if (horizontalMovement > 0.55f) h = 1;
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f) h = -0.5f;
            else if (horizontalMovement < -0.55f) h = -1;
            else h = 0;
            #endregion

            if (isSprinting && inputHandler.moveAmount > 0) {
                v = 2;
                h = horizontalMovement;
            }
            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        // �ش� �ִϸ��̼��� �����Ѵ�.
        public void PlayTargetAnimation(string targetAnim, bool isInteracting) {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.3f);
        }

        public void CanRotate() {
            canRotate = true;
        }
        public void StopRotation() {
            canRotate = false;
        }
        public void EnableCombo() {
            anim.SetBool("canDoCombo", true);
        }
        public void DisableCombo() {
            anim.SetBool("canDoCombo", false);
        }

        private void OnAnimatorMove() {
            if(!playerManager.isInteracting)
                return;

            float delta = Time.deltaTime;
            playerLocomotion.rigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition; // �ִϸ��̼��� ������ �����Ӷ� �ƹ�Ÿ�� ��ǥ�� ������
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta; // �̵��� �Ÿ��� �̵��� �ð����� ���� �ӵ��� ���Ѵ�.
            playerLocomotion.rigidbody.velocity = velocity; // Rigidbody�� �ӵ��� ����
        }
    }
}
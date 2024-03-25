using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerAnimatorManager : CharacterAnimatorManager {
        InputHandler inputHandler;
        PlayerLocomotionManager playerLocomotionManager;
        PlayerManager playerManager;
        int vertical;
        int horizontal;

        protected override void Awake() {
            base.Awake();
            playerManager = GetComponent<PlayerManager>();
            anim = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerLocomotionManager = GetComponentInParent<PlayerLocomotionManager>();
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

        private void OnAnimatorMove() {
            if (!characterManager.isInteracting) return;
            
            float delta = Time.deltaTime;
            playerLocomotionManager.rigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition; // ���� ���������� ���� �����ӿ��� �ƹ�Ÿ�� ��ǥ ��ȭ��
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta; // �Ÿ��� ��ȭ���� �ð����� ���� �ӵ��� ���Ѵ�.
            playerLocomotionManager.rigidbody.velocity = velocity; // Rigidbody�� �ӵ��� ����
        }

        public void DisableCollision() {
            playerLocomotionManager.characterCollider.enabled = false;
            playerLocomotionManager.characterColliderBlocker.enabled = false;
        }

        public void EnableCollision() {
            playerLocomotionManager.characterCollider.enabled = true;
            playerLocomotionManager.characterColliderBlocker.enabled = true;
        }

        public void FinishJump() {
            if (playerManager.isJumping)
                playerManager.isJumping = false;
        }
    }
}
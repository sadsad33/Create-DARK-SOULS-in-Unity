using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerAnimatorManager : CharacterAnimatorManager {
        InputHandler inputHandler;
        PlayerManager playerManager;
        int vertical;
        int horizontal;

        protected override void Awake() {
            base.Awake();
            playerManager = GetComponent<PlayerManager>();
            playerManager.anim = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
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
            playerManager.anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            playerManager.anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        private void OnAnimatorMove() {
            if (!characterManager.isInteracting) return;

            float delta = Time.deltaTime;
            playerManager.playerLocomotion.rigidbody.drag = 0;
            Vector3 deltaPosition = playerManager.anim.deltaPosition; // ���� ���������� ���� �����ӿ��� �ƹ�Ÿ�� ��ǥ ��ȭ��
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta; // �Ÿ��� ��ȭ���� �ð����� ���� �ӵ��� ���Ѵ�.
            playerManager.playerLocomotion.rigidbody.velocity = velocity; // Rigidbody�� �ӵ��� ����
        }

        public void DisableCollision() {
            playerManager.playerLocomotion.characterCollider.enabled = false;
            playerManager.playerLocomotion.characterColliderBlocker.enabled = false;
        }

        public void EnableCollision() {
            playerManager.playerLocomotion.characterCollider.enabled = true;
            playerManager.playerLocomotion.characterColliderBlocker.enabled = true;
        }

        public void FinishJump() {
            if (playerManager.isJumping)
                playerManager.isJumping = false;
        }

        public void FinishLadderInteractionAtTop() {
            playerManager.isMoving = false;
        }
    }
}
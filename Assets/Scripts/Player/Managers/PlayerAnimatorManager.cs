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

        // BlendTree를 이용한 단순 이동제어
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
            Vector3 deltaPosition = anim.deltaPosition; // 현재 마지막으로 계산된 프레임에서 아바타의 좌표 변화량
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta; // 거리의 변화량을 시간으로 나눠 속도를 구한다.
            playerLocomotionManager.rigidbody.velocity = velocity; // Rigidbody의 속도를 설정
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
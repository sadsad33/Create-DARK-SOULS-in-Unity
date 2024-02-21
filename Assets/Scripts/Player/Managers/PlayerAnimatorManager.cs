using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerAnimatorManager : CharacterAnimatorManager {
        InputHandler inputHandler;
        PlayerLocomotionManager playerLocomotionManager;

        int vertical;
        int horizontal;
        
        protected override void Awake() {
            base.Awake();
            anim = GetComponentInChildren<Animator>();
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
            if(!characterManager.isInteracting) return;

            float delta = Time.deltaTime;
            playerLocomotionManager.rigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition; // 애니메이션의 마지막 프레임때 아바타의 좌표를 가져옴
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta; // 이동한 거리를 이동한 시간으로 나눠 속도를 구한다.
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
    }
}
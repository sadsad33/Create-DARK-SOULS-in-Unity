using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerAnimatorManager : CharacterAnimatorManager {
        public PlayerManager player;
        int vertical;
        int horizontal;

        protected override void Awake() {
            base.Awake();
            player = GetComponent<PlayerManager>();
            player.animator = GetComponent<Animator>();
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

            if (isSprinting && player.inputHandler.moveAmount > 0) {
                v = 2;
                h = horizontalMovement;
            }
            player.animator.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            player.animator.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void DisableCollision() {
            //playerManager.playerLocomotion.characterCollider.enabled = false;
            //playerManager.playerLocomotion.characterColliderBlocker.enabled = false;
            player.characterController.enabled = false;
        }

        public void EnableCollision() {
            //player.playerLocomotion.characterCollider.enabled = true;
            //player.playerLocomotion.characterColliderBlocker.enabled = true;
            player.characterController.enabled = true;
        }

        public virtual void SuccessfullyUseCurrentConsumable() {
            if (character.characterInventoryManager.currentConsumable != null) {
                character.characterInventoryManager.currentConsumable.SuccessfullyConsumedItem(player);
            }
        }

        public void FinishJump() {
            if (player.isJumping)
                player.isJumping = false;
        }

        public void FinishLadderInteractionAtTop() {
            player.isMoving = false;
        }
    }
}
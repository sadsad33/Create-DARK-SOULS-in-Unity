using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �÷��̾ ���� Update �Լ��� ó��
// �÷��̾��� ���� Flag�� ó���Ѵ�.
// �÷��̾��� ���� ��ɵ��� �����Ѵ�.
namespace sg {
    public class PlayerManager : CharacterManager {
        InputHandler inputHandler;
        Animator anim;
        public GameObject interactableUIGameObject; // ��ȣ�ۿ� �޼��� (�� ����, ���� ������ ��)
        public GameObject itemInteractableGameObject; // ������ ȹ�� �޼���

        PlayerAnimatorManager playerAnimatorManager;
        PlayerStatsManager playerStatsManager;
        PlayerEffectsManager playerEffectsManager;
        PlayerLocomotionManager playerLocomotion;
        CameraHandler cameraHandler;
        InteractableUI interactableUI; // ��ȣ�ۿ붧 ��Ÿ���� �޼��� â

        private void Awake() {
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            backStabCollider = GetComponentInChildren<CriticalDamageCollider>();
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponent<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotionManager>();
            interactableUI = FindObjectOfType<InteractableUI>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();
        }

        void Update() {
            float delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            isFiringSpell = anim.GetBool("isFiringSpell");
            canDoCombo = anim.GetBool("canDoCombo");
            anim.SetBool("isInAir", isInAir);
            isUsingRightHand = anim.GetBool("isUsingRightHand");
            isUsingLeftHand = anim.GetBool("isUsingLeftHand");
            inputHandler.TickInput(delta);
            isInvulnerable = anim.GetBool("isInvulnerable");
            anim.SetBool("isDead", playerStatsManager.isDead);
            anim.SetBool("isBlocking", isBlocking);
            // Rigidbody�� �̵��Ǵ� �������� �ƴ϶�� �Ϲ����� Update�Լ����� ȣ���ص� ������.
            playerLocomotion.HandleRollingAndSprinting(delta);
            playerLocomotion.HandleJumping();
            playerStatsManager.RegenerateStamina();
            CheckForInteractableObject();

            playerAnimatorManager.canRotate = anim.GetBool("canRotate");

            // �̵�Ű�� �齺��Ű�� ª�� �������� ������ �齺�� ���� sprint �ִϸ��̼��� ����Ǵ� ��찡 �ִ�.
            // �̸� �ذ��ϱ� ���� delay �߰�
            if (inputHandler.moveAmount == 0) {
                inputHandler.backstepDelay += delta;
            } else {
                inputHandler.backstepDelay = 0;
            }
        }

        private void FixedUpdate() {
            float delta = Time.fixedDeltaTime;

            // Rigidbody�� ���� ó���Ǵ� �������� FixedUpdate���� ó���Ǵ°��� ����
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
            playerLocomotion.HandleRotation(delta);
            playerEffectsManager.HandleAllBuildUpEffects();
        }

        private void LateUpdate() {
            // 1�����Ӵ� �ѹ��� ȣ�⸸ �̷������� �Ѵ�.
            inputHandler.rollFlag = false;
            inputHandler.rb_Input = false;
            inputHandler.rt_Input = false;
            inputHandler.lt_Input = false;
            inputHandler.d_Pad_Up = false;
            inputHandler.d_Pad_Down = false;
            inputHandler.d_Pad_Left = false;
            inputHandler.d_Pad_Right = false;
            inputHandler.a_Input = false;
            inputHandler.jump_Input = false;
            inputHandler.inventory_Input = false;

            float delta = Time.deltaTime;
            if (cameraHandler != null) {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }

            if (isInAir) { // �÷��̾ ����� �ִٸ�
                playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
            }
        }

        #region Player Interactions

        public void CheckForInteractableObject() {
            if (isInteracting) return;
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayer)) {
                if (hit.collider.tag == "Interactable") {
                    Interactable interactableObject = hit.collider.GetComponent<Interactable>();
                    if (interactableObject != null) {
                        string interactableText = interactableObject.interactableText;
                        interactableUI.interactableText.text = interactableText;
                        interactableUIGameObject.SetActive(true);

                        if (inputHandler.a_Input) {
                            interactableUIGameObject.SetActive(false);
                            hit.collider.GetComponent<Interactable>().Interact(this);
                        }
                    }
                }
            } else {
                // �ֺ��� ��ȣ�ۿ밡���� ������Ʈ�� �������� �޼���â�� ������ �ʵ���
                if (interactableUIGameObject != null) {
                    interactableUIGameObject.SetActive(false);
                }

                // �������� �����ϰ� ���� ����Ű�� �ѹ� �� ������ �޽���â�� ������.
                if (itemInteractableGameObject != null && inputHandler.a_Input) {
                    itemInteractableGameObject.SetActive(false);
                }
            }
        }

        public void OpenChestInteraction(Transform playerStandingPosition) {
            // �޸��ٰ� ��ȣ�ۿ��� �� ��� �̲������°��� ����
            playerLocomotion.rigidbody.velocity = Vector3.zero;
            transform.position = playerStandingPosition.transform.position;
            playerAnimatorManager.PlayTargetAnimation("Open Chest", true);
        }

        // �Ȱ��� ��� ��ȣ�ۿ�
        public void PassThroughFogWallInteraction(Transform fogWallEntrance) {
            playerLocomotion.rigidbody.velocity = Vector3.zero;
            Vector3 rotationDirection = fogWallEntrance.transform.forward;
            Quaternion turnRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = turnRotation;

            playerAnimatorManager.PlayTargetAnimation("PassThroughFog", true);
        }
        #endregion
    }
}
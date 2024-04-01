using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �÷��̾ ���� Update �Լ��� ó��
// �÷��̾��� ���� Flag�� ó���Ѵ�.
// �÷��̾��� ���� ��ɵ��� �����Ѵ�.

namespace SoulsLike {
    public class PlayerManager : CharacterManager {
        InputHandler inputHandler;
        Animator anim;
        public GameObject interactableUIGameObject; // ��ȣ�ۿ� �޼��� (�� ����, ���� ������ ��) : InteractionPopUp
        public GameObject itemInteractableGameObject; // ������ ȹ�� �޼��� : ItemPopup
        public GameObject dialogUI; // NPC�� ��縦 ����� â
        public Transform leftFoot, rightFoot;
        public LadderEndPositionDetection ladderEndPositionDetector;

        // ��ũ�ҿ� �ø������ ��ȭ ���� �ൿ�� �����ϹǷ� isInteracting �� �и�
        public bool isInConversation;
        public bool isJumping = false;
        public bool rightFootUp;
        public bool isLadderTop;
        private float turnPageTimer;
        private readonly float turnPageTime = 10f;
        private int currentPageIndex;

        PlayerAnimatorManager playerAnimatorManager;
        PlayerStatsManager playerStatsManager;
        PlayerEffectsManager playerEffectsManager;
        PlayerLocomotionManager playerLocomotion;
        public NPCScript[] currentDialog;
        CameraHandler cameraHandler;

        [SerializeField]
        InteractableUI interactableUI; // ��ȣ�ۿ붧 ��Ÿ���� �޼��� â
        LayerMask interactableLayer;

        protected override void Awake() {
            base.Awake();
            ladderEndPositionDetector = GetComponentInChildren<LadderEndPositionDetection>();
            if (ladderEndPositionDetector != null) ladderEndPositionDetector.transform.gameObject.SetActive(false);
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            backStabCollider = GetComponentInChildren<CriticalDamageCollider>();
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponent<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotionManager>();
            interactableUI = FindObjectOfType<InteractableUI>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();
            interactableLayer = 1 << 0 | 1 << 9;
        }

        void Update() {
            if (isClimbing) {
                if (leftFoot.position.y > rightFoot.position.y) {
                    rightFootUp = false;
                } else if (leftFoot.position.y < rightFoot.position.y) {
                    rightFootUp = true;
                }
            }

            float delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            isFiringSpell = anim.GetBool("isFiringSpell");
            canDoCombo = anim.GetBool("canDoCombo");
            anim.SetBool("isInAir", isInAir);
            isUsingRightHand = anim.GetBool("isUsingRightHand");
            isUsingLeftHand = anim.GetBool("isUsingLeftHand");
            inputHandler.TickInput(delta);
            isTwoHandingWeapon = inputHandler.twoHandFlag;
            isInvulnerable = anim.GetBool("isInvulnerable");
            anim.SetBool("isDead", playerStatsManager.isDead);
            anim.SetBool("isBlocking", isBlocking);

            // ��ٸ� ����
            anim.SetBool("isClimbing", isClimbing);
            anim.SetBool("isLadderTop", isLadderTop);
            anim.SetBool("rightFootUp", rightFootUp);
            HandleConversation();

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

        protected override void FixedUpdate() {
            base.FixedUpdate();
            float delta = Time.fixedDeltaTime;

            // Rigidbody�� ���� ó���Ǵ� �������� FixedUpdate���� ó���Ǵ°��� ����
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
            playerLocomotion.HandleRotation(delta);
            playerLocomotion.MaintainVelocity();
            if (isClimbing)
                playerLocomotion.HandleClimbing();
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

            // ��ٸ� ��ȣ�ۿ� �ִϸ��̼��� ������ Animator�� ���̾�5 ������ Empty �ִϸ��̼����� ���̰� �Ͼ
            // ��ٸ��� ����� �κп��� ��ȣ�ۿ� �ִϸ��̼��� ����� ��� �ִϸ��̼��� ������ Rigidbody �� velocity �� 0�� �����ʾ� Ư�� �������� ���Ӿ��� �з������µ�
            // �̴� PlayerAnimatorManager Ŭ�������� OnAnimatorMove �޼��� �����ΰ����� ����
            // �ش� �޼���� �ִϸ��̼��� ����� �� ������ �����ӱ��� ȣ��Ǹ�, �ִϸ��̼��� ����Ǵµ���
            // �ƹ�Ÿ�� ������ ��ǥ�� ��ȭ���� �����ϰ� �ִϸ��̼� ����� �ð����� ���� �ӵ��� ������ �÷��̾��� rigidbody �� �����Ͽ� �ڿ������Ժ��̵��� �ӵ��� ������
            // �ִϸ��̼��� ���̰� ��� �ִϸ��̼��� ������ ������ ���� ����Ǵ� �Ŷ�� ��������� �ڿ������� ���̸� ���� ��κ� �׷��� �����Ƿ� Rigidbody�� velocity �� 0�� �Ǿ��ٰ� ������ �����ӿ���
            // �ٽ� OnAnimatorMove �޼��尡 ȣ��Ǹ鼭 Rigidbody �� velocity ���� ��ȭ�ϴ� ��
            if (playerAnimatorManager.anim.GetCurrentAnimatorClipInfoCount(5) == 0) { // �� �����Ӹ��� 5�� ���̾��� ���� �ִϸ��̼��� Empty ��� rigidbody �� �ӵ��� 0���� ����
                playerLocomotion.rigidbody.velocity = Vector3.zero;
            }

            float delta = Time.deltaTime;
            if (cameraHandler != null) {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }

            if (isInAir) { // �÷��̾ ����� �ִٸ�
                playerLocomotion.inAirTimer += Time.deltaTime;
            }
        }

        #region �÷��̾� ��ȣ�ۿ�

        CharacterManager character;
        public void CheckForInteractableObject() {
            if (isInteracting || isInConversation) return; // �ൿ ���̰ų� ��ȭ���̶�� �ٸ� ������Ʈ�� NPC�� ��ȣ�ۿ� �Ұ�

            Interactable interactableObject; // �ֺ� ��ȣ�ۿ� ������ ������Ʈ�� ���� ����
            if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out RaycastHit hit, 1f, interactableLayer)) {
                if (hit.collider.CompareTag("Interactable")) {
                    interactableObject = hit.collider.GetComponent<Interactable>();
                    if (interactableObject != null) { // �ֺ��� ��ȣ�ۿ� ������ ��ü�� �ִٸ�
                        if (!itemInteractableGameObject.activeSelf) {
                            StartInteraction(interactableObject, hit, false);
                        } else if (inputHandler.a_Input) {
                            itemInteractableGameObject.SetActive(false);
                        }
                    }
                } else if (hit.collider.CompareTag("Character")) {
                    interactableObject = hit.collider.GetComponent<Interactable>();
                    character = hit.collider.GetComponent<CharacterManager>();
                    if (character.canTalk) {
                        if (interactableObject != null) {
                            if (!itemInteractableGameObject.activeSelf) {
                                StartInteraction(interactableObject, hit, false);
                            } else if (inputHandler.a_Input) {
                                itemInteractableGameObject.SetActive(false);
                            }
                        }
                    }
                }
            } else {
                // �ֺ��� ��ȣ�ۿ밡���� ������Ʈ�� �������� �޼���â�� ������ �ʵ���
                if (interactableUIGameObject != null) {
                    interactableUIGameObject.SetActive(false);
                }
                //�������� �����ϰ� ���� ����Ű�� �ѹ� �� ������ �޽���â�� ������.
                if (itemInteractableGameObject != null && inputHandler.a_Input) {
                    itemInteractableGameObject.SetActive(false);
                }
            }
        }

        private void StartInteraction(Interactable interactableObject, RaycastHit hit, bool withCharacter) {
            string interactableText = interactableObject.interactableText;
            interactableUI.interactableText.text = interactableText;
            interactableUIGameObject.SetActive(true);

            if (inputHandler.a_Input) {
                interactableUIGameObject.SetActive(false);
                hit.collider.GetComponent<Interactable>().Interact(this); // �ش� ������Ʈ�� Interact �� ����
                if (withCharacter) currentPageIndex = 0;
            }
        }

        // ��ȭ ����
        private void HandleConversation() {
            if (isInConversation) { // ���� ��ȭ���̰�
                float distance = Vector3.Distance(character.transform.position, transform.position);
                // ��縦 ��� ������� �ʾҴٸ�
                if (currentPageIndex <= currentDialog.Length - 1) {
                    PrintDialog();
                    if (distance >= 3f) FinishConversation();
                } else if (turnPageTimer <= 0) {
                    NPCManager npc = character.GetComponent<NPCManager>();
                    npc.interactCount++; // ����� ���������������� �о��ٸ� ��ȣ�ۿ� Ƚ���� �������� ���� ��ȣ�ۿ�� �ٸ� ��縦 ����ϰ� ��
                    FinishConversation();
                }
            }
        }

        // ��ȭ ����
        private void FinishConversation() {
            isInConversation = false;
            currentPageIndex = 0;
            dialogUI.SetActive(false);
        }

        // ��� ���
        private void PrintDialog() {
            if (!dialogUI.activeSelf) dialogUI.SetActive(true);
            if (turnPageTimer == 0)
                turnPageTimer = turnPageTime;
            interactableUI.dialogText.text = currentDialog[currentPageIndex].script;
            HandleTurnPageTimer();
            if (turnPageTimer == 0 && currentPageIndex < currentDialog.Length) {
                currentPageIndex++;
            }
        }

        // ���� ��� ��� Ÿ�̸�
        private void HandleTurnPageTimer() {
            if (turnPageTimer <= 0) turnPageTimer = 0;
            else {
                turnPageTimer -= Time.deltaTime;
                if (inputHandler.a_Input) turnPageTimer -= turnPageTime;
            }
        }

        public void InteractionAtPosition(string animation, Transform playerStandingPosition) {
            characterWeaponSlotManager.rightHandSlot.UnloadWeapon();
            characterWeaponSlotManager.leftHandSlot.UnloadWeapon();
            playerLocomotion.rigidbody.velocity = Vector3.zero;
            playerAnimatorManager.PlayTargetAnimation(animation, true);
            transform.position = playerStandingPosition.position;
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
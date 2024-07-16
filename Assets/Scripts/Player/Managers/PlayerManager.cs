using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// �÷��̾ ���� Update �Լ��� ó��
// �÷��̾��� ���� Flag�� ó���Ѵ�.
// �÷��̾��� ���� ��ɵ��� �����Ѵ�.

namespace SoulsLike {
    public class PlayerManager : CharacterManager {
        public InputHandler inputHandler;
        //public Animator anim;
        public GameObject interactableUIGameObject; // ��ȣ�ۿ� �޼��� (�� ����, ���� ������ ��) : InteractionPopUp
        public GameObject itemInteractableGameObject; // ������ ȹ�� �޼��� : ItemPopup
        public GameObject dialogUI; // NPC�� ��縦 ����� â
        public Transform leftFoot, rightFoot;
        public LadderEndPositionDetection ladderEndPositionDetector;
        public PlayerNetworkManager playerNetworkManager;
        public PlayerInventoryManager playerInventoryManager;
        public PlayerWeaponSlotManager playerWeaponSlotManager;
        // ��ũ�ҿ� �ø������ ��ȭ ���� �ൿ�� �����ϹǷ� isInteracting �� �и�
        public bool isInConversation;

        public bool isJumping = false;
        public bool rightFootUp;
        public bool isLadderTop;
        public bool isAtBonfire;
        private float turnPageTimer;
        private readonly float turnPageTime = 10f;
        private int currentPageIndex;

        public PlayerEquipmentManager playerEquipmentManager;
        public PlayerAnimatorManager playerAnimatorManager;
        public PlayerStatsManager playerStatsManager;
        public PlayerEffectsManager playerEffectsManager;
        public PlayerLocomotionManager playerLocomotion;
        public NPCScript[] currentDialog;
        public CameraHandler cameraHandler;

        [SerializeField]
        InteractableUI interactableUI; // ��ȣ�ۿ붧 ��Ÿ���� �޼��� â
        LayerMask interactableLayer;

        protected override void Awake() {
            base.Awake();
            Debug.Log("Player.Awake()");
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
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

            // Ÿ��Ʋ ��ũ������ �÷��̾ �Ѱܹޱ� ����
            WorldSaveGameManager.instance.player = this;
        }
        private void Start() {
            DontDestroyOnLoad(this);
        }

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            if (IsOwner) {
                playerStatsManager.SetStatBarsHUD();
                CameraHandler.instance.AssignCameraToPlayer(this);
                UIManager.instance.playerStatsManager = playerStatsManager;
                UIManager.instance.playerInventory = transform.GetComponent<PlayerInventoryManager>();
                interactableUIGameObject = UIManager.instance.transform.GetChild(0).GetChild(2).GetChild(0).gameObject;
                itemInteractableGameObject = UIManager.instance.transform.GetChild(0).GetChild(2).GetChild(1).gameObject;
                dialogUI = UIManager.instance.transform.GetChild(0).GetChild(2).GetChild(2).gameObject;
            }
        }

        protected override void Update() {
            base.Update();
            if (!IsOwner) return;
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

            // ȭ���
            anim.SetBool("isAtBonefire", isAtBonfire);

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

        public Transform interactionTargetPosition;
        protected override void FixedUpdate() {
            base.FixedUpdate();
            if (!IsOwner) return;
            float delta = Time.fixedDeltaTime;

            // Rigidbody�� ���� ó���Ǵ� �������� FixedUpdate���� ó���Ǵ°��� ����
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
            playerLocomotion.HandleRotation(delta);
            playerLocomotion.MaintainVelocity();
            if (isClimbing)
                playerLocomotion.HandleClimbing();
            if (isMoving)
                transform.position = Vector3.Slerp(transform.position, interactionTargetPosition.position, Time.deltaTime);
            playerEffectsManager.HandleAllBuildUpEffects();
        }

        private void LateUpdate() {
            if (!IsOwner) return;
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
            if ((isClimbing || isAtBonfire) && anim.GetCurrentAnimatorClipInfoCount(5) == 0) { // �� �����Ӹ��� 5�� ���̾��� ���� �ִϸ��̼��� Empty ��� rigidbody �� �ӵ��� 0���� ����
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
            if (isInteracting || isInConversation || isClimbing || isAtBonfire) return; // �ൿ ���̰ų� ��ȭ���̶�� �ٸ� ������Ʈ�� NPC�� ��ȣ�ۿ� �Ұ�

            Interactable interactableObject; // �ֺ� ��ȣ�ۿ� ������ ������Ʈ�� ���� ����
            if (Physics.SphereCast(transform.position + transform.forward * 0.15f, 0.2f, transform.forward, out RaycastHit hit, 1f, interactableLayer)) {
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

        public void FinsihRest() {
            isAtBonfire = false;
            playerAnimatorManager.PlayTargetAnimation("Bonfire_End", true);
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

        // �ش� ��ġ���� �ش� �ִϸ��̼��� ���� ��ȣ�ۿ� ����
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

        // ������ ���̺� ���Ͽ� ĳ���� ������ ����
        public void SaveCharacterDataToCurrentSaveData(ref CharacterSaveData currentCharacterSaveData) {
            currentCharacterSaveData.characterName = playerStatsManager.characterName;
            currentCharacterSaveData.characterLevel = playerStatsManager.level;
            currentCharacterSaveData.xPosition = transform.position.x;
            currentCharacterSaveData.yPosition = transform.position.y;
            currentCharacterSaveData.zPosition = transform.position.z;

            currentCharacterSaveData.currentRightHandWeaponID = playerInventoryManager.rightWeapon.itemID;
            currentCharacterSaveData.currentLeftHandWeaponID = playerInventoryManager.leftWeapon.itemID;

            if (playerInventoryManager.currentHelmetEquipment != null)
                currentCharacterSaveData.currentHeadGearItemID = playerInventoryManager.currentHelmetEquipment.itemID;
            else currentCharacterSaveData.currentHeadGearItemID = -1;

            if (playerInventoryManager.currentTorsoEquipment != null)
                currentCharacterSaveData.currentChestGearItemID = playerInventoryManager.currentTorsoEquipment.itemID;
            else currentCharacterSaveData.currentChestGearItemID = -1;

            if (playerInventoryManager.currentGuntletEquipment != null)
                currentCharacterSaveData.currentHandGearItemID = playerInventoryManager.currentGuntletEquipment.itemID;
            else currentCharacterSaveData.currentHandGearItemID = -1;

            if (playerInventoryManager.currentLegEquipment != null)
                currentCharacterSaveData.currentLegGearItemID = playerInventoryManager.currentLegEquipment.itemID;
            else currentCharacterSaveData.currentLegGearItemID = -1;
        }

        public void LoadCharacterDataFromCurrentCharacterSaveData(ref CharacterSaveData currentCharacterSaveData) {
            playerStatsManager.characterName = currentCharacterSaveData.characterName;
            playerStatsManager.level = currentCharacterSaveData.characterLevel;

            transform.position = new Vector3(currentCharacterSaveData.xPosition, currentCharacterSaveData.yPosition, currentCharacterSaveData.zPosition);
            playerInventoryManager.rightWeapon = WorldItemDatabase.instance.GetWeaponItemByID(currentCharacterSaveData.currentRightHandWeaponID);
            playerInventoryManager.leftWeapon = WorldItemDatabase.instance.GetWeaponItemByID(currentCharacterSaveData.currentLeftHandWeaponID);
            playerWeaponSlotManager.LoadBothWeaponsOnSlots();

            EquipmentItem headEquipment = WorldItemDatabase.instance.GetEquipmentItemByID(currentCharacterSaveData.currentHeadGearItemID);
            // ������ ���̽��� �ش� �������� �����ϸ� ����
            if (headEquipment != null) {
                playerInventoryManager.currentHelmetEquipment = headEquipment as HelmetEquipment;
            }

            EquipmentItem bodyEquipment = WorldItemDatabase.instance.GetEquipmentItemByID(currentCharacterSaveData.currentChestGearItemID);
            if (bodyEquipment != null) {
                playerInventoryManager.currentTorsoEquipment = bodyEquipment as TorsoEquipment;
            }

            EquipmentItem handEquipment = WorldItemDatabase.instance.GetEquipmentItemByID(currentCharacterSaveData.currentHandGearItemID);
            if (handEquipment != null) {
                playerInventoryManager.currentGuntletEquipment = handEquipment as GuntletEquipment;
            }

            EquipmentItem legEquipment = WorldItemDatabase.instance.GetEquipmentItemByID(currentCharacterSaveData.currentLegGearItemID);
            if (bodyEquipment != null) {
                playerInventoryManager.currentLegEquipment = legEquipment as LegEquipment;
            }
            playerEquipmentManager.EquipAllEquipmentModels();
        }
    }
}
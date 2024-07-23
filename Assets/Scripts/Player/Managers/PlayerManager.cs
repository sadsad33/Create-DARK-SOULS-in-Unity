using System;
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
            animator = GetComponent<Animator>();
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
        
        // 이벤트 핸들러
        private void OnClientConnectedCallback(ulong clientID) {
            Debug.Log("Client Connected! Client ID : " + clientID);
            GameSessionManager.instance.AddPlayerToActivePlayerList(this);

            // 호스트가 아닌 단순 클라이언트라면 
            if (!IsServer && IsOwner) {
                foreach (var player in GameSessionManager.instance.players) {
                    if (player != this) {
                        player.LoadOtherPlayerCharacterWhenJoiningOnline(player);
                    }
                }
            }
        }

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

            if (IsOwner) {
                playerStatsManager.SetStatBarsHUD();
                CameraHandler.instance.AssignCameraToPlayer(this);
                UIManager.instance.playerStatsManager = playerStatsManager;
                UIManager.instance.playerInventory = transform.GetComponent<PlayerInventoryManager>();
                interactableUIGameObject = UIManager.instance.transform.GetChild(0).GetChild(2).GetChild(0).gameObject;
                itemInteractableGameObject = UIManager.instance.transform.GetChild(0).GetChild(2).GetChild(1).gameObject;
                dialogUI = UIManager.instance.transform.GetChild(0).GetChild(2).GetChild(2).gameObject;
            }
            playerNetworkManager.currentRightWeaponID.OnValueChanged += playerNetworkManager.OnRightWeaponChange;
            playerNetworkManager.currentLeftWeaponID.OnValueChanged += playerNetworkManager.OnLeftWeaponChange;
            playerNetworkManager.currentHeadEquipmentID.OnValueChanged += playerNetworkManager.OnHeadEquipmentChange;
            playerNetworkManager.currentTorsoEquipmentID.OnValueChanged += playerNetworkManager.OnTorsoEquipmentChange;
            playerNetworkManager.currentGuntletEquipmentID.OnValueChanged += playerNetworkManager.OnGuntletEquipmentChange;
            playerNetworkManager.currentLegEquipmentID.OnValueChanged += playerNetworkManager.OnLegEquipmentChange;
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
            isInteracting = animator.GetBool("isInteracting");
            isFiringSpell = animator.GetBool("isFiringSpell");
            canDoCombo = animator.GetBool("canDoCombo");
            isUsingRightHand = animator.GetBool("isUsingRightHand");
            isUsingLeftHand = animator.GetBool("isUsingLeftHand");
            inputHandler.TickInput(delta);
            isTwoHandingWeapon = inputHandler.twoHandFlag;
            isInvulnerable = animator.GetBool("isInvulnerable");
            animator.SetBool("isDead", playerStatsManager.isDead);
            animator.SetBool("isBlocking", isBlocking);

            // ȭ���
            animator.SetBool("isAtBonefire", isAtBonfire);

            // ��ٸ� ����
            animator.SetBool("isClimbing", isClimbing);
            animator.SetBool("isLadderTop", isLadderTop);
            animator.SetBool("rightFootUp", rightFootUp);
            HandleConversation();

            // Rigidbody�� �̵��Ǵ� �������� �ƴ϶�� �Ϲ����� Update�Լ����� ȣ���ص� ������.
            playerLocomotion.HandleRollingAndSprinting(delta);
            playerLocomotion.HandleJumping();
            // Rigidbody�� ���� ó���Ǵ� �������� FixedUpdate���� ó���Ǵ°��� ����
            playerLocomotion.HandleGroundedMovement(delta);
            playerLocomotion.HandleRotation(delta);
            playerLocomotion.MaintainVelocity();
            playerStatsManager.RegenerateStamina();
            CheckForInteractableObject();

            playerAnimatorManager.canRotate = animator.GetBool("canRotate");

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

            //playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
            
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
            if ((isClimbing || isAtBonfire) && animator.GetCurrentAnimatorClipInfoCount(5) == 0) { // �� �����Ӹ��� 5�� ���̾��� ���� �ִϸ��̼��� Empty ��� rigidbody �� �ӵ��� 0���� ����
                playerLocomotion.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }

            float delta = Time.deltaTime;
            if (cameraHandler != null) {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
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
            playerLocomotion.GetComponent<Rigidbody>().velocity = Vector3.zero;
            playerAnimatorManager.PlayTargetAnimation(animation, true);
            transform.position = playerStandingPosition.position;
        }

        // �Ȱ��� ��� ��ȣ�ۿ�
        public void PassThroughFogWallInteraction(Transform fogWallEntrance) {
            playerLocomotion.GetComponent<Rigidbody>().velocity = Vector3.zero;
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

            // 현재 무기 저장
            currentCharacterSaveData.currentRightHandWeaponID = playerInventoryManager.rightWeapon.itemID;
            currentCharacterSaveData.currentLeftHandWeaponID = playerInventoryManager.leftWeapon.itemID;

            // 현재 갑옷 저장
            // 식별번호로 -1은 사용하지 않으므로 해당 부위에 아무런 장비도 없다면 -1을 저장
            // LINQ를 이용해 값을 찾을때 null을 반환하게 됨
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

            // 현재 반지 저장
            if (playerInventoryManager.ringSlot01 != null)
                currentCharacterSaveData.currentRingSlot01ItemID = playerInventoryManager.ringSlot01.itemID;
            else currentCharacterSaveData.currentRingSlot01ItemID = -1;

            if (playerInventoryManager.ringSlot02 != null)
                currentCharacterSaveData.currentRingSlot02ItemID = playerInventoryManager.ringSlot02.itemID;
            else currentCharacterSaveData.currentRingSlot02ItemID = -1;

            if (playerInventoryManager.ringSlot03 != null)
                currentCharacterSaveData.currentRingSlot03ItemID = playerInventoryManager.ringSlot03.itemID;
            else currentCharacterSaveData.currentRingSlot03ItemID = -1;

            if (playerInventoryManager.ringSlot04 != null)
                currentCharacterSaveData.currentRingSlot04ItemID = playerInventoryManager.ringSlot04.itemID;
            else currentCharacterSaveData.currentRingSlot04ItemID = -1;
        }

        [Obsolete(" 플레이어의 장비 UI에 반지를 장착/해제 할수 있는 부분이 없으므로 반지의 장착 해제 메서드는 호출되는 부분이 없음. 따라서 반지를 장착하고 있다가 해제하고 세이브해도 로드 하는 과정에서 반영이 되지 않는 문제가 있음")]
        public void LoadCharacterDataFromCurrentCharacterSaveData(ref CharacterSaveData currentCharacterSaveData) {
            playerStatsManager.characterName = currentCharacterSaveData.characterName;
            playerStatsManager.level = currentCharacterSaveData.characterLevel;

            transform.position = new Vector3(currentCharacterSaveData.xPosition, currentCharacterSaveData.yPosition, currentCharacterSaveData.zPosition);
            playerInventoryManager.rightWeapon = WorldItemDatabase.instance.GetWeaponItemByID(currentCharacterSaveData.currentRightHandWeaponID);
            playerInventoryManager.leftWeapon = WorldItemDatabase.instance.GetWeaponItemByID(currentCharacterSaveData.currentLeftHandWeaponID);
            playerWeaponSlotManager.LoadBothWeaponsOnSlots();

            //EquipmentItem headEquipment = WorldItemDatabase.instance.GetEquipmentItemByID(currentCharacterSaveData.currentHeadGearItemID);
            //// ������ ���̽��� �ش� �������� �����ϸ� ����
            //Debug.Log(headEquipment);
            //if (headEquipment != null) {
            //    playerInventoryManager.currentHelmetEquipment = headEquipment as HelmetEquipment;
            //} else {
            //    playerInventoryManager.currentHelmetEquipment = null;
            //}
            EquipmentItem headEquipment = WorldItemDatabase.instance.GetEquipmentItemByID(currentCharacterSaveData.currentHeadGearItemID);
            playerInventoryManager.currentHelmetEquipment = headEquipment == null ? null : headEquipment as HelmetEquipment; 

            EquipmentItem bodyEquipment = WorldItemDatabase.instance.GetEquipmentItemByID(currentCharacterSaveData.currentChestGearItemID);
            playerInventoryManager.currentTorsoEquipment = bodyEquipment == null ? null : bodyEquipment as TorsoEquipment;

            EquipmentItem handEquipment = WorldItemDatabase.instance.GetEquipmentItemByID(currentCharacterSaveData.currentHandGearItemID);
            playerInventoryManager.currentGuntletEquipment = handEquipment == null ? null : handEquipment as GuntletEquipment;

            EquipmentItem legEquipment = WorldItemDatabase.instance.GetEquipmentItemByID(currentCharacterSaveData.currentLegGearItemID);
            playerInventoryManager.currentLegEquipment = legEquipment == null ? null : legEquipment as LegEquipment;
            
            playerEquipmentManager.EquipAllEquipmentModels();

            RingItem slot01Ring = WorldItemDatabase.instance.GetRingItemByID(currentCharacterSaveData.currentRingSlot01ItemID);
            playerInventoryManager.ringSlot01 = slot01Ring == null ? null : slot01Ring;
            
            RingItem slot02Ring = WorldItemDatabase.instance.GetRingItemByID(currentCharacterSaveData.currentRingSlot02ItemID);
            playerInventoryManager.ringSlot02 = slot02Ring == null ? null : slot02Ring;

            RingItem slot03Ring = WorldItemDatabase.instance.GetRingItemByID(currentCharacterSaveData.currentRingSlot03ItemID);
            playerInventoryManager.ringSlot03 = slot03Ring == null ? null : slot03Ring;

            RingItem slot04Ring = WorldItemDatabase.instance.GetRingItemByID(currentCharacterSaveData.currentRingSlot04ItemID);
            playerInventoryManager.ringSlot04 = slot04Ring == null ? null : slot04Ring;

            playerInventoryManager.LoadRingEffect();
        }

        public void LoadOtherPlayerCharacterWhenJoiningOnline(PlayerManager player) {
            Debug.Log("다른 플레이어 캐릭터 로드");
            player.playerNetworkManager.OnRightWeaponChange(0, player.playerNetworkManager.currentRightWeaponID.Value);
            player.playerNetworkManager.OnLeftWeaponChange(0, player.playerNetworkManager.currentLeftWeaponID.Value);
            player.playerNetworkManager.OnHeadEquipmentChange(0, player.playerNetworkManager.currentHeadEquipmentID.Value);
            player.playerNetworkManager.OnTorsoEquipmentChange(0, player.playerNetworkManager.currentTorsoEquipmentID.Value);
            player.playerNetworkManager.OnGuntletEquipmentChange(0, player.playerNetworkManager.currentGuntletEquipmentID.Value);
            player.playerNetworkManager.OnLegEquipmentChange(0, player.playerNetworkManager.currentLegEquipmentID.Value);
        }
    }
}
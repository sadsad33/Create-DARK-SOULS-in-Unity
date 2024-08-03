using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SoulsLike {
    public class PlayerManager : CharacterManager {
        
        [Header("Input")]
        public InputHandler inputHandler;

        [Header("Camera")]
        public CameraHandler cameraHandler;

        public bool isJumping = false;
        public bool rightFootUp;
        public bool isLadderTop;
        public bool isAtBonfire;
        public bool isInConversation;

        public Transform leftFoot, rightFoot;
        public LadderEndPositionDetection ladderEndPositionDetector;

        public PlayerCombatManager playerCombatManager;
        public PlayerNetworkManager playerNetworkManager;
        public PlayerInventoryManager playerInventoryManager;
        public PlayerWeaponSlotManager playerWeaponSlotManager;
        public PlayerEquipmentManager playerEquipmentManager;
        public PlayerAnimatorManager playerAnimatorManager;
        public PlayerStatsManager playerStatsManager;
        public PlayerEffectsManager playerEffectsManager;
        public PlayerLocomotionManager playerLocomotionManager;
        public PlayerInteractionManager playerInteractionManager;
        
        //[SerializeField]
        //InteractableUI interactableUI;
        //LayerMask interactableLayer;

        protected override void Awake() {
            base.Awake();

            playerCombatManager = GetComponent<PlayerCombatManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            inputHandler = GetComponent<InputHandler>();
            animator = GetComponent<Animator>();
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();
            backStabCollider = GetComponentInChildren<CriticalDamageCollider>();
            playerInteractionManager = GetComponentInChildren<PlayerInteractionManager>();
            ladderEndPositionDetector = GetComponentInChildren<LadderEndPositionDetection>();
            if (ladderEndPositionDetector != null) ladderEndPositionDetector.transform.gameObject.SetActive(false);
            //interactableUI = FindObjectOfType<InteractableUI>();
            //interactableLayer = 1 << 0 | 1 << 9;

            WorldSaveGameManager.instance.player = this;
        }
        protected override void Start() {
            base.Start();
            DontDestroyOnLoad(this);
        }

        // 이벤트 핸들러
        private void OnClientConnectedCallback(ulong clientID) {
            //Debug.Log("Client Connected! Client ID : " + clientID);
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
            if (isClimbing) {
                playerLocomotionManager.HandleClimbing();
                if (leftFoot.position.y > rightFoot.position.y) {
                    rightFootUp = false;
                } else if (leftFoot.position.y < rightFoot.position.y) {
                    rightFootUp = true;
                }
            }
            if (isMoving) {
                characterController.enabled = false;
                transform.position = Vector3.Slerp(transform.position, interactionTargetPosition.position, Time.deltaTime);
            }

            float delta = Time.deltaTime;
            isInteracting = animator.GetBool("isInteracting");
            isFiringSpell = animator.GetBool("isFiringSpell");
            canDoCombo = animator.GetBool("canDoCombo");
            isInvulnerable = animator.GetBool("isInvulnerable");
            animator.SetBool("isDead", playerStatsManager.isDead);
            animator.SetBool("isBlocking", characterNetworkManager.isBlocking.Value);

            animator.SetBool("isAtBonefire", isAtBonfire);

            animator.SetBool("isClimbing", isClimbing);
            animator.SetBool("isLadderTop", isLadderTop);
            animator.SetBool("rightFootUp", rightFootUp);
            playerAnimatorManager.canRotate = animator.GetBool("canRotate");

            if (!IsOwner) return;

            inputHandler.TickInput(delta);
            characterNetworkManager.isTwoHandingWeapon.Value = inputHandler.twoHandFlag;

            playerLocomotionManager.HandleRollingAndSprinting(delta);
            playerLocomotionManager.HandleJumping();
            playerLocomotionManager.HandleGroundedMovement(delta);
            playerLocomotionManager.HandleRotation(delta);
            playerLocomotionManager.MaintainVelocity();
            playerStatsManager.RegenerateStamina();
            playerInteractionManager.HandleConversation();
            //CheckForInteractableObject();
            playerInteractionManager.TryInteraction();

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
                playerLocomotionManager.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }

            float delta = Time.deltaTime;
            if (cameraHandler != null) {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
        }

        //CharacterManager character;
        //public void CheckForInteractableObject() {
        //    if (isInteracting || isInConversation || isClimbing || isAtBonfire) return;
        //    Interactable interactableObject;
        //    if (Physics.SphereCast(transform.root.position + (transform.forward * 0.2f), 0.5f, transform.forward, out RaycastHit hit, 1f, interactableLayer)) {
        //        if (hit.collider.CompareTag("Interactable")) {
        //            interactableObject = hit.collider.GetComponent<Interactable>();
        //            if (interactableObject != null) {
        //                if (!UIManager.instance.itemInteractableGameObject.activeSelf) {
        //                    StartInteraction(interactableObject, hit, false);
        //                } else if (inputHandler.a_Input) {
        //                    UIManager.instance.itemInteractableGameObject.SetActive(false);
        //                }
        //            }
        //        } else if (hit.collider.CompareTag("Character")) {
        //            interactableObject = hit.collider.GetComponent<Interactable>();
        //            character = hit.collider.GetComponent<CharacterManager>();
        //            if (character.canTalk) {
        //                if (interactableObject != null) {
        //                    if (!UIManager.instance.itemInteractableGameObject.activeSelf) {
        //                        StartInteraction(interactableObject, hit, false);
        //                    } else if (inputHandler.a_Input) {
        //                        UIManager.instance.itemInteractableGameObject.SetActive(false);
        //                    }
        //                }
        //            }
        //        }
        //    } else {
        //        if (UIManager.instance.interactableUIGameObject != null) {
        //            UIManager.instance.interactableUIGameObject.SetActive(false);
        //        }
        //        if (UIManager.instance.itemInteractableGameObject != null && inputHandler.a_Input) {
        //            UIManager.instance.itemInteractableGameObject.SetActive(false);
        //        }
        //    }
        //}

        //private void StartInteraction(Interactable interactableObject, RaycastHit hit, bool withCharacter) {
        //    string interactableText = interactableObject.interactableText;
        //    interactableUI.interactableText.text = interactableText;
        //    UIManager.instance.interactableUIGameObject.SetActive(true);

        //    if (inputHandler.a_Input) {
        //        Debug.Log("hi");
        //        UIManager.instance.interactableUIGameObject.SetActive(false);
        //        hit.collider.GetComponent<Interactable>().Interact(this);
        //        if (withCharacter) currentPageIndex = 0;
        //    }
        //}

        //private void HandleConversation() {
        //    if (isInConversation) {
        //        float distance = Vector3.Distance(character.transform.position, transform.position);
        //        if (currentPageIndex <= currentDialog.Length - 1) {
        //            PrintDialog();
        //            if (distance >= 3f) FinishConversation();
        //        } else if (turnPageTimer <= 0) {
        //            NPCManager npc = character.GetComponent<NPCManager>();
        //            npc.interactCount++;
        //            FinishConversation();
        //        }
        //    }
        //}

        //public void LeaveBonfire() {
        //    isAtBonfire = false;
        //    playerAnimatorManager.PlayTargetAnimation("Bonfire_End", true);
        //}

        //private void FinishConversation() {
        //    isInConversation = false;
        //    currentPageIndex = 0;
        //    UIManager.instance.dialogUI.SetActive(false);
        //}

        //private void PrintDialog() {
        //    if (!UIManager.instance.dialogUI.activeSelf) UIManager.instance.dialogUI.SetActive(true);
        //    if (turnPageTimer == 0)
        //        turnPageTimer = turnPageTime;
        //    interactableUI.dialogText.text = currentDialog[currentPageIndex].script;
        //    HandleTurnPageTimer();
        //    if (turnPageTimer == 0 && currentPageIndex < currentDialog.Length)
        //        currentPageIndex++;
        //}

        //private void HandleTurnPageTimer() {
        //    if (turnPageTimer <= 0) turnPageTimer = 0;
        //    else {
        //        turnPageTimer -= Time.deltaTime;
        //        if (inputHandler.a_Input) turnPageTimer -= turnPageTime;
        //    }
        //}

        //public void InteractionAtPosition(string animation, Transform playerStandingPosition) {
        //    characterWeaponSlotManager.rightHandSlot.UnloadWeapon();
        //    characterWeaponSlotManager.leftHandSlot.UnloadWeapon();
        //    //playerLocomotion.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //    playerAnimatorManager.PlayTargetAnimation(animation, true);
        //    transform.position = playerStandingPosition.position;
        //}


        //public void PassThroughFogWallInteraction(Transform fogWallEntrance) {
        //    playerLocomotionManager.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //    Vector3 rotationDirection = fogWallEntrance.transform.forward;
        //    Quaternion turnRotation = Quaternion.LookRotation(rotationDirection);
        //    transform.rotation = turnRotation;

        //    playerAnimatorManager.PlayTargetAnimation("PassThroughFog", true);
        //}

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

        // 다른 플레이어의 장비 상태를 로드
        public void LoadOtherPlayerCharacterWhenJoiningOnline(PlayerManager player) {
            player.playerNetworkManager.OnRightWeaponChange(0, player.playerNetworkManager.currentRightWeaponID.Value);
            player.playerNetworkManager.OnLeftWeaponChange(0, player.playerNetworkManager.currentLeftWeaponID.Value);
            player.playerNetworkManager.OnHeadEquipmentChange(0, player.playerNetworkManager.currentHeadEquipmentID.Value);
            player.playerNetworkManager.OnTorsoEquipmentChange(0, player.playerNetworkManager.currentTorsoEquipmentID.Value);
            player.playerNetworkManager.OnGuntletEquipmentChange(0, player.playerNetworkManager.currentGuntletEquipmentID.Value);
            player.playerNetworkManager.OnLegEquipmentChange(0, player.playerNetworkManager.currentLegEquipmentID.Value);
        }
    }
}
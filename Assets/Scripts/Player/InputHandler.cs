using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SoulsLike {
    public class InputHandler : MonoBehaviour {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX, mouseY;

        public bool b_Input;
        public bool x_Input;
        public bool mouseLeftBtn;
        public bool mouseRightBtn;
        public bool heavyAtkInput;
        public bool lt_Input;
        public bool critical_Attack_Input;
        public bool nextMemorySlotInput;
        public bool nextConsumableSlotInput;
        public bool nextRightHandSlotInput;
        public bool nextLeftHandSlotInput;
        public bool a_Input;
        public bool y_Input;
        public bool jump_Input;
        public bool inventory_Input;
        public bool lockOn_Input;
        public bool right_Stick_Right_Input;
        public bool right_Stick_Left_Input;

        public bool twoHandFlag;
        public bool rollFlag;
        //public bool sprintFlag;
        public bool comboFlag;
        //public bool inventoryFlag;

        public float rollInputTimer; // 다크소울 처럼 tap할 경우 구르고, 계속 누르고있을시 달리도록 하기위한 타이머
        public float backstepDelay;

        public Transform criticalAttackRayCastStartPoint;

        // Input Action 인스턴스
        PlayerControls inputActions;
        PlayerManager player;
        CameraHandler cameraHandler;
        BlockingCollider blockingCollider;

        Vector2 movementInput;
        Vector2 cameraInput;

        public void Awake() {
            player = GetComponent<PlayerManager>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            blockingCollider = GetComponentInChildren<BlockingCollider>();
        }

        public void OnEnable() {
            if (inputActions == null) {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

                // InputAction은 Event 형식이므로 굳이 Update에서 입력을 계속해서 감지할 필요 없음
                // 버튼의 입력을 매 프레임마다 감지하게 되면 GarbageCollector에게 부담을 주게 된다.
                inputActions.PlayerActions.MouseLeftBtn.performed += i => mouseLeftBtn = true;
                inputActions.PlayerActions.RBtn.performed += i => heavyAtkInput = true;
                inputActions.PlayerActions.LT.performed += i => lt_Input = true;
                inputActions.PlayerActions.UpArrowBtn.performed += i => nextMemorySlotInput = true;
                inputActions.PlayerActions.DownArrowBtn.performed += i => nextConsumableSlotInput = true;
                inputActions.PlayerActions.RightArrowBtn.performed += i => nextRightHandSlotInput = true;
                inputActions.PlayerActions.LeftArrowBtn.performed += i => nextLeftHandSlotInput = true;
                inputActions.PlayerActions.XBtn.performed += i => x_Input = true;

                // 버튼이 눌리면 이벤트 호출
                inputActions.PlayerActions.Roll.performed += i => b_Input = true;
                inputActions.PlayerActions.InteractionBtn.performed += i => a_Input = true;
                inputActions.PlayerActions.MouseRightBtn.performed += i => mouseRightBtn = true;
                
                // 버튼을 눌렀다 바로 뗄떼 이벤트가 호출되는듯
                inputActions.PlayerActions.Roll.canceled += i => b_Input = false;
                inputActions.PlayerActions.MouseRightBtn.canceled += i => mouseRightBtn = false;

                inputActions.PlayerActions.Jump.performed += i => jump_Input = true;
                inputActions.PlayerActions.InGameMenu.performed += i => inventory_Input = true;
                inputActions.PlayerActions.LockOn.performed += i => lockOn_Input = true;
                inputActions.PlayerActions.LockOnTargetLeft.performed += i => right_Stick_Left_Input = true;
                inputActions.PlayerActions.LockOnTargetRight.performed += i => right_Stick_Right_Input = true;
                inputActions.PlayerActions.Ybutton.performed += i => y_Input = true;
                inputActions.PlayerActions.CriticalAttack.performed += i => critical_Attack_Input = true;
            }

            inputActions.Enable();
        }

        private void OnDisable() {
            inputActions.Disable();
        }

        private void OnApplicationFocus(bool focus) {
            // 창을 최소화 하면 입력값을 받지 않음
            // 다시 창을 열면 입력을 받음
            if (this.enabled) {
                if (focus) {
                    inputActions.Enable();
                } else {
                    inputActions.Disable();
                }
            }
        }

        // 모든 입력 처리 호출
        public void TickInput(float delta) {
            if (player.playerStatsManager.isDead) return;

            HandleMoveInput();
            HandleRollInput(delta);
            HandleCombatInput();
            HandleQuickSlotInput();
            HandleInventoryInput();
            HandleLockOnInput();
            HandleTwoHandInput();
            HandleCriticalAttackInput();
            HandleUseConsumableInput();
        }

        private void HandleMoveInput() {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
            player.playerLocomotionManager.SetMovementValues(vertical, horizontal, moveAmount);
        }

        // 구르기 버튼이 눌리면 회피 Flag의 bool값이 true가 된다.
        private void HandleRollInput(float delta) {

            if (b_Input) { // 회피버튼을 누르고 있는 동안
                //Debug.Log("rollFlag : " + rollFlag);
                rollInputTimer += delta;

                if (player.playerStatsManager.currentStamina <= 0) { // 스테미너가 남아있지 않다면
                    b_Input = false;
                    //sprintFlag = false;
                    player.playerNetworkManager.isSprinting.Value = false;
                }
                if (moveAmount > 0.5f && player.playerStatsManager.currentStamina > 0) {
                    player.playerNetworkManager.isSprinting.Value = true;
                }
            } else {
                player.playerNetworkManager.isSprinting.Value = false;
                if (rollInputTimer > 0 && rollInputTimer < 0.3f) {
                    rollFlag = true;
                }
                rollInputTimer = 0;
            }
        }

        private void HandleCombatInput() {
            if (UIManager.instance.uiStack.Count >= 2) return;
            // RB 버튼은 오른손에 들린 무기로 공격하는 버튼
            if (mouseLeftBtn) {
                player.playerCombatManager.HandleRBAction();
            }
            if (heavyAtkInput) {
                if (player.isInteracting) return;
                if (player.canDoCombo) return;
                player.playerCombatManager.HandleHeavyAttack(player.playerInventoryManager.rightWeapon);
            }

            if (lt_Input) {
                // 양잡상태라면 전투기술 사용
                if (twoHandFlag) {

                } else {
                    // 왼손또한 무기를 들고있다면 왼손 약공
                    // 방패를 들고있다면 방패 전투기술 사용
                    player.playerCombatManager.HandleLTAction();
                }
            }

            if (mouseRightBtn) {
                player.playerCombatManager.HandleLBAction();
            } else {
                player.characterNetworkManager.isBlocking.Value = false;
                if (blockingCollider.blockingCollider.enabled) {
                    blockingCollider.DisableBlockingCollider();
                }
            }

        }

        private void HandleQuickSlotInput() {
            if (nextRightHandSlotInput) {
                player.playerInventoryManager.ChangeRightWeapon();
            } else if (nextLeftHandSlotInput) {
                player.playerInventoryManager.ChangeLeftWeapon();
            } else if (nextMemorySlotInput) {
                player.playerInventoryManager.ChangeSpell();
            } else if (nextConsumableSlotInput) {
                player.playerInventoryManager.ChangeConsumableItem();
            }
        }

        private void HandleInventoryInput() {
            if (inventory_Input) {
                if (UIManager.instance.uiStack.Count <= 1) {
                    UIManager.instance.OpenSelectedWindow(0);
                    UIManager.instance.UpdateInventoryUI();
                } else {
                    UIManager.instance.CloseWindow();
                }
            }
        }

        private void HandleLockOnInput() {
            // 록온 버튼이 눌렸고 아직 록온 상태가 아닌경우
            if (lockOn_Input && !player.playerNetworkManager.isLockedOn.Value) {
                lockOn_Input = !lockOn_Input;
                cameraHandler.HandleLockOn();
                if (cameraHandler.nearestLockOnTarget != null) {
                    // 록온 대상 설정
                    player.currentTarget = cameraHandler.nearestLockOnTarget;
                    player.playerNetworkManager.currentTargetID.Value = player.currentTarget.GetComponent<NetworkObject>().NetworkObjectId;
                    //Debug.Log("플레이어의 현재 타겟 ID : " + player.playerNetworkManager.currentTargetID.Value);
                    player.playerNetworkManager.isLockedOn.Value = !player.playerNetworkManager.isLockedOn.Value;
                }
            }
            // 록온 버튼이 눌렸고 이미 록온 상태인 경우 => 록온을 풀고 싶은 경우
            else if (lockOn_Input && player.playerNetworkManager.isLockedOn.Value) {
                lockOn_Input = !lockOn_Input;
                player.playerNetworkManager.isLockedOn.Value = !player.playerNetworkManager.isLockedOn.Value;
                cameraHandler.ClearLockOnTargets();
            }

            if (player.playerNetworkManager.isLockedOn.Value && right_Stick_Left_Input) {
                right_Stick_Left_Input = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.leftLockTarget != null) {
                    player.currentTarget = cameraHandler.leftLockTarget;
                    player.playerNetworkManager.isLockedOn.Value = !player.playerNetworkManager.isLockedOn.Value;
                }
            }

            if (player.playerNetworkManager.isLockedOn.Value && right_Stick_Right_Input) {
                right_Stick_Right_Input = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.rightLockTarget != null) {
                    player.currentTarget = cameraHandler.rightLockTarget;
                    player.playerNetworkManager.isLockedOn.Value = !player.playerNetworkManager.isLockedOn.Value;
                }
            }

            cameraHandler.SetCameraHeight();
        }

        // 양잡 기능
        // 양잡을 하거나 양잡을 풀때 기본적으로 무기를 다시 로드한다.
        private void HandleTwoHandInput() {
            if (y_Input) {
                y_Input = false;
                twoHandFlag = !twoHandFlag;
                if (twoHandFlag) { // 양잡
                    player.characterNetworkManager.isTwoHandingWeapon.Value = true;
                    player.playerWeaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.rightWeapon, false);
                    player.playerWeaponSlotManager.LoadTwoHandIKTargets(true);
                } else { // 양잡 해제
                    player.characterNetworkManager.isTwoHandingWeapon.Value = false;
                    player.playerWeaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.rightWeapon, false);
                    player.playerWeaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.leftWeapon, true);
                    player.playerWeaponSlotManager.LoadTwoHandIKTargets(false);
                }
            }
        }

        private void HandleCriticalAttackInput() {
            if (critical_Attack_Input) {
                critical_Attack_Input = false;
                player.playerCombatManager.AttemptBackStabOrRiposte();
            }
        }

        private void HandleUseConsumableInput() {
            if (x_Input) {
                if (player.isInteracting) return;
                x_Input = false;
                // 현재 소비 아이템을 사용한다.
                player.playerInventoryManager.currentConsumable.AttemptToConsumeItem(player);
            }
        }
    }
}
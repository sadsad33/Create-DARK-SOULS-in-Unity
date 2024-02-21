using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class InputHandler : MonoBehaviour {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX, mouseY;

        public bool b_Input;
        public bool x_Input;
        public bool rb_Input;
        public bool lb_Input;
        public bool rt_Input;
        public bool lt_Input;
        public bool critical_Attack_Input;
        public bool d_Pad_Up;
        public bool d_Pad_Down;
        public bool d_Pad_Right;
        public bool d_Pad_Left;
        public bool a_Input;
        public bool y_Input;
        public bool jump_Input;
        public bool inventory_Input;
        public bool lockOn_Input;
        public bool right_Stick_Right_Input;
        public bool right_Stick_Left_Input;

        public bool twoHandFlag;
        public bool rollFlag;
        public bool sprintFlag;
        public bool comboFlag;
        public bool lockOnFlag;
        public bool inventoryFlag;

        public float rollInputTimer; // 다크소울 처럼 tap할 경우 구르고, 계속 누르고있을시 달리도록 하기위한 타이머
        public float backstepDelay;

        public Transform criticalAttackRayCastStartPoint;

        // Input Action 인스턴스
        PlayerControls inputActions;

        PlayerCombatManager playerCombatManager;
        PlayerInventoryManager playerInventoryManager;
        PlayerManager playerManager;
        UIManager uiManager;
        CameraHandler cameraHandler;
        PlayerWeaponSlotManager weaponSlotManager;
        PlayerAnimatorManager playerAnimatiorManager;
        PlayerStatsManager playerStatsManager;
        PlayerEffectsManager playerEffectsManager;

        BlockingCollider blockingCollider;

        Vector2 movementInput;
        Vector2 cameraInput;

        public void Awake() {
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerManager = GetComponent<PlayerManager>();
            uiManager = FindObjectOfType<UIManager>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            weaponSlotManager = GetComponent<PlayerWeaponSlotManager>();
            blockingCollider = GetComponentInChildren<BlockingCollider>();
            playerAnimatiorManager = GetComponent<PlayerAnimatorManager>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();
        }

        public void OnEnable() {
            if (inputActions == null) {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

                // InputAction은 Event 형식이므로 굳이 Update에서 입력을 계속해서 감지할 필요 없음
                // 버튼의 입력을 매 프레임마다 감지하게 되면 GarbageCollector에게 부담을 주게 된다.
                inputActions.PlayerActions.RB.performed += i => rb_Input = true;
                inputActions.PlayerActions.RT.performed += i => rt_Input = true;
                inputActions.PlayerActions.LB.performed += i => lb_Input = true;
                inputActions.PlayerActions.LB.canceled += i => lb_Input = false;
                inputActions.PlayerActions.LT.performed += i => lt_Input = true;
                inputActions.PlayerActions.DpadRight.performed += i => d_Pad_Right = true;
                inputActions.PlayerActions.DpadLeft.performed += i => d_Pad_Left = true;
                inputActions.PlayerActions.Abutton.performed += i => a_Input = true;
                inputActions.PlayerActions.Xbutton.performed += i => x_Input = true;

                // 버튼이 눌리면 이벤트 호출
                inputActions.PlayerActions.Roll.performed += i => b_Input = true;
                
                // 버튼을 눌렀다 바로 뗄떼 이벤트가 호출되는듯
                inputActions.PlayerActions.Roll.canceled += i => b_Input = false;
                inputActions.PlayerActions.Jump.performed += i => jump_Input = true;
                inputActions.PlayerActions.Inventory.performed += i => inventory_Input = true;
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

        // 모든 입력 처리 호출
        public void TickInput(float delta) {
            if (playerStatsManager.isDead) return;

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
        }

        // 구르기 버튼이 눌리면 회피 Flag의 bool값이 true가 된다.
        private void HandleRollInput(float delta) {

            if (b_Input) { // 회피버튼을 누르고 있는 동안
                //Debug.Log("rollFlag : " + rollFlag);
                rollInputTimer += delta;

                if (playerStatsManager.currentStamina <= 0) { // 스테미너가 남아있지 않다면
                    b_Input = false;
                    sprintFlag = false;
                }
                if (moveAmount > 0.5f && playerStatsManager.currentStamina > 0) {
                    sprintFlag = true;
                }
            } else {
                sprintFlag = false;
                if (rollInputTimer > 0 && rollInputTimer < 0.3f) {
                    rollFlag = true;
                }
                rollInputTimer = 0;
            }
        }

        private void HandleCombatInput() {

            // RB 버튼은 오른손에 들린 무기로 공격하는 버튼
            if (rb_Input) {
                playerCombatManager.HandleRBAction();
            }
            if (rt_Input) {
                if (playerManager.isInteracting) return;
                if (playerManager.canDoCombo) return;
                playerCombatManager.HandleHeavyAttack(playerInventoryManager.rightWeapon);
            }

            if (lt_Input) {
                // 양잡상태라면 전투기술 사용
                if (twoHandFlag) {

                } else {
                    // 왼손또한 무기를 들고있다면 왼손 약공
                    // 방패를 들고있다면 방패 전투기술 사용
                    playerCombatManager.HandleLTAction();
                }
            }

            if (lb_Input) {
                playerCombatManager.HandleLBAction();
            } else {
                playerManager.isBlocking = false;
                if (blockingCollider.blockingCollider.enabled) {
                    blockingCollider.DisableBlockingCollider();
                }
            }

        }

        private void HandleQuickSlotInput() {

            if (d_Pad_Right) {
                playerInventoryManager.ChangeRightWeapon();
            } else if (d_Pad_Left) {
                playerInventoryManager.ChangeLeftWeapon();
            }
        }

        private void HandleInventoryInput() {
            if (inventory_Input) {
                inventoryFlag = !inventoryFlag;
                if (inventoryFlag) {
                    uiManager.OpenSelectWindow();
                    uiManager.UpdateUI(); // 인벤토리 업데이트
                    uiManager.hudWindow.SetActive(false); // 인벤토리가 열리면 HUD를 끈다.
                } else {
                    uiManager.CloseSelectWindow();
                    uiManager.CloseAllInventoryWindows();
                    uiManager.hudWindow.SetActive(true); // 인벤토리가 닫히면 HUD를 킨다.
                }
            }
        }

        private void HandleLockOnInput() {
            // 록온 버튼이 눌렸고 아직 록온 상태가 아닌경우
            if (lockOn_Input && !lockOnFlag) {

                lockOn_Input = !lockOn_Input;
                cameraHandler.HandleLockOn();
                if (cameraHandler.nearestLockOnTarget != null) {
                    // 록온 대상 설정
                    cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                    lockOnFlag = !lockOnFlag;
                }
            }
            // 록온 버튼이 눌렸고 이미 록온 상태인 경우 => 록온을 풀고 싶은 경우
            else if (lockOn_Input && lockOnFlag) {
                lockOn_Input = !lockOn_Input;
                lockOnFlag = !lockOnFlag;
                cameraHandler.ClearLockOnTargets();
            }

            if (lockOnFlag && right_Stick_Left_Input) {
                right_Stick_Left_Input = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.leftLockTarget != null) {
                    cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
                }
            }

            if (lockOnFlag && right_Stick_Right_Input) {
                right_Stick_Right_Input = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.rightLockTarget != null) {
                    cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
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
                    playerManager.isTwoHandingWeapon = true;
                    weaponSlotManager.LoadWeaponOnSlot(playerInventoryManager.rightWeapon, false);
                    weaponSlotManager.LoadTwoHandIKTargets(true);
                } else { // 양잡 해제
                    playerManager.isTwoHandingWeapon = false;
                    weaponSlotManager.LoadWeaponOnSlot(playerInventoryManager.rightWeapon, false);
                    weaponSlotManager.LoadWeaponOnSlot(playerInventoryManager.leftWeapon, true);
                    weaponSlotManager.LoadTwoHandIKTargets(false);
                }
            }
        }

        private void HandleCriticalAttackInput() {
            if (critical_Attack_Input) {
                critical_Attack_Input = false;
                playerCombatManager.AttemptBackStabOrRiposte();
            }
        }

        private void HandleUseConsumableInput() {
            if (x_Input) {
                if (playerManager.isInteracting) return;
                x_Input = false;
                // 현재 소비 아이템을 사용한다.
                playerInventoryManager.currentConsumable.AttemptToConsumeItem(playerAnimatiorManager, weaponSlotManager, playerEffectsManager);
            }
        }
    }
}
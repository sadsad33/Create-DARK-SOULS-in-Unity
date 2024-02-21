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

        public float rollInputTimer; // ��ũ�ҿ� ó�� tap�� ��� ������, ��� ������������ �޸����� �ϱ����� Ÿ�̸�
        public float backstepDelay;

        public Transform criticalAttackRayCastStartPoint;

        // Input Action �ν��Ͻ�
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

                // InputAction�� Event �����̹Ƿ� ���� Update���� �Է��� ����ؼ� ������ �ʿ� ����
                // ��ư�� �Է��� �� �����Ӹ��� �����ϰ� �Ǹ� GarbageCollector���� �δ��� �ְ� �ȴ�.
                inputActions.PlayerActions.RB.performed += i => rb_Input = true;
                inputActions.PlayerActions.RT.performed += i => rt_Input = true;
                inputActions.PlayerActions.LB.performed += i => lb_Input = true;
                inputActions.PlayerActions.LB.canceled += i => lb_Input = false;
                inputActions.PlayerActions.LT.performed += i => lt_Input = true;
                inputActions.PlayerActions.DpadRight.performed += i => d_Pad_Right = true;
                inputActions.PlayerActions.DpadLeft.performed += i => d_Pad_Left = true;
                inputActions.PlayerActions.Abutton.performed += i => a_Input = true;
                inputActions.PlayerActions.Xbutton.performed += i => x_Input = true;

                // ��ư�� ������ �̺�Ʈ ȣ��
                inputActions.PlayerActions.Roll.performed += i => b_Input = true;
                
                // ��ư�� ������ �ٷ� ���� �̺�Ʈ�� ȣ��Ǵµ�
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

        // ��� �Է� ó�� ȣ��
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

        // ������ ��ư�� ������ ȸ�� Flag�� bool���� true�� �ȴ�.
        private void HandleRollInput(float delta) {

            if (b_Input) { // ȸ�ǹ�ư�� ������ �ִ� ����
                //Debug.Log("rollFlag : " + rollFlag);
                rollInputTimer += delta;

                if (playerStatsManager.currentStamina <= 0) { // ���׹̳ʰ� �������� �ʴٸ�
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

            // RB ��ư�� �����տ� �鸰 ����� �����ϴ� ��ư
            if (rb_Input) {
                playerCombatManager.HandleRBAction();
            }
            if (rt_Input) {
                if (playerManager.isInteracting) return;
                if (playerManager.canDoCombo) return;
                playerCombatManager.HandleHeavyAttack(playerInventoryManager.rightWeapon);
            }

            if (lt_Input) {
                // ������¶�� ������� ���
                if (twoHandFlag) {

                } else {
                    // �޼ն��� ���⸦ ����ִٸ� �޼� ���
                    // ���и� ����ִٸ� ���� ������� ���
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
                    uiManager.UpdateUI(); // �κ��丮 ������Ʈ
                    uiManager.hudWindow.SetActive(false); // �κ��丮�� ������ HUD�� ����.
                } else {
                    uiManager.CloseSelectWindow();
                    uiManager.CloseAllInventoryWindows();
                    uiManager.hudWindow.SetActive(true); // �κ��丮�� ������ HUD�� Ų��.
                }
            }
        }

        private void HandleLockOnInput() {
            // �Ͽ� ��ư�� ���Ȱ� ���� �Ͽ� ���°� �ƴѰ��
            if (lockOn_Input && !lockOnFlag) {

                lockOn_Input = !lockOn_Input;
                cameraHandler.HandleLockOn();
                if (cameraHandler.nearestLockOnTarget != null) {
                    // �Ͽ� ��� ����
                    cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                    lockOnFlag = !lockOnFlag;
                }
            }
            // �Ͽ� ��ư�� ���Ȱ� �̹� �Ͽ� ������ ��� => �Ͽ��� Ǯ�� ���� ���
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

        // ���� ���
        // ������ �ϰų� ������ Ǯ�� �⺻������ ���⸦ �ٽ� �ε��Ѵ�.
        private void HandleTwoHandInput() {
            if (y_Input) {
                y_Input = false;
                twoHandFlag = !twoHandFlag;
                if (twoHandFlag) { // ����
                    playerManager.isTwoHandingWeapon = true;
                    weaponSlotManager.LoadWeaponOnSlot(playerInventoryManager.rightWeapon, false);
                    weaponSlotManager.LoadTwoHandIKTargets(true);
                } else { // ���� ����
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
                // ���� �Һ� �������� ����Ѵ�.
                playerInventoryManager.currentConsumable.AttemptToConsumeItem(playerAnimatiorManager, weaponSlotManager, playerEffectsManager);
            }
        }
    }
}
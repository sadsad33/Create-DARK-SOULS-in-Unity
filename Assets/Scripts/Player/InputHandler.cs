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
        //public bool inventoryFlag;

        public float rollInputTimer; // ��ũ�ҿ� ó�� tap�� ��� ������, ��� ������������ �޸����� �ϱ����� Ÿ�̸�
        public float backstepDelay;

        public Transform criticalAttackRayCastStartPoint;

        // Input Action �ν��Ͻ�
        PlayerControls inputActions;

        PlayerCombatManager playerCombatManager;
        PlayerManager playerManager;
        UIManager uiManager;
        CameraHandler cameraHandler;

        BlockingCollider blockingCollider;

        Vector2 movementInput;
        Vector2 cameraInput;

        public void Awake() {
            playerCombatManager = GetComponent<PlayerCombatManager>();
            playerManager = GetComponent<PlayerManager>();
            uiManager = FindObjectOfType<UIManager>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            blockingCollider = GetComponentInChildren<BlockingCollider>();
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
                inputActions.PlayerActions.LT.performed += i => lt_Input = true;
                inputActions.PlayerActions.DpadRight.performed += i => d_Pad_Right = true;
                inputActions.PlayerActions.DpadLeft.performed += i => d_Pad_Left = true;
                inputActions.PlayerActions.DpadUp.performed += i => d_Pad_Up = true;
                inputActions.PlayerActions.DpadDown.performed += i => d_Pad_Down = true;
                inputActions.PlayerActions.Xbutton.performed += i => x_Input = true;

                // ��ư�� ������ �̺�Ʈ ȣ��
                inputActions.PlayerActions.Roll.performed += i => b_Input = true;
                inputActions.PlayerActions.Abutton.performed += i => a_Input = true;
                inputActions.PlayerActions.LB.performed += i => lb_Input = true;
                
                // ��ư�� ������ �ٷ� ���� �̺�Ʈ�� ȣ��Ǵµ�
                inputActions.PlayerActions.Roll.canceled += i => b_Input = false;
                inputActions.PlayerActions.LB.canceled += i => lb_Input = false;

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

        private void OnApplicationFocus(bool focus) {
            // â�� �ּ�ȭ �ϸ� �Է°��� ���� ����
            // �ٽ� â�� ���� �Է��� ����
            if (this.enabled) {
                if (focus) {
                    inputActions.Enable();
                } else {
                    inputActions.Disable();
                }
            }
        }
        // ��� �Է� ó�� ȣ��
        public void TickInput(float delta) {
            if (playerManager.playerStatsManager.isDead) return;

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
            playerManager.playerLocomotion.SetMovementValues(vertical, horizontal, moveAmount);
        }

        // ������ ��ư�� ������ ȸ�� Flag�� bool���� true�� �ȴ�.
        private void HandleRollInput(float delta) {

            if (b_Input) { // ȸ�ǹ�ư�� ������ �ִ� ����
                //Debug.Log("rollFlag : " + rollFlag);
                rollInputTimer += delta;

                if (playerManager.playerStatsManager.currentStamina <= 0) { // ���׹̳ʰ� �������� �ʴٸ�
                    b_Input = false;
                    sprintFlag = false;
                }
                if (moveAmount > 0.5f && playerManager.playerStatsManager.currentStamina > 0) {
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
            if (uiManager.uiStack.Count >= 2) return;
            // RB ��ư�� �����տ� �鸰 ����� �����ϴ� ��ư
            if (rb_Input) {
                playerCombatManager.HandleRBAction();
            }
            if (rt_Input) {
                if (playerManager.isInteracting) return;
                if (playerManager.canDoCombo) return;
                playerCombatManager.HandleHeavyAttack(playerManager.playerInventoryManager.rightWeapon);
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
                playerManager.playerInventoryManager.ChangeRightWeapon();
            } else if (d_Pad_Left) {
                playerManager.playerInventoryManager.ChangeLeftWeapon();
            } else if (d_Pad_Up) {
                playerManager.playerInventoryManager.ChangeSpell();
            } else if (d_Pad_Down) {
                playerManager.playerInventoryManager.ChangeConsumableItem();
            }
        }

        private void HandleInventoryInput() {
            if (inventory_Input) {
                if (uiManager.uiStack.Count <= 1) {
                    uiManager.OpenSelectedWindow(0);
                    uiManager.UpdateUI();
                } else {
                    uiManager.CloseWindow();
                }
            }
        }

        private void HandleLockOnInput() {
            // �Ͽ� ��ư�� ���Ȱ� ���� �Ͽ� ���°� �ƴѰ��
            if (lockOn_Input && !playerManager.playerNetworkManager.isLockedOn.Value) {

                lockOn_Input = !lockOn_Input;
                cameraHandler.HandleLockOn();
                if (cameraHandler.nearestLockOnTarget != null) {
                    // �Ͽ� ��� ����
                    cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                    playerManager.playerNetworkManager.isLockedOn.Value = !playerManager.playerNetworkManager.isLockedOn.Value;
                }
            }
            // �Ͽ� ��ư�� ���Ȱ� �̹� �Ͽ� ������ ��� => �Ͽ��� Ǯ�� ���� ���
            else if (lockOn_Input && playerManager.playerNetworkManager.isLockedOn.Value) {
                lockOn_Input = !lockOn_Input;
                playerManager.playerNetworkManager.isLockedOn.Value = !playerManager.playerNetworkManager.isLockedOn.Value;
                cameraHandler.ClearLockOnTargets();
            }

            if (playerManager.playerNetworkManager.isLockedOn.Value && right_Stick_Left_Input) {
                right_Stick_Left_Input = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.leftLockTarget != null) {
                    cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
                }
            }

            if (playerManager.playerNetworkManager.isLockedOn.Value && right_Stick_Right_Input) {
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
                    playerManager.playerWeaponSlotManager.LoadWeaponOnSlot(playerManager.playerInventoryManager.rightWeapon, false);
                    playerManager.playerWeaponSlotManager.LoadTwoHandIKTargets(true);
                } else { // ���� ����
                    playerManager.isTwoHandingWeapon = false;
                    playerManager.playerWeaponSlotManager.LoadWeaponOnSlot(playerManager.playerInventoryManager.rightWeapon, false);
                    playerManager.playerWeaponSlotManager.LoadWeaponOnSlot(playerManager.playerInventoryManager.leftWeapon, true);
                    playerManager.playerWeaponSlotManager.LoadTwoHandIKTargets(false);
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
                playerManager.playerInventoryManager.currentConsumable.AttemptToConsumeItem(playerManager.playerAnimatorManager, playerManager.playerWeaponSlotManager, playerManager.playerEffectsManager);
            }
        }
    }
}
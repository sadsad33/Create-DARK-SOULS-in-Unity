using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class InputHandler : MonoBehaviour {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX, mouseY;

        public bool b_Input;
        public bool rb_Input;
        public bool rt_Input;
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
        // Input Action �ν��Ͻ�
        PlayerControls inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        UIManager uiManager;
        CameraHandler cameraHandler;
        WeaponSlotManager weaponSlotManager;
        AnimatorHandler animatorHandler;
        Vector2 movementInput;
        Vector2 cameraInput;

        public void Awake() {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            uiManager = FindObjectOfType<UIManager>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
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
                inputActions.PlayerActions.DpadRight.performed += i => d_Pad_Right = true;
                inputActions.PlayerActions.DpadLeft.performed += i => d_Pad_Left = true;
                inputActions.PlayerActions.Abutton.performed += i => a_Input = true;
                inputActions.PlayerActions.Jump.performed += i => jump_Input = true;
                inputActions.PlayerActions.Inventory.performed += i => inventory_Input = true;
                inputActions.PlayerActions.LockOn.performed += i => lockOn_Input = true;
                inputActions.PlayerActions.LockOnTargetLeft.performed += i => right_Stick_Left_Input = true;
                inputActions.PlayerActions.LockOnTargetRight.performed += i => right_Stick_Right_Input = true;
                inputActions.PlayerActions.Ybutton.performed += i => y_Input = true;
            }
            inputActions.Enable();
        }

        private void OnDisable() {
            inputActions.Disable();
        }

        // ��� �Է� ó�� ȣ��
        public void TickInput(float delta) {
            HandleMoveInput(delta);
            HandleRollInput(delta);
            HandleAttackInput(delta);
            HandleQuickSlotInput(delta);
            HandleInventoryInput(delta);
            HandleLockOnInput(delta);
            HandleTwoHandInput(delta);
        }

        private void HandleMoveInput(float delta) {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        // ������ ��ư�� ������ ȸ�� Flag�� bool���� true�� �ȴ�.
        private void HandleRollInput(float delta) {
            b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            sprintFlag = b_Input;

            if (b_Input) { // ȸ�ǹ�ư�� ������ �ִ� ����
                //Debug.Log("rollFlag : " + rollFlag);
                rollInputTimer += delta;
            } else {
                if (rollInputTimer > 0 && rollInputTimer < 0.3f) {
                    sprintFlag = false;
                    rollFlag = true;
                }
                rollInputTimer = 0;
            }
        }

        private void HandleAttackInput(float delta) {

            // RB ��ư�� �����տ� �鸰 ����� �����ϴ� ��ư
            if (rb_Input) {
                if (playerManager.canDoCombo) {
                    comboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                    comboFlag = false;
                } else {
                    if (playerManager.isInteracting) return;
                    if (playerManager.canDoCombo) return;
                    animatorHandler.anim.SetBool("isUsingRightHand", true);
                    if (playerInventory.currentRightWeaponIndex != -1) {
                        playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
                    } else if (playerInventory.currentRightWeaponIndex == -1) {
                        playerAttacker.HandleUnarmedAttack(playerInventory.rightWeapon);
                    }
                }
            }
            if (rt_Input) {
                if (playerManager.isInteracting) return;
                if (playerManager.canDoCombo) return;
                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
            }

        }

        private void HandleQuickSlotInput(float delta) {

            if (d_Pad_Right) {
                playerInventory.ChangeRightWeapon();
            } else if (d_Pad_Left) {
                playerInventory.ChangeLeftWeapon();
            }
        }

        private void HandleInventoryInput(float delta) {
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

        private void HandleLockOnInput(float delta) {
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
        private void HandleTwoHandInput(float delta) {
            if (y_Input) {
                y_Input = false;
                twoHandFlag = !twoHandFlag;
                if (twoHandFlag) { // ����
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
                } else { // ���� ����
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);
                }
            }
        }
    }
}
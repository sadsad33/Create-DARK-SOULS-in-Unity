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
        public bool jump_Input;
        public bool inventory_Input;

        public bool rollFlag;
        public bool sprintFlag;
        public bool comboFlag;
        public bool inventoryFlag;

        public float rollInputTimer; // ��ũ�ҿ� ó�� tap�� ��� ������, ��� ������������ �޸����� �ϱ����� Ÿ�̸�
        public float backstepDelay;
        // Input Action �ν��Ͻ�
        PlayerControls inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        UIManager uiManager;
        Vector2 movementInput;
        Vector2 cameraInput;

        public void Awake() {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            uiManager = FindObjectOfType<UIManager>();
        }
        public void OnEnable() {
            if (inputActions == null) {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            }
            inputActions.Enable();
        }

        private void OnDisable() {
            inputActions.Disable();
        }

        // ��� �Է� ����
        public void TickInput(float delta) {
            MoveInput(delta);
            HandleRollInput(delta);
            HandleAttackInput(delta);
            HandleQuickSlotInput(delta);
            HandleInteractingButtonInput(delta);
            HandleJumpInput(delta);
            HandleInventoryInput(delta);
        }

        private void MoveInput(float delta) {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        // ������ ��ư�� ������ ȸ�� Flag�� bool���� true�� �ȴ�.
        private void HandleRollInput(float delta) {
            b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            if (b_Input) { // ȸ�ǹ�ư�� ������ �ִ� ����
                //Debug.Log("rollFlag : " + rollFlag);
                rollInputTimer += delta;
                sprintFlag = true;
            } else {
                if (rollInputTimer > 0 && rollInputTimer < 0.3f) {
                    sprintFlag = false;
                    rollFlag = true;
                }
                rollInputTimer = 0;
            }
        }

        private void HandleAttackInput(float delta) {
            inputActions.PlayerActions.RB.performed += i => rb_Input = true;
            inputActions.PlayerActions.RT.performed += i => rt_Input = true;

            // RB ��ư�� �����տ� �鸰 ����� �����ϴ� ��ư
            if (rb_Input) {
                if (playerManager.canDoCombo) {
                    comboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                    comboFlag = false;
                } else {
                    if (playerManager.isInteracting) return;
                    if (playerManager.canDoCombo) return;
                    playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
                }
            }
            if (rt_Input) {
                if (playerManager.isInteracting) return;
                if (playerManager.canDoCombo) return;
                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
            }

        }

        private void HandleQuickSlotInput(float delta) {
            inputActions.PlayerActions.DpadRight.performed += i => d_Pad_Right = true;
            inputActions.PlayerActions.DpadLeft.performed += i => d_Pad_Left = true;
            if (d_Pad_Right) {
                playerInventory.ChangeRightWeapon();
            } else if (d_Pad_Left) {
                playerInventory.ChangeLeftWeapon();
            }
        }

        private void HandleInteractingButtonInput(float delta) {
            inputActions.PlayerActions.Abutton.performed += i => a_Input = true;
        }

        private void HandleJumpInput(float delta) {
            inputActions.PlayerActions.Jump.performed += i => jump_Input = true;
        }

        private void HandleInventoryInput(float delta) {
            inputActions.PlayerActions.Inventory.performed += i => inventory_Input = true;
            if (inventory_Input) {
                inventoryFlag = !inventoryFlag;
                if (inventoryFlag) {
                    uiManager.OpenSelectWindow();
                } else {
                    uiManager.CloseSelectWindow();
                }
            }
        }
    }
}
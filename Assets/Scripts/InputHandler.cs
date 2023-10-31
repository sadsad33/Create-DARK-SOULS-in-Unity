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

        public bool rollFlag;
        public bool sprintFlag;
        public float rollInputTimer; // ��ũ�ҿ� ó�� tap�� ��� ������, ��� ������������ �޸����� �ϱ����� Ÿ�̸�
        public float backstepDelay;
        // Input Action �ν��Ͻ�
        PlayerControls inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;

        Vector2 movementInput;
        Vector2 cameraInput;

        public void Awake() {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
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

        public void TickInput(float delta) {
            MoveInput(delta);
            HandleRollInput(delta);
            HandleAttackInput(delta);
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
                playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
            }
            if (rt_Input) {
                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
            }
            
        }
    }
}
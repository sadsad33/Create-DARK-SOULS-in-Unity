using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Update Method�� �ַ� ó��
// �÷��̾��� ���� Flag�� ó���Ѵ�.
// �÷��̾��� ���� ��ɵ��� �����Ѵ�.
namespace sg {
    public class PlayerManager : MonoBehaviour {
        InputHandler inputHandler;
        Animator anim;
        public bool isInteracting;

        [Header("Player Flags")]
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;

        PlayerLocomotion playerLocomotion;
        CameraHandler cameraHandler;
        private void Awake() {
            cameraHandler = CameraHandler.singleton;
        }

        void Start() {
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
        }

        void Update() {
            float delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            
            inputHandler.TickInput(delta);
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleRollingAndSprinting(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
            
            // �̵�Ű�� �齺��Ű�� ª�� �������� ������ �齺�� ���� sprint �ִϸ��̼��� ����Ǵ� ��찡 �ִ�.
            // �̸� �ذ��ϱ� ���� delay �߰�
            if (inputHandler.moveAmount == 0) {
                inputHandler.backstepDelay += delta;
            } else {
                inputHandler.backstepDelay = 0;
            }
        }

        private void FixedUpdate() {
            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null) {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
        }

        private void LateUpdate() {
            inputHandler.rollFlag = false; // ȸ�� �÷��� ����
            inputHandler.sprintFlag = false; // ������Ʈ �÷��� ����
            inputHandler.rb_Input = false; // ����� �÷��� ����
            inputHandler.rt_Input = false; // ������ �÷��� ����

            if (isInAir) { // �÷��̾ ����� �ִٸ�
                playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �÷��̾ ���� Update �Լ��� ó��
// �÷��̾��� ���� Flag�� ó���Ѵ�.
// �÷��̾��� ���� ��ɵ��� �����Ѵ�.
namespace sg {
    public class PlayerManager : CharacterManager {
        InputHandler inputHandler;
        Animator anim;
        public bool isInteracting;
        public GameObject interactableUIGameObject; // ��ȣ�ۿ� �޼��� (�� ����, ���� ������ ��)
        public GameObject itemInteractableGameObject; // ������ ȹ�� �޼���

        [Header("Player Flags")]
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;
        public bool canDoCombo;

        PlayerLocomotion playerLocomotion;
        CameraHandler cameraHandler;
        InteractableUI interactableUI; // ��ȣ�ۿ붧 ��Ÿ���� �޼��� â

        private void Awake() {
            cameraHandler = FindObjectOfType<CameraHandler>();
        }

        void Start() {
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            interactableUI = FindObjectOfType<InteractableUI>();
        }

        void Update() {
            float delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            canDoCombo = anim.GetBool("canDoCombo");
            anim.SetBool("isInAir", isInAir);
            
            inputHandler.TickInput(delta);
            // Rigidbody�� �̵��Ǵ� �������� �ƴ϶�� �Ϲ����� Update�Լ����� ȣ���ص� ������.
            playerLocomotion.HandleRollingAndSprinting(delta);
            playerLocomotion.HandleJumping();

            CheckForInteractableObject();
            
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
            
            // Rigidbody�� ���� ó���Ǵ� �������� FixedUpdate���� ó���Ǵ°��� ����
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
        }

        private void LateUpdate() {
            // 1�����Ӵ� �ѹ��� ȣ�⸸ �̷������� �Ѵ�.
            inputHandler.rollFlag = false;
            inputHandler.rb_Input = false;
            inputHandler.rt_Input = false;
            inputHandler.d_Pad_Up = false;
            inputHandler.d_Pad_Down = false;
            inputHandler.d_Pad_Left = false;
            inputHandler.d_Pad_Right = false;
            inputHandler.a_Input = false;
            inputHandler.jump_Input = false;
            inputHandler.inventory_Input = false;

            float delta = Time.deltaTime;
            if (cameraHandler != null) {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }

            if (isInAir) { // �÷��̾ ����� �ִٸ�
                playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
            }
        }

        public void CheckForInteractableObject() {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayer)) {
                if (hit.collider.tag == "Interactable") {
                    Interactable interactableObject = hit.collider.GetComponent<Interactable>();
                    if (interactableObject != null) {
                        string interactableText = interactableObject.interactableText;
                        interactableUI.interactableText.text = interactableText;
                        interactableUIGameObject.SetActive(true);
                        
                        if (inputHandler.a_Input) {
                            hit.collider.GetComponent<Interactable>().Interact(this);
                        }
                    }
                }
            } else { // �ֺ��� ��ȣ�ۿ밡���� ������Ʈ�� �������� �޼���â�� ������ �ʵ���
                if (interactableUIGameObject != null) {
                    interactableUIGameObject.SetActive(false);
                }

                // �������� �����ϰ� ���� ����Ű�� �ѹ� �� ������ �޽���â�� ������.
                if (itemInteractableGameObject != null && inputHandler.a_Input) {
                    itemInteractableGameObject.SetActive(false);
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어를 위한 Update 함수를 처리
// 플레이어의 각종 Flag를 처리한다.
// 플레이어의 각종 기능들을 연결한다.
namespace sg {
    public class PlayerManager : CharacterManager {
        InputHandler inputHandler;
        Animator anim;
        public bool isInteracting;
        public GameObject interactableUIGameObject; // 상호작용 메세지 (문 열기, 레버 내리기 등)
        public GameObject itemInteractableGameObject; // 아이템 획득 메세지

        [Header("Player Flags")]
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;
        public bool canDoCombo;
        public bool isUsingRightHand, isUsingLeftHand;

        PlayerLocomotion playerLocomotion;
        CameraHandler cameraHandler;
        InteractableUI interactableUI; // 상호작용때 나타나는 메세지 창

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
            isUsingRightHand = anim.GetBool("isUsingRightHand");
            isUsingLeftHand = anim.GetBool("isUsingLeftHand");
            inputHandler.TickInput(delta);
            // Rigidbody가 이동되는 움직임이 아니라면 일반적인 Update함수에서 호출해도 괜찮다.
            playerLocomotion.HandleRollingAndSprinting(delta);
            playerLocomotion.HandleJumping();

            CheckForInteractableObject();
            
            // 이동키와 백스텝키가 짧은 간격으로 눌리면 백스텝 이후 sprint 애니메이션이 실행되는 경우가 있다.
            // 이를 해결하기 위해 delay 추가
            if (inputHandler.moveAmount == 0) {
                inputHandler.backstepDelay += delta;
            } else {
                inputHandler.backstepDelay = 0;
            }
        }

        private void FixedUpdate() {
            float delta = Time.fixedDeltaTime;
            
            // Rigidbody를 통해 처리되는 움직임은 FixedUpdate에서 처리되는것이 좋음
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
        }

        private void LateUpdate() {
            // 1프레임당 한번의 호출만 이뤄지도록 한다.
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

            if (isInAir) { // 플레이어가 허공에 있다면
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
            } else { // 주변에 상호작용가능한 오브젝트가 없음에도 메세지창이 떠있지 않도록
                if (interactableUIGameObject != null) {
                    interactableUIGameObject.SetActive(false);
                }

                // 아이템을 수집하고 나서 수집키를 한번 더 누르면 메시지창이 닫힌다.
                if (itemInteractableGameObject != null && inputHandler.a_Input) {
                    itemInteractableGameObject.SetActive(false);
                }
            }
        }
    }
}
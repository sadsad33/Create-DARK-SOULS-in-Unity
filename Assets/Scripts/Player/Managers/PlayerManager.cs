using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어를 위한 Update 함수를 처리
// 플레이어의 각종 Flag를 처리한다.
// 플레이어의 각종 기능들을 연결한다.

namespace SoulsLike {
    public class PlayerManager : CharacterManager {
        InputHandler inputHandler;
        Animator anim;
        public GameObject interactableUIGameObject; // 상호작용 메세지 (문 열기, 레버 내리기 등) : InteractionPopUp
        public GameObject itemInteractableGameObject; // 아이템 획득 메세지 : ItemPopup
        public GameObject dialogUI; // NPC의 대사를 출력할 창
        public Transform leftFoot, rightFoot;
        public LadderEndPositionDetection ladderEndPositionDetector;

        // 다크소울 시리즈에서는 대화 도중 행동이 가능하므로 isInteracting 과 분리
        public bool isInConversation;
        public bool isJumping = false;
        public bool rightFootUp;
        public bool isLadderTop;
        private float turnPageTimer;
        private readonly float turnPageTime = 10f;
        private int currentPageIndex;

        PlayerAnimatorManager playerAnimatorManager;
        PlayerStatsManager playerStatsManager;
        PlayerEffectsManager playerEffectsManager;
        PlayerLocomotionManager playerLocomotion;
        public NPCScript[] currentDialog;
        CameraHandler cameraHandler;

        [SerializeField]
        InteractableUI interactableUI; // 상호작용때 나타나는 메세지 창
        LayerMask interactableLayer;

        protected override void Awake() {
            base.Awake();
            ladderEndPositionDetector = GetComponentInChildren<LadderEndPositionDetection>();
            if (ladderEndPositionDetector != null) ladderEndPositionDetector.transform.gameObject.SetActive(false);
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            backStabCollider = GetComponentInChildren<CriticalDamageCollider>();
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponent<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotionManager>();
            interactableUI = FindObjectOfType<InteractableUI>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();
            interactableLayer = 1 << 0 | 1 << 9;
        }

        void Update() {
            if (isClimbing) {
                if (leftFoot.position.y > rightFoot.position.y) {
                    rightFootUp = false;
                } else if (leftFoot.position.y < rightFoot.position.y) {
                    rightFootUp = true;
                }
            }

            float delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            isFiringSpell = anim.GetBool("isFiringSpell");
            canDoCombo = anim.GetBool("canDoCombo");
            anim.SetBool("isInAir", isInAir);
            isUsingRightHand = anim.GetBool("isUsingRightHand");
            isUsingLeftHand = anim.GetBool("isUsingLeftHand");
            inputHandler.TickInput(delta);
            isTwoHandingWeapon = inputHandler.twoHandFlag;
            isInvulnerable = anim.GetBool("isInvulnerable");
            anim.SetBool("isDead", playerStatsManager.isDead);
            anim.SetBool("isBlocking", isBlocking);

            // 사다리 관련
            anim.SetBool("isClimbing", isClimbing);
            anim.SetBool("isLadderTop", isLadderTop);
            anim.SetBool("rightFootUp", rightFootUp);
            HandleConversation();

            // Rigidbody가 이동되는 움직임이 아니라면 일반적인 Update함수에서 호출해도 괜찮다.
            playerLocomotion.HandleRollingAndSprinting(delta);
            playerLocomotion.HandleJumping();

            playerStatsManager.RegenerateStamina();
            CheckForInteractableObject();

            playerAnimatorManager.canRotate = anim.GetBool("canRotate");

            // 이동키와 백스텝키가 짧은 간격으로 눌리면 백스텝 이후 sprint 애니메이션이 실행되는 경우가 있다.
            // 이를 해결하기 위해 delay 추가
            if (inputHandler.moveAmount == 0) {
                inputHandler.backstepDelay += delta;
            } else {
                inputHandler.backstepDelay = 0;
            }
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            float delta = Time.fixedDeltaTime;

            // Rigidbody를 통해 처리되는 움직임은 FixedUpdate에서 처리되는것이 좋음
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
            playerLocomotion.HandleRotation(delta);
            playerLocomotion.MaintainVelocity();
            if (isClimbing)
                playerLocomotion.HandleClimbing();
            playerEffectsManager.HandleAllBuildUpEffects();
        }

        private void LateUpdate() {
            // 1프레임당 한번의 호출만 이뤄지도록 한다.
            inputHandler.rollFlag = false;
            inputHandler.rb_Input = false;
            inputHandler.rt_Input = false;
            inputHandler.lt_Input = false;
            inputHandler.d_Pad_Up = false;
            inputHandler.d_Pad_Down = false;
            inputHandler.d_Pad_Left = false;
            inputHandler.d_Pad_Right = false;
            inputHandler.a_Input = false;
            inputHandler.jump_Input = false;
            inputHandler.inventory_Input = false;

            // 사다리 상호작용 애니메이션이 끝난후 Animator의 레이어5 에서는 Empty 애니메이션으로 전이가 일어남
            // 사다리의 꼭대기 부분에서 상호작용 애니메이션이 실행될 경우 애니메이션이 끝나도 Rigidbody 의 velocity 가 0이 되지않아 특정 방향으로 끊임없이 밀려나가는데
            // 이는 PlayerAnimatorManager 클래스내의 OnAnimatorMove 메서드 때문인것으로 추정
            // 해당 메서드는 애니메이션이 실행될 때 마지막 프레임까지 호출되며, 애니메이션이 실행되는동안
            // 아바타가 움직인 좌표의 변화량을 측정하고 애니메이션 실행된 시간으로 나눠 속도를 구한후 플레이어의 rigidbody 에 대입하여 자연스럽게보이도록 속도를 대입함
            // 애니메이션의 전이가 모든 애니메이션의 마지막 프레임 이후 실행되는 거라면 상관없지만 자연스러운 전이를 위해 대부분 그러지 않으므로 Rigidbody의 velocity 가 0이 되었다가 마지막 프레임에서
            // 다시 OnAnimatorMove 메서드가 호출되면서 Rigidbody 의 velocity 값이 변화하는 듯
            if (playerAnimatorManager.anim.GetCurrentAnimatorClipInfoCount(5) == 0) { // 한 프레임마다 5번 레이어의 현재 애니메이션이 Empty 라면 rigidbody 의 속도를 0으로 만듬
                playerLocomotion.rigidbody.velocity = Vector3.zero;
            }

            float delta = Time.deltaTime;
            if (cameraHandler != null) {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }

            if (isInAir) { // 플레이어가 허공에 있다면
                playerLocomotion.inAirTimer += Time.deltaTime;
            }
        }

        #region 플레이어 상호작용

        CharacterManager character;
        public void CheckForInteractableObject() {
            if (isInteracting || isInConversation) return; // 행동 중이거나 대화중이라면 다른 오브젝트나 NPC와 상호작용 불가

            Interactable interactableObject; // 주변 상호작용 가능한 오브젝트를 담을 변수
            if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out RaycastHit hit, 1f, interactableLayer)) {
                if (hit.collider.CompareTag("Interactable")) {
                    interactableObject = hit.collider.GetComponent<Interactable>();
                    if (interactableObject != null) { // 주변에 상호작용 가능한 물체가 있다면
                        if (!itemInteractableGameObject.activeSelf) {
                            StartInteraction(interactableObject, hit, false);
                        } else if (inputHandler.a_Input) {
                            itemInteractableGameObject.SetActive(false);
                        }
                    }
                } else if (hit.collider.CompareTag("Character")) {
                    interactableObject = hit.collider.GetComponent<Interactable>();
                    character = hit.collider.GetComponent<CharacterManager>();
                    if (character.canTalk) {
                        if (interactableObject != null) {
                            if (!itemInteractableGameObject.activeSelf) {
                                StartInteraction(interactableObject, hit, false);
                            } else if (inputHandler.a_Input) {
                                itemInteractableGameObject.SetActive(false);
                            }
                        }
                    }
                }
            } else {
                // 주변에 상호작용가능한 오브젝트가 없음에도 메세지창이 떠있지 않도록
                if (interactableUIGameObject != null) {
                    interactableUIGameObject.SetActive(false);
                }
                //아이템을 수집하고 나서 수집키를 한번 더 누르면 메시지창이 닫힌다.
                if (itemInteractableGameObject != null && inputHandler.a_Input) {
                    itemInteractableGameObject.SetActive(false);
                }
            }
        }

        private void StartInteraction(Interactable interactableObject, RaycastHit hit, bool withCharacter) {
            string interactableText = interactableObject.interactableText;
            interactableUI.interactableText.text = interactableText;
            interactableUIGameObject.SetActive(true);

            if (inputHandler.a_Input) {
                interactableUIGameObject.SetActive(false);
                hit.collider.GetComponent<Interactable>().Interact(this); // 해당 오브젝트의 Interact 를 수행
                if (withCharacter) currentPageIndex = 0;
            }
        }

        // 대화 지속
        private void HandleConversation() {
            if (isInConversation) { // 현재 대화중이고
                float distance = Vector3.Distance(character.transform.position, transform.position);
                // 대사를 모두 출력하지 않았다면
                if (currentPageIndex <= currentDialog.Length - 1) {
                    PrintDialog();
                    if (distance >= 3f) FinishConversation();
                } else if (turnPageTimer <= 0) {
                    NPCManager npc = character.GetComponent<NPCManager>();
                    npc.interactCount++; // 대사의 마지막페이지까지 읽었다면 상호작용 횟수를 증가시켜 다음 상호작용시 다른 대사를 출력하게 함
                    FinishConversation();
                }
            }
        }

        // 대화 종료
        private void FinishConversation() {
            isInConversation = false;
            currentPageIndex = 0;
            dialogUI.SetActive(false);
        }

        // 대사 출력
        private void PrintDialog() {
            if (!dialogUI.activeSelf) dialogUI.SetActive(true);
            if (turnPageTimer == 0)
                turnPageTimer = turnPageTime;
            interactableUI.dialogText.text = currentDialog[currentPageIndex].script;
            HandleTurnPageTimer();
            if (turnPageTimer == 0 && currentPageIndex < currentDialog.Length) {
                currentPageIndex++;
            }
        }

        // 다음 대사 출력 타이머
        private void HandleTurnPageTimer() {
            if (turnPageTimer <= 0) turnPageTimer = 0;
            else {
                turnPageTimer -= Time.deltaTime;
                if (inputHandler.a_Input) turnPageTimer -= turnPageTime;
            }
        }

        public void InteractionAtPosition(string animation, Transform playerStandingPosition) {
            characterWeaponSlotManager.rightHandSlot.UnloadWeapon();
            characterWeaponSlotManager.leftHandSlot.UnloadWeapon();
            playerLocomotion.rigidbody.velocity = Vector3.zero;
            playerAnimatorManager.PlayTargetAnimation(animation, true);
            transform.position = playerStandingPosition.position;
        }

        // 안개벽 통과 상호작용
        public void PassThroughFogWallInteraction(Transform fogWallEntrance) {
            playerLocomotion.rigidbody.velocity = Vector3.zero;
            Vector3 rotationDirection = fogWallEntrance.transform.forward;
            Quaternion turnRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = turnRotation;

            playerAnimatorManager.PlayTargetAnimation("PassThroughFog", true);
        }
        #endregion
    }
}
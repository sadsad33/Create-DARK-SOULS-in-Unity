using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerLocomotionManager : CharacterLocomotionManager {
        PlayerManager player;
        PlayerStatsManager playerStatsManager;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public PlayerAnimatorManager playerAnimatorManager;

        //public new Rigidbody rigidbody;
        public GameObject normalCamera;

        #region 리지드 바디를 이용할 경우
        // 플레이어의 Collider를 살짝 위로 들어올렸기 때문에 플레이어의 다리 부분이 묻히게 된다.
        // 따라서 Collider의 끝 부분에서 레이캐스트를 바닥으로 쏴서 착지, 낙하를 감지함
        //[Header("Ground & Air Detection Stats")]
        //[SerializeField]
        //float groundDetectionRayStartPoint = 0.5f; // 플레이어로부터 뻗어나가는 레이캐스트 시작 지점
        //[SerializeField]
        //float minimumDistanceNeededToBeginFall = 1f; // 플레이어가 떨어지는 최소 높이
        //[SerializeField]
        //float groundDirectionRayDistance = 0.2f; // 레이캐스트시작 지점 오프셋
        //LayerMask ignoreForGroundCheck;
        //public Vector3 moveDirection;
        //LayerMask groundCheck;
        //public float inAirTimer;
        //[SerializeField]
        //float fallingSpeed = 80;
        //public CapsuleCollider characterCollider;
        //public CapsuleCollider characterColliderBlocker;
        #endregion

        [Header("Movement Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float walkingSpeed = 1;
        [SerializeField]
        float sprintSpeed = 7;
        [SerializeField]
        float rotationSpeed = 10;
        

        [Header("Stamina Costs")]
        [SerializeField]
        float rollStaminaCost = 15;
        float backstepStaminaCost = 12;
        float sprintStaminaCost = 1f;

        protected override void Awake() {
            base.Awake();
            player = GetComponent<PlayerManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            //rigidbody = GetComponent<Rigidbody>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        }

        protected override void Start() {
            base.Start();
            myTransform = transform;
            groundLayer = (1 << 8 | 1 << 1);
            #region 리지드 바디를 이용할 경우
            //playerManager.isGrounded = true; // 시작할때는 땅에 착지해있다.
            //Physics.IgnoreCollision(characterCollider, characterColliderBlocker, true);
            #endregion
        }

        protected override void Update() {
            base.Update();
            if (player.IsOwner) {
                player.playerNetworkManager.verticalNetworkMovement.Value = verticalMovement;
                player.playerNetworkManager.horizontalNetworkMovement.Value = horizontalMovement;
                player.playerNetworkManager.moveAmountNetworkMovement.Value = movementAmount;
            } else {
                verticalMovement = player.playerNetworkManager.verticalNetworkMovement.Value;
                horizontalMovement = player.playerNetworkManager.horizontalNetworkMovement.Value;
                movementAmount = player.playerNetworkManager.moveAmountNetworkMovement.Value;

                // 만약 플레이어가 질주중이라면 vertical 값에 2 할당
                // 만약 플레이어가 록온을 한 상태라면 horizontal 값은 strafing 에 사용
                if (player.playerNetworkManager.isSprinting.Value) {
                    player.animator.SetFloat("Vertical", 2, 0.1f, Time.deltaTime);
                    player.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
                    return;
                }
                if (player.playerNetworkManager.isLockedOn.Value) {
                    player.animator.SetFloat("Vertical", verticalMovement, 0.1f, Time.deltaTime);
                    player.animator.SetFloat("Horizontal", horizontalMovement, 0.1f, Time.deltaTime);
                } else {
                    player.animator.SetFloat("Vertical", movementAmount, 0.1f, Time.deltaTime);
                    player.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
                }
            }
        }

        #region Movement

        #region 리지드 바디를 이용할 경우
        //Vector3 normalVector;
        //Vector3 targetPosition;
        #endregion

        // 캐릭터 회전
        public void HandleRotation(float delta) {
            if (player.isClimbing || player.isAtBonfire) return;
            if (playerAnimatorManager.canRotate) {
                if (player.playerNetworkManager.isLockedOn.Value) {
                    // 록온을 해도 달리거나 구를때는, 이동하던 방향으로 행동
                    if (player.playerNetworkManager.isSprinting.Value || player.inputHandler.rollFlag) {
                        Vector3 targetDirection = Vector3.zero;
                        targetDirection = player.cameraHandler.cameraTransform.forward * player.inputHandler.vertical;
                        targetDirection += player.cameraHandler.cameraTransform.right * player.inputHandler.horizontal;
                        targetDirection.Normalize();
                        targetDirection.y = 0;
                        if (targetDirection == Vector3.zero) {
                            targetDirection = transform.forward;
                        }
                        Quaternion tr = Quaternion.LookRotation(targetDirection);
                        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                        transform.rotation = targetRotation;
                    } else {
                        Vector3 rotationDirection = moveDirection;
                        rotationDirection = player.cameraHandler.currentLockOnTarget.transform.position - transform.position;
                        rotationDirection.y = 0;
                        rotationDirection.Normalize();
                        Quaternion tr = Quaternion.LookRotation(rotationDirection);
                        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                        transform.rotation = targetRotation;
                    }
                } else {
                    // 바라볼 방향
                    Vector3 targetDir = Vector3.zero;
                    // WASD 방향키 값을 반영한다.
                    targetDir = player.cameraHandler.transform.forward * player.inputHandler.vertical;
                    targetDir += player.cameraHandler.transform.right * player.inputHandler.horizontal;
                    targetDir.Normalize();
                    // 위아래로는 회전하지 않을 것
                    targetDir.y = 0;

                    // 방향 조작이 없다면 캐릭터의 현재좌표에서 정면을 바라봄
                    if (targetDir == Vector3.zero)
                        targetDir = myTransform.forward;

                    float moveOverride = player.inputHandler.moveAmount;

                    // 스크립트에서 회전 처리를 다루는 경우 Quaternion 클래스와 이 클래스의 함수를 사용하여 회전 값을 만들고 수정해야 한다.
                    // 일부의 경우 스크립트에서 오일러 각을 사용하는 것이 더 좋다. 이 경우 각을 변수로 유지하고 회전에 오일러 각으로 적용하는 데만 사용해야 하고 궁극적으로 Quaterion 으로 저장되어야 한다.
                    float rs = rotationSpeed;
                    Quaternion tr = Quaternion.LookRotation(targetDir); // 회전
                    Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta); // 회전이 부드럽게 이어지도록한다
                    myTransform.rotation = targetRotation; // 회전값을 Quaternion으로 저장한다.
                }
            }
        }

        Vector3 jumpDirection;
        // 캐릭터 이동
        public void HandleGroundedMovement(float delta) {
            if (player.inputHandler.rollFlag) return;
            if (player.isInteracting || player.isClimbing || player.isAtBonfire) return;
            if (!player.isGrounded) return;

            #region 리지드 바디를 이용하여 이동할때 필요한 벡터 생성
            // 이동방향에 입력을 반영한다.
            //moveDirection = cameraObject.forward * inputHandler.vertical; // 주된 방향
            //moveDirection += cameraObject.right * inputHandler.horizontal; // 부가적인 방향
            //moveDirection.Normalize();
            //moveDirection.y = 0;
            //float speed = movementSpeed;

            //if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f) {
            //    speed = sprintSpeed;
            //    player.playerNetworkManager.isSprinting.Value = true;
            //    moveDirection *= speed; // 이동속도 반영
            //    jumpDirection = moveDirection; // 점프는 달리기 상태에서만 가능하므로 달리기 상태에서의 벡터를 기억
            //    //Debug.Log("점프 전 : " + jumpDirection);
            //    playerStatsManager.TakeStaminaDamage(sprintStaminaCost);
            //} else {
            //    if (inputHandler.moveAmount <= 0.5f) {
            //        moveDirection *= walkingSpeed;
            //    } else {
            //        moveDirection *= speed;
            //    }
            //    player.playerNetworkManager.isSprinting.Value = false;
            //}

            /*
             * Vector3.ProjectOnPlane(Vector3 vector, Vector3 normalVector)
             * @vector - 정사영하고자 하는 벡터
             * @normalVector - 면의 법선 벡터
             * vector를 normalVector에 수직인 방향으로 투영된 벡터를 반환한다.
             * 플레이어가 이동하는 방향을 바닥면의 법선벡터에 대해 정사영하여 바닥면에 수직인 방향을 구함
             * 기울어진 바닥을 따라 이동할 때, 수직방향을 유지하며 이동할 수 있다.
             */
            //Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            // 위에서 만든 벡터를 rigidbody 에 적용
            //GetComponent<Rigidbody>().velocity = projectedVelocity;
            #endregion

            moveDirection = player.cameraHandler.transform.forward * player.inputHandler.vertical;
            moveDirection += player.cameraHandler.transform.right * player.inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (player.playerNetworkManager.isSprinting.Value && player.inputHandler.moveAmount > 0.5f) {
                player.characterController.Move(moveDirection * sprintSpeed * Time.deltaTime);
                jumpDirection = moveDirection * sprintSpeed;
                playerStatsManager.DeductSprintingStamina(sprintStaminaCost);
            } else {
                if (player.inputHandler.moveAmount > 0.5f) {
                    player.characterController.Move(moveDirection * movementSpeed * Time.deltaTime);
                } else if (player.inputHandler.moveAmount <= 0.5f) {
                    player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                }
            }

            // 록온 상태의경우 수평이동 입력값과 수직이동 입력값을 모두 사용한다.
            if (player.playerNetworkManager.isLockedOn.Value && !player.playerNetworkManager.isSprinting.Value) {
                playerAnimatorManager.UpdateAnimatorValues(player.inputHandler.vertical, player.inputHandler.horizontal, player.playerNetworkManager.isSprinting.Value);
            } else { // 아닐경우 정면방향으로 움직이면 되므로 수직이동값만 사용
                playerAnimatorManager.UpdateAnimatorValues(player.inputHandler.moveAmount, 0, player.playerNetworkManager.isSprinting.Value);
            }
        }

        // 질주,회피
        public void HandleRollingAndSprinting(float delta) {
            // 다른 행동을하고 있다면
            if (player.isInteracting || player.isClimbing || player.isAtBonfire) return;

            if (playerStatsManager.currentStamina <= 0) return;

            if (player.inputHandler.rollFlag) {
                moveDirection = player.cameraHandler.transform.forward * player.inputHandler.vertical;
                moveDirection += player.cameraHandler.transform.right * player.inputHandler.horizontal;

                if (player.inputHandler.moveAmount > 0) { // 이동중이라면 구르기
                    //Debug.Log("구르기!!!");
                    playerAnimatorManager.PlayTargetAnimation("Rolling", true);
                    playerAnimatorManager.EraseHandIKForWeapon();
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                    playerStatsManager.DeductStamina(rollStaminaCost);
                } else { // 이동중이 아니라면 백스텝
                    //Debug.Log("백스텝!!!");
                    if (player.inputHandler.backstepDelay > 1f) {
                        playerAnimatorManager.PlayTargetAnimation("Backstep", true);
                        playerAnimatorManager.EraseHandIKForWeapon();
                        GetComponent<Rigidbody>().AddForce(-myTransform.forward * 20, ForceMode.Impulse);
                        playerStatsManager.DeductStamina(backstepStaminaCost);
                    }
                }
            }
        }

        // 리지드 바디를 이용한 낙하
        //public void HandleFalling(float delta, Vector3 moveDirection) {
        //    if (playerManager.isClimbing|| playerManager.isMoving) return;
        //    if (!playerManager.IsOwner) return;
        //    //playerManager.isGrounded = false;
        //    RaycastHit hit;
        //    Vector3 origin = myTransform.position; // 낙하 시작지점
        //    origin.y += groundDetectionRayStartPoint; // 레이캐스트 시작 지점 설정

        //    if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f)) {
        //        moveDirection = Vector3.zero;
        //    }

        //    if (playerManager.isInAir) {
        //        GetComponent<Rigidbody>().AddForce(-Vector3.up * fallingSpeed); // 아래쪽으로 힘을 받는다.
        //        GetComponent<Rigidbody>().AddForce(moveDirection * fallingSpeed / 12); // 플레이어가 난간에서 발을 떼면 난간에 걸리지 않고 떨어질 수 있도록 밀어줌, 힘의 크기가 작아야 자연스러움
        //    }

        //    Vector3 dir = moveDirection;
        //    dir.Normalize();
        //    origin += dir * groundDirectionRayDistance;
        //    targetPosition = myTransform.position;
        //    Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.05f, false);
        //    if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, groundCheck)) { // 최소 낙하거리 이내에 땅이 존재한다면
        //        normalVector = hit.normal; // 아래쪽으로 레이를 쏴서 부딪힌 지점의 법선 벡터
        //        Vector3 tp = hit.point; // 착지할 곳의 좌표
        //        targetPosition.y = tp.y; // 도착지점의 y좌표는 hit.point의 y좌표가 된다.
        //        if (playerManager.isInAir) { // 플레이어가 공중에 있었다면
        //            if (inAirTimer > 0.5f) { // 공중에 있는 시간이 0.5초보다 길다면
        //                Debug.Log("You were in the air for" + inAirTimer);
        //                playerAnimatorManager.PlayTargetAnimation("Land", true);
        //                playerAnimatorManager.EraseHandIKForWeapon();
        //                playerManager.isInteracting = true;
        //            } else {
        //                Debug.Log("You were in the air for" + inAirTimer);
        //                playerManager.anim.SetTrigger("doEmpty");
        //                //playerAnimationManager.PlayTargetAnimation("Empty", false);
        //            }
        //        }
        //        playerManager.isGrounded = true;
        //        inAirTimer = 0;
        //        playerManager.isInAir = false;
        //    } else { // 현재 땅과의 거리가 최소 낙하거리보다 크다면
        //        if (playerManager.isGrounded) {
        //            playerManager.isGrounded = false; // flag 변경
        //        }
        //        if (!playerManager.isInAir) {
        //            playerManager.isInAir = true; // flag 변경
        //            if (playerManager.isInAir && !playerManager.isInteracting && !playerManager.isGrounded) {
        //                playerAnimatorManager.PlayTargetAnimation("Falling", true); // 낙하 애니메이션 실행
        //                playerAnimatorManager.EraseHandIKForWeapon();
        //            }
        //            Vector3 vel = GetComponent<Rigidbody>().velocity;
        //            vel.Normalize();
        //            GetComponent<Rigidbody>().velocity = vel * (movementSpeed / 2);
        //        }
        //    }

        //    if (playerManager.isGrounded) {
        //        if (playerManager.isInteracting || inputHandler.moveAmount > 0)
        //            myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime / 0.1f);
        //        else
        //            myTransform.position = targetPosition;
        //    }
        //}

        public void HandleJumping() {
            if (player.isInteracting) return;
            if (playerStatsManager.currentStamina <= 0) return;

            if (player.inputHandler.jump_Input) {
                if (player.playerNetworkManager.isSprinting.Value && player.inputHandler.moveAmount > 0) {
                    player.isJumping = true;
                    playerAnimatorManager.PlayTargetAnimation("Jump", true);
                    playerAnimatorManager.EraseHandIKForWeapon();
                    moveDirection.y = 0;
                    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = jumpRotation;
                }
            }
        }

        public void HandleClimbing() {
            if (player.ladderEndPositionDetector.isTopEnd && player.inputHandler.vertical >= 1) {
                //Debug.Log(player.ladderEndPositionDetector.ladderTopFinishingPosition.transform.position);
                player.interactionTargetPosition = player.ladderEndPositionDetector.ladderTopFinishingPosition.transform;
                player.isMoving = true;
                if (player.rightFootUp) {
                    player.playerInteractionManager.InteractionAtPosition("Ladder_End_Top_RightFootUp", player.transform);
                } else {
                    player.playerInteractionManager.InteractionAtPosition("Ladder_End_Top_LeftFootUp", player.transform);
                }
                player.isClimbing = false;
            } else if (player.ladderEndPositionDetector.isBottomEnd && player.inputHandler.vertical <= -1) {
                if (player.rightFootUp) {
                    player.playerInteractionManager.InteractionAtPosition("Ladder_End_Bottom_RightFootUp", player.ladderEndPositionDetector.ladderBottomFinishingPosition.transform);
                } else {
                    player.playerInteractionManager.InteractionAtPosition("Ladder_End_Bottom_LeftFootUp", player.ladderEndPositionDetector.ladderBottomFinishingPosition.transform);
                }
                player.isClimbing = false;
            } else {
                player.animator.SetFloat("Vertical", player.inputHandler.vertical, 0.1f, Time.deltaTime);
                //GetComponent<Rigidbody>().velocity = new Vector3(0, player.inputHandler.vertical, 0);
                Vector3 climbingDirection = new Vector3(0, player.inputHandler.vertical / 200, 0);
                player.characterController.Move(climbingDirection);
            }
        }

        public void MaintainVelocity() {
            if (!player.isJumping) return;
            player.characterController.Move(jumpDirection / 2 * Time.deltaTime);

            //GetComponent<Rigidbody>().velocity = jumpDirection;
            // rigidbody.MovePosition : 충돌연산의 영향을 받으며 물체를 이동시키는 메서드
            // 중력이나 가속, 감속같은 연속적인 물리효과에 대해서 영향을 받지는 않으면서 부드럽게 물체를 이동시킴
            //rigidbody.MovePosition(transform.position + jumpDirection * Time.deltaTime);
        }

        public void SetMovementValues(float vertical, float horizontal, float moveAmount) {
            verticalMovement = vertical;
            horizontalMovement = horizontal;
            movementAmount = moveAmount;
        }
        #endregion
    }
}
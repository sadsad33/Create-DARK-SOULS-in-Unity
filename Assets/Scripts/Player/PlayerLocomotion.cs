using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerLocomotion : MonoBehaviour {
        CameraHandler cameraHandler;
        Transform cameraObject;
        InputHandler inputHandler;
        PlayerManager playerManager;
        public Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public CapsuleCollider characterCollider;
        public CapsuleCollider characterColliderBlocker;
        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        // 플레이어의 Collider를 살짝 위로 들어올렸기 때문에 플레이어의 다리 부분이 묻히게 된다.
        // 따라서 Collider의 끝 부분에서 레이캐스트를 바닥으로 쏴서 착지, 낙하를 감지함
        [Header("Ground & Air Detection Stats")]
        [SerializeField]
        float groundDetectionRayStartPoint = 0.5f; // 플레이어로부터 뻗어나가는 레이캐스트 시작 지점
        [SerializeField]
        float minimumDistanceNeededToBeginFall = 1f; // 플레이어가 떨어지는 최소 높이
        [SerializeField]
        float groundDirectionRayDistance = 0.2f; // 레이캐스트시작 지점 오프셋
        LayerMask ignoreForGroundCheck;
        public float inAirTimer;

        [Header("Movement Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float walkingSpeed = 1;
        [SerializeField]
        float sprintSpeed = 7;
        [SerializeField]
        float rotationSpeed = 10;
        [SerializeField]
        float fallingSpeed = 80;

        private void Awake() {
            cameraHandler = FindObjectOfType<CameraHandler>();
        }

        void Start() {
            playerManager = GetComponent<PlayerManager>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();

            playerManager.isGrounded = true; // 시작할때는 땅에 착지해있다.
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11); // 착지를 판단할때 무시할 레이어
            Physics.IgnoreCollision(characterCollider, characterColliderBlocker, true);
        }


        #region Movement

        Vector3 normalVector;
        Vector3 targetPosition;

        // 캐릭터 회전
        private void HandleRotation(float delta) {
            if (inputHandler.lockOnFlag) {
                // 록온을 해도 달리거나 구를때는, 이동하던 방향으로 행동
                if (inputHandler.sprintFlag || inputHandler.rollFlag) {
                    Vector3 targetDirection = Vector3.zero;
                    targetDirection = cameraHandler.cameraTransform.forward * inputHandler.vertical;
                    targetDirection += cameraHandler.cameraTransform.right * inputHandler.horizontal;
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
                    rotationDirection = cameraHandler.currentLockOnTarget.position - transform.position;
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
                targetDir = cameraObject.forward * inputHandler.vertical;
                targetDir += cameraObject.right * inputHandler.horizontal;
                targetDir.Normalize();
                // 위아래로는 회전하지 않을 것
                targetDir.y = 0;

                // 방향 조작이 없다면 캐릭터의 현재좌표에서 정면을 바라봄
                if (targetDir == Vector3.zero)
                    targetDir = myTransform.forward;

                float moveOverride = inputHandler.moveAmount;

                // 스크립트에서 회전 처리를 다루는 경우 Quaternion 클래스와 이 클래스의 함수를 사용하여 회전 값을 만들고 수정해야 한다.
                // 일부의 경우 스크립트에서 오일러 각을 사용하는 것이 더 좋다. 이 경우 각을 변수로 유지하고 회전에 오일러 각으로 적용하는 데만 사용해야 하고 궁극적으로 Quaterion 으로 저장되어야 한다.
                float rs = rotationSpeed;
                Quaternion tr = Quaternion.LookRotation(targetDir); // 회전
                Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta); // 회전이 부드럽게 이어지도록한다
                myTransform.rotation = targetRotation; // 회전값을 Quaternion으로 저장한다.
            }
        }

        // 캐릭터 이동
        public void HandleMovement(float delta) {
            if (inputHandler.rollFlag) return;
            if (playerManager.isInteracting) return;
            // 이동방향에 입력을 반영한다.
            moveDirection = cameraObject.forward * inputHandler.vertical; // 주된 방향
            moveDirection += cameraObject.right * inputHandler.horizontal; // 부가적인 방향
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;

            if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f) {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed; // 이동속도 반영
            } else {
                if (inputHandler.moveAmount < 0.5f) {
                    moveDirection *= walkingSpeed;
                    playerManager.isSprinting = false;
                } else {
                    moveDirection *= speed;
                    playerManager.isSprinting = false;
                }
            }

            /*
             * Vector3.ProjectOnPlane(Vector3 vector, Vector3 normalVector)
             * @vector - 정사영하고자 하는 벡터
             * @normalVector - 면의 법선 벡터
             * vector를 normalVector에 수직인 방향으로 투영된 벡터를 반환한다.
             * 플레이어가 이동하는 방향을 바닥면의 법선벡터에 대해 정사영하여 바닥면에 수직인 방향을 구함
             * 기울어진 바닥을 따라 이동할 때, 수직방향을 유지하며 이동할 수 있다.
             */
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            // 록온 상태의경우 수평이동 입력값과 수직이동 입력값을 모두 사용한다.
            if (inputHandler.lockOnFlag && !inputHandler.sprintFlag) { 
                animatorHandler.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal, playerManager.isSprinting);
            } else { // 아닐경우 정면방향으로 움직이면 되므로 수직이동값만 사용
                animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);
            }

            if (animatorHandler.canRotate)
                HandleRotation(delta);
        }

        // 질주,회피
        public void HandleRollingAndSprinting(float delta) {
            if (animatorHandler.anim.GetBool("isInteracting")) // 다른 행동을하고 있다면
                return;

            if (inputHandler.rollFlag) {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;

                if (inputHandler.moveAmount > 0) { // 이동중이라면 구르기
                    //Debug.Log("구르기!!!");
                    animatorHandler.PlayTargetAnimation("Rolling", true);
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                } else { // 이동중이 아니라면 백스텝
                    //Debug.Log("백스텝!!!");
                    if (inputHandler.backstepDelay > 0.3f) {
                        animatorHandler.PlayTargetAnimation("Backstep", true);
                        rigidbody.AddForce(-myTransform.forward * 20, ForceMode.Impulse);
                    }
                }
            }
        }

        // 낙하
        public void HandleFalling(float delta, Vector3 moveDirection) {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position; // 낙하 시작지점
            origin.y += groundDetectionRayStartPoint; // 레이캐스트 시작 지점 설정

            if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f)) {
                moveDirection = Vector3.zero;
            }

            if (playerManager.isInAir) {
                rigidbody.AddForce(-Vector3.up * fallingSpeed); // 아래쪽으로 힘을 받는다.
                rigidbody.AddForce(moveDirection * fallingSpeed / 7); // 플레이어가 난간에서 발을 떼면 난간에 걸리지 않고 떨어질 수 있도록 밀어줌, 힘의 크기가 작아야 자연스러움
            }

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin += dir * groundDirectionRayDistance;
            targetPosition = myTransform.position;
            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck)) { // 최소 낙하거리 이내에 땅이 존재한다면
                normalVector = hit.normal; // 아래쪽으로 레이를 쏴서 부딪힌 지점의 법선 벡터
                Vector3 tp = hit.point; // 착지할 곳의 좌표
                playerManager.isGrounded = true;
                targetPosition.y = tp.y; // 도착지점의 y좌표는 hit.point의 y좌표가 된다.

                if (playerManager.isInAir) { // 플레이어가 공중에 있다면
                    if (inAirTimer > 0.5f) { // 공중에 있는 시간이 0.5초보다 길다면
                        Debug.Log("You were in the air for" + inAirTimer);
                        animatorHandler.PlayTargetAnimation("Land", true);
                        playerManager.isInteracting = true;
                    } else {
                        animatorHandler.PlayTargetAnimation("Empty", false);
                    }
                    inAirTimer = 0;
                    playerManager.isInAir = false;
                }
            } else { // 현재 땅과의 거리가 최소 낙하거리보다 크다면
                if (playerManager.isGrounded) {
                    playerManager.isGrounded = false; // flag 변경
                }
                if (!playerManager.isInAir) {
                    if (!playerManager.isInteracting) {
                        animatorHandler.PlayTargetAnimation("Falling", true); // 낙하 애니메이션 실행
                    }
                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                    playerManager.isInAir = true; // flag 변경
                }
            }

            //if (playerManager.isGrounded) {
            //    if (playerManager.isInteracting || inputHandler.moveAmount > 0) {
            //        myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, Time.deltaTime);
            //    } else {
            //        myTransform.position = targetPosition;
            //    }
            //}

            if (playerManager.isInteracting || inputHandler.moveAmount > 0)
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime / 0.1f);
            else
                myTransform.position = targetPosition;

        }

        public void HandleJumping() {
            if (playerManager.isInteracting) return;

            if (inputHandler.jump_Input) {
                if (inputHandler.sprintFlag && inputHandler.moveAmount > 0) {
                    moveDirection = cameraObject.forward * inputHandler.vertical;
                    moveDirection += cameraObject.right * inputHandler.horizontal;

                    //// 리지드 바디의 현재 속도를 저장
                    //Vector3 currentVelocity = rigidbody.velocity;
                    //// 점프할 방향
                    //Vector3 jumpDirection = (moveDirection + rigidbody.transform.up).normalized;
                    //// 현재 속도에 점프 속도를 합성
                    //rigidbody.velocity = currentVelocity + jumpDirection * 30;

                    animatorHandler.PlayTargetAnimation("Jump", true);
                    moveDirection.y = 0;
                    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = jumpRotation;
                }
            }
        }
        #endregion


    }
}
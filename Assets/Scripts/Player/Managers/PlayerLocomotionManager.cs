using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerLocomotionManager : MonoBehaviour {
        CameraHandler cameraHandler;
        Transform cameraObject;
        InputHandler inputHandler;
        PlayerManager playerManager;
        PlayerStatsManager playerStatsManager;
        public Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public PlayerAnimatorManager playerAnimationManager;

        public CapsuleCollider characterCollider;
        public CapsuleCollider characterColliderBlocker;
        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        // �÷��̾��� Collider�� ��¦ ���� ���÷ȱ� ������ �÷��̾��� �ٸ� �κ��� ������ �ȴ�.
        // ���� Collider�� �� �κп��� ����ĳ��Ʈ�� �ٴ����� ���� ����, ���ϸ� ������
        [Header("Ground & Air Detection Stats")]
        [SerializeField]
        float groundDetectionRayStartPoint = 0.5f; // �÷��̾�κ��� ������� ����ĳ��Ʈ ���� ����
        [SerializeField]
        float minimumDistanceNeededToBeginFall = 1f; // �÷��̾ �������� �ּ� ����
        [SerializeField]
        float groundDirectionRayDistance = 0.2f; // ����ĳ��Ʈ���� ���� ������
        //LayerMask ignoreForGroundCheck;
        LayerMask groundCheck;
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

        [Header("Stamina Costs")]
        [SerializeField]
        float rollStaminaCost = 15;
        float backstepStaminaCost = 12;
        float sprintStaminaCost = 1;

        public BoxCollider jumpCollider;
        private void Awake() {
            jumpCollider.enabled = false;
            cameraHandler = FindObjectOfType<CameraHandler>();
            playerManager = GetComponent<PlayerManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            playerAnimationManager = GetComponent<PlayerAnimatorManager>();
        }

        void Start() {
            cameraObject = Camera.main.transform;
            myTransform = transform;

            playerManager.isGrounded = true; // �����Ҷ��� ���� �������ִ�.
            //ignoreForGroundCheck = ~(1 << 8 | 1 << 11); // ������ �Ǵ��Ҷ� ������ ���̾�
            groundCheck = (1 << 8 | 1 << 1);
            Physics.IgnoreCollision(characterCollider, characterColliderBlocker, true);
        }


        #region Movement

        Vector3 normalVector;
        Vector3 targetPosition;

        // ĳ���� ȸ��
        public void HandleRotation(float delta) {
            if (playerAnimationManager.canRotate) {
                if (inputHandler.lockOnFlag) {
                    // �Ͽ��� �ص� �޸��ų� ��������, �̵��ϴ� �������� �ൿ
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
                        rotationDirection = cameraHandler.currentLockOnTarget.transform.position - transform.position;
                        rotationDirection.y = 0;
                        rotationDirection.Normalize();
                        Quaternion tr = Quaternion.LookRotation(rotationDirection);
                        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                        transform.rotation = targetRotation;
                    }
                } else {
                    // �ٶ� ����
                    Vector3 targetDir = Vector3.zero;
                    // WASD ����Ű ���� �ݿ��Ѵ�.
                    targetDir = cameraObject.forward * inputHandler.vertical;
                    targetDir += cameraObject.right * inputHandler.horizontal;
                    targetDir.Normalize();
                    // ���Ʒ��δ� ȸ������ ���� ��
                    targetDir.y = 0;

                    // ���� ������ ���ٸ� ĳ������ ������ǥ���� ������ �ٶ�
                    if (targetDir == Vector3.zero)
                        targetDir = myTransform.forward;

                    float moveOverride = inputHandler.moveAmount;

                    // ��ũ��Ʈ���� ȸ�� ó���� �ٷ�� ��� Quaternion Ŭ������ �� Ŭ������ �Լ��� ����Ͽ� ȸ�� ���� ����� �����ؾ� �Ѵ�.
                    // �Ϻ��� ��� ��ũ��Ʈ���� ���Ϸ� ���� ����ϴ� ���� �� ����. �� ��� ���� ������ �����ϰ� ȸ���� ���Ϸ� ������ �����ϴ� ���� ����ؾ� �ϰ� �ñ������� Quaterion ���� ����Ǿ�� �Ѵ�.
                    float rs = rotationSpeed;
                    Quaternion tr = Quaternion.LookRotation(targetDir); // ȸ��
                    Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta); // ȸ���� �ε巴�� �̾��������Ѵ�
                    myTransform.rotation = targetRotation; // ȸ������ Quaternion���� �����Ѵ�.
                }
            }
        }

        // ĳ���� �̵�
        public void HandleMovement(float delta) {
            if (inputHandler.rollFlag) return;
            if (playerManager.isInteracting) return;
            // �̵����⿡ �Է��� �ݿ��Ѵ�.
            moveDirection = cameraObject.forward * inputHandler.vertical; // �ֵ� ����
            moveDirection += cameraObject.right * inputHandler.horizontal; // �ΰ����� ����
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;

            if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f) {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed; // �̵��ӵ� �ݿ�
                playerStatsManager.TakeStaminaDamage(sprintStaminaCost);
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
             * @vector - ���翵�ϰ��� �ϴ� ����
             * @normalVector - ���� ���� ����
             * vector�� normalVector�� ������ �������� ������ ���͸� ��ȯ�Ѵ�.
             * �÷��̾ �̵��ϴ� ������ �ٴڸ��� �������Ϳ� ���� ���翵�Ͽ� �ٴڸ鿡 ������ ������ ����
             * ������ �ٴ��� ���� �̵��� ��, ���������� �����ϸ� �̵��� �� �ִ�.
             */
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            // �Ͽ� �����ǰ�� �����̵� �Է°��� �����̵� �Է°��� ��� ����Ѵ�.
            if (inputHandler.lockOnFlag && !inputHandler.sprintFlag) {
                playerAnimationManager.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal, playerManager.isSprinting);
            } else { // �ƴҰ�� ����������� �����̸� �ǹǷ� �����̵����� ���
                playerAnimationManager.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);
            }
        }

        // ����,ȸ��
        public void HandleRollingAndSprinting(float delta) {
            if (playerAnimationManager.anim.GetBool("isInteracting")) // �ٸ� �ൿ���ϰ� �ִٸ�
                return;

            if (playerStatsManager.currentStamina <= 0) return;

            if (inputHandler.rollFlag) {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;

                if (inputHandler.moveAmount > 0) { // �̵����̶�� ������
                    //Debug.Log("������!!!");
                    playerAnimationManager.PlayTargetAnimation("Rolling", true);
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                    playerStatsManager.TakeStaminaDamage(rollStaminaCost);
                } else { // �̵����� �ƴ϶�� �齺��
                    //Debug.Log("�齺��!!!");
                    if (inputHandler.backstepDelay > 0.3f) {
                        playerAnimationManager.PlayTargetAnimation("Backstep", true);
                        rigidbody.AddForce(-myTransform.forward * 20, ForceMode.Impulse);
                        playerStatsManager.TakeStaminaDamage(backstepStaminaCost);
                    }
                }
            }
        }

        // ����
        public void HandleFalling(float delta, Vector3 moveDirection) {
            //playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position; // ���� ��������
            origin.y += groundDetectionRayStartPoint; // ����ĳ��Ʈ ���� ���� ����

            if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f)) {
                moveDirection = Vector3.zero;
            }

            if (playerManager.isInAir) {
                rigidbody.AddForce(-Vector3.up * fallingSpeed); // �Ʒ������� ���� �޴´�.
                rigidbody.AddForce(moveDirection * fallingSpeed / 12); // �÷��̾ �������� ���� ���� ������ �ɸ��� �ʰ� ������ �� �ֵ��� �о���, ���� ũ�Ⱑ �۾ƾ� �ڿ�������
            }

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin += dir * groundDirectionRayDistance;
            targetPosition = myTransform.position;
            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.05f, false);
            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, groundCheck)) { // �ּ� ���ϰŸ� �̳��� ���� �����Ѵٸ�
                normalVector = hit.normal; // �Ʒ������� ���̸� ���� �ε��� ������ ���� ����
                Vector3 tp = hit.point; // ������ ���� ��ǥ
                targetPosition.y = tp.y; // ���������� y��ǥ�� hit.point�� y��ǥ�� �ȴ�.
                if (playerManager.isInAir) { // �÷��̾ ���߿� �־��ٸ�
                    if (inAirTimer > 0.5f) { // ���߿� �ִ� �ð��� 0.5�ʺ��� ��ٸ�
                        Debug.Log("You were in the air for" + inAirTimer);
                        playerAnimationManager.PlayTargetAnimation("Land", true);
                        playerManager.isInteracting = true;
                    } else {
                        Debug.Log("You were in the air for" + inAirTimer);
                        playerAnimationManager.anim.SetTrigger("doEmpty");
                        //playerAnimationManager.PlayTargetAnimation("Empty", false);
                    }
                }
                playerManager.isGrounded = true;
                inAirTimer = 0;
                playerManager.isInAir = false;
            } else { // ���� ������ �Ÿ��� �ּ� ���ϰŸ����� ũ�ٸ�
                if (playerManager.isGrounded) {
                    playerManager.isGrounded = false; // flag ����
                }
                if (!playerManager.isInAir) {
                    playerManager.isInAir = true; // flag ����
                    if (playerManager.isInAir && !playerManager.isInteracting && !playerManager.isGrounded) {
                        playerAnimationManager.PlayTargetAnimation("Falling", true); // ���� �ִϸ��̼� ����
                    }
                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                }
            }

            if (playerManager.isGrounded) {
                if (playerManager.isInteracting || inputHandler.moveAmount > 0)
                    myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime / 0.1f);
                else
                    myTransform.position = targetPosition;
            }
        }

        public void HandleJumping() {
            if (playerManager.isInteracting) return;
            if (playerStatsManager.currentStamina <= 0) return;

            if (inputHandler.jump_Input) {
                if (inputHandler.sprintFlag && inputHandler.moveAmount > 0) {
                    moveDirection = cameraObject.forward * inputHandler.vertical;
                    moveDirection += cameraObject.right * inputHandler.horizontal;
                    StartCoroutine(JumpBooster());
                    playerAnimationManager.PlayTargetAnimation("Jump", true);
                    moveDirection.y = 0;
                    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = jumpRotation;
                }
            }
        }
        #endregion
        IEnumerator JumpBooster() {
            jumpCollider.enabled = true;
            yield return new WaitForSeconds(0.75f);
            jumpCollider.enabled = false;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerLocomotion : MonoBehaviour {
        Transform cameraObject;
        InputHandler inputHandler;
        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float rotationSpeed = 10;

        void Start() {
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();
        }

        public void Update() {
            float delta = Time.deltaTime;
            inputHandler.TickInput(delta);
            HandleMovement(delta);
            HandleRollingAndSprinting(delta);
        }
        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        // 캐릭터 회전
        private void HandleRotation(float delta) {
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

        public void HandleMovement(float delta) {
            // 이동방향에 입력을 반영한다.
            moveDirection = cameraObject.forward * inputHandler.vertical; // 주된 방향
            moveDirection += cameraObject.right * inputHandler.horizontal; // 부가적인 방향
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;
            moveDirection *= speed; // 이동속도 반영

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

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);

            if (animatorHandler.canRotate)
                HandleRotation(delta);
        }

        public void HandleRollingAndSprinting(float delta) {
            if(animatorHandler.anim.GetBool("isInteracting")) // 다른 행동을하고 있다면
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
                    animatorHandler.PlayTargetAnimation("Backstep", true);
                }
            }
        }
        #endregion
    }
}
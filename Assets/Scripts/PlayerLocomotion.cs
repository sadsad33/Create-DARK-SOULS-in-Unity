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
            // 이동방향에 입력을 반영한다.
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();

            float speed = movementSpeed;
            moveDirection *= speed; // 이동속도 반영

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);

            if(animatorHandler.canRotate)
                HandleRotation(delta);
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
            if(targetDir == Vector3.zero)
                targetDir = myTransform.forward;
            
            float moveOverride = inputHandler.moveAmount;

            // 스크립트에서 회전 처리를 다루는 경우 Quaternion 클래스와 이 클래스의 함수를 사용하여 회전 값을 만들고 수정해야 한다.
            // 일부의 경우 스크립트에서 오일러 각을 사용하는 것이 더 좋다. 이 경우 각을 변수로 유지하고 회전에 오일러 각으로 적용하는 데만 사용해야 하고 궁극적으로 Quaterion 으로 저장되어야 한다.
            float rs = rotationSpeed;
            Quaternion tr = Quaternion.LookRotation(targetDir); // 회전
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta); // 회전이 부드럽게 이어지도록한다
            myTransform.rotation = targetRotation; // 회전값을 Quaternion으로 저장한다.
        }
        #endregion
    }
}
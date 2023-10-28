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
            // �̵����⿡ �Է��� �ݿ��Ѵ�.
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();

            float speed = movementSpeed;
            moveDirection *= speed; // �̵��ӵ� �ݿ�

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);

            if(animatorHandler.canRotate)
                HandleRotation(delta);
        }
        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        // ĳ���� ȸ��
        private void HandleRotation(float delta) {
            // �ٶ� ����
            Vector3 targetDir = Vector3.zero;
            // WASD ����Ű ���� �ݿ��Ѵ�.
            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;
            targetDir.Normalize();
            // ���Ʒ��δ� ȸ������ ���� ��
            targetDir.y = 0;

            // ���� ������ ���ٸ� ĳ������ ������ǥ���� ������ �ٶ�
            if(targetDir == Vector3.zero)
                targetDir = myTransform.forward;
            
            float moveOverride = inputHandler.moveAmount;

            // ��ũ��Ʈ���� ȸ�� ó���� �ٷ�� ��� Quaternion Ŭ������ �� Ŭ������ �Լ��� ����Ͽ� ȸ�� ���� ����� �����ؾ� �Ѵ�.
            // �Ϻ��� ��� ��ũ��Ʈ���� ���Ϸ� ���� ����ϴ� ���� �� ����. �� ��� ���� ������ �����ϰ� ȸ���� ���Ϸ� ������ �����ϴ� ���� ����ؾ� �ϰ� �ñ������� Quaterion ���� ����Ǿ�� �Ѵ�.
            float rs = rotationSpeed;
            Quaternion tr = Quaternion.LookRotation(targetDir); // ȸ��
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta); // ȸ���� �ε巴�� �̾��������Ѵ�
            myTransform.rotation = targetRotation; // ȸ������ Quaternion���� �����Ѵ�.
        }
        #endregion
    }
}
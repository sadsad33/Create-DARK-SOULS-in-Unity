using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class CameraHandler : MonoBehaviour {
        // ī�޶� ������ Ÿ���� ��ġ
        public Transform targetTransform;
        // ī�޶��� ��ġ
        public Transform cameraTransform;
        // ī�޶��� ȸ�� �߽� ��ġ(?)
        public Transform cameraPivotTransform;
        private Transform myTransform; // CameraHolder�� Transform (= Player�� Transform)
        private Vector3 cameraTransformPosition;
        private LayerMask ignoreLayers;
        private Vector3 cameraFollowVelocity = Vector3.zero;

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float targetPosition;
        private float defaultPosition;
        
        // Euler Angle�� ����ϱ� ���� ����
        private float lookAngle;
        private float pivotAngle;

        public float minimumPivot = -35f;
        public float maximumPivot = 35f;

        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;

        private void Awake() {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        // ī�޶� ����� ���󰡵��� �ϴ� �Լ�
        public void FollowTarget(float delta) {
            //Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);

            // ��ǥ �������� �ε巴�� �̵��Ѵ�
            Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPosition;

            HandleCameraCollision(delta);

            Debug.Log("�÷��̾� Position : " + targetTransform.position);
            Debug.Log("ī�޶� PivotPosition : " + cameraPivotTransform.position);
        }

        /*
         * ī�޶� ȸ��
         * @delta - FixeddeltaTime
         * @mousXInput - ���콺 �¿� ȸ����
         * @mouseYInput - ���콺 ���� ȸ����
         */
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput) {
            // �ð��� ���� ������ ȸ���� ��ȭ���� ����
            lookAngle += (mouseXInput * lookSpeed) / delta; 
            pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            Vector3 rotation = Vector3.zero; // Euler Angle ���� ������ ���� ����
            rotation.y = lookAngle; // Euler Angle�� y ���� ���콺 �¿� ȸ������ �������ش�.
            Quaternion targetRotation = Quaternion.Euler(rotation); // Quaternion���� ��ȯ
            myTransform.rotation = targetRotation; // ī�޶� ȸ������ Quaternion���� �����.
            
            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        // ī�޶� �ٸ� ��ü�� �浹�� ��� �ذ��ϴ� �Լ�
        private void HandleCameraCollision(float delta) {
            targetPosition = defaultPosition;
            RaycastHit hit;
            // ī�޶�� �÷��̾� ������ ȸ���Ѵ�.
            // cameraPivotTransform �� ī�޶� ȸ���� �߽�����, ��ǥ�� �÷��̾��� Transform.position �� ����.
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position; // �÷��̾��� ��ǥ�κ��� ī�޶������ ����
            direction.Normalize();
            
            Debug.DrawRay(cameraPivotTransform.position, direction, Color.magenta);

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers)) {
                // �÷��̾��� ��ǥ�κ��� ī�޶��� �������� ray�� �߻��Ͽ� ī�޶� ������ ���𰡿� �浹���� ��� �浹�� ��ǥ������ �Ÿ�
                float dist = Vector3.Distance(cameraPivotTransform.position, hit.point); 
                
                // ī�޶� �̵��ؾ��� z ��ǥ�� �����Ѵ�.
                // ī�޶�� pivot���� �׻� �ڿ�(z ��ǥ�� ����)�־�� �ϹǷ� ���� ���� �Ǿ�� �Ѵ�.
                targetPosition = -(dist - cameraCollisionOffset);
            }

            // ��ü�� �÷��̾� ������ �Ÿ��� �ʹ� �����ٸ� �ּ�ġ�� ������
            if (Mathf.Abs(targetPosition) < minimumCollisionOffset) {
                targetPosition = -minimumCollisionOffset;
            }

            Debug.Log("ī�޶� �̵��Ÿ�(?)" + targetPosition);
            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class CameraHandler : MonoBehaviour {
        // 카메라가 포착할 타겟의 위치
        public Transform targetTransform;
        // 카메라의 위치
        public Transform cameraTransform;
        // 카메라의 회전 중심 위치(?)
        public Transform cameraPivotTransform;
        private Transform myTransform; // CameraHolder의 Transform (= Player의 Transform)
        private Vector3 cameraTransformPosition;
        private LayerMask ignoreLayers;
        private Vector3 cameraFollowVelocity = Vector3.zero;

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float targetPosition;
        private float defaultPosition;
        
        // Euler Angle을 사용하기 위한 변수
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

        // 카메라가 대상을 따라가도록 하는 함수
        public void FollowTarget(float delta) {
            //Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);

            // 목표 지점까지 부드럽게 이동한다
            Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPosition;

            HandleCameraCollision(delta);

            Debug.Log("플레이어 Position : " + targetTransform.position);
            Debug.Log("카메라 PivotPosition : " + cameraPivotTransform.position);
        }

        /*
         * 카메라 회전
         * @delta - FixeddeltaTime
         * @mousXInput - 마우스 좌우 회전값
         * @mouseYInput - 마우스 상하 회전값
         */
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput) {
            // 시간에 따른 각각의 회전값 변화량을 대입
            lookAngle += (mouseXInput * lookSpeed) / delta; 
            pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            Vector3 rotation = Vector3.zero; // Euler Angle 값을 저장할 변수 생성
            rotation.y = lookAngle; // Euler Angle의 y 값에 마우스 좌우 회전값을 대입해준다.
            Quaternion targetRotation = Quaternion.Euler(rotation); // Quaternion으로 변환
            myTransform.rotation = targetRotation; // 카메라 회전값을 Quaternion으로 만든다.
            
            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        // 카메라가 다른 물체와 충돌할 경우 해결하는 함수
        private void HandleCameraCollision(float delta) {
            targetPosition = defaultPosition;
            RaycastHit hit;
            // 카메라는 플레이어 주위를 회전한다.
            // cameraPivotTransform 은 카메라 회전의 중심으로, 좌표는 플레이어의 Transform.position 과 같다.
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position; // 플레이어의 좌표로부터 카메라까지의 방향
            direction.Normalize();
            
            Debug.DrawRay(cameraPivotTransform.position, direction, Color.magenta);

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers)) {
                // 플레이어의 좌표로부터 카메라의 방향으로 ray를 발사하여 카메라를 제외한 무언가와 충돌했을 경우 충돌한 좌표까지의 거리
                float dist = Vector3.Distance(cameraPivotTransform.position, hit.point); 
                
                // 카메라가 이동해야할 z 좌표를 설정한다.
                // 카메라는 pivot보다 항상 뒤에(z 좌표가 음수)있어야 하므로 음수 값이 되어야 한다.
                targetPosition = -(dist - cameraCollisionOffset);
            }

            // 물체와 플레이어 사이의 거리가 너무 가깝다면 최소치로 맞춰줌
            if (Mathf.Abs(targetPosition) < minimumCollisionOffset) {
                targetPosition = -minimumCollisionOffset;
            }

            Debug.Log("카메라 이동거리(?)" + targetPosition);
            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CameraHandler : MonoBehaviour {

        // 카메라가 포착할 타겟의 위치
        public Transform targetTransform;
        // 카메라의 위치(중심 기준)
        public Transform cameraTransform;
        private Vector3 cameraTransformPosition;
        // 카메라의 중심 위치
        public Transform cameraPivotTransform;
        
        public LayerMask obstacleLayer; // 플레이어와 카메라 사이에 오브젝트가 존재할 경우 카메라를 플레이어에게 가깝게 밀착시키기 위해 충돌 체크를 하고 싶은 레이어

        private Transform myTransform; // CameraHolder의 Transform (= Player의 Transform)
        private Vector3 cameraFollowVelocity = Vector3.zero;

        public static CameraHandler instance;
        public InputHandler inputHandler;
        public PlayerManager player;

        public float lookSpeed = 0.1f;
        public float groundedFollowSpeed = 20f;
        public float inAirFollowSpeed = 20f;
        public float pivotSpeed = 0.03f;

        private float targetPosition;
        private float defaultPosition; // 평소 카메라의 z좌표를 저장할 변수

        // Euler Angle을 사용하기 위한 변수
        private float lookAngle;
        private float pivotAngle;

        public float minimumPivot = -35f;
        public float maximumPivot = 35f;

        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;
        public float lockedPivotPosition = 2.25f;
        public float unlockedPivotPosition = 1.65f;

        List<CharacterManager> availableTargets = new();
        public CharacterManager nearestLockOnTarget;
        public float maximumLockOnDistance;
        public CharacterManager leftLockTarget, rightLockTarget;

        private void Awake() {
            if (instance == null) instance = this;
            else Destroy(gameObject);
            DontDestroyOnLoad(this);
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            obstacleLayer = 1 << 8;
            inputHandler = FindObjectOfType<InputHandler>();
        }

        public void AssignCameraToPlayer(PlayerManager player) {
            this.player = player;
            targetTransform = player.transform;
            inputHandler = player.inputHandler;
        }
        
        // 카메라가 대상을 따라가도록 하는 함수
        public void FollowTarget(float delta) {

            if (player.isGrounded) {
                // 목표 지점까지 부드럽게 이동한다
                Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, groundedFollowSpeed * Time.deltaTime);
                //Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
                myTransform.position = targetPosition;
            } else {
                Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, inAirFollowSpeed * Time.deltaTime);
                myTransform.position = targetPosition;
            }
            ZoomForInteraction(delta);
            HandleCameraCollision(delta);

            //Debug.Log("플레이어 Position : " + targetTransform.position);
            //Debug.Log("카메라 PivotPosition : " + cameraPivotTransform.position);
        }

        /*
         * 카메라 회전(구면 좌표계)
         * @delta - FixeddeltaTime
         * @mousXInput - 마우스 좌우 회전값
         * @mouseYInput - 마우스 상하 회전값
         */
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput) {
            // 록온을 하지 않았을 경우
            if (!player.playerNetworkManager.isLockedOn.Value && player.currentTarget == null) {
                // 시간에 따른 각각의 회전값 변화량을 대입
                lookAngle += (mouseXInput * lookSpeed) * delta; // 좌우 앵글
                pivotAngle -= (mouseYInput * pivotSpeed) * delta; // 상하 앵글
                pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

                Vector3 rotation = Vector3.zero; // Euler Angle 값을 저장할 변수 생성
                rotation.y = lookAngle; // Euler Angle의 y 값에 마우스 좌우 회전값을 대입해준다.
                Quaternion targetRotation = Quaternion.Euler(rotation); // 목표 회전값(Quaternion)
                myTransform.rotation = targetRotation; // 카메라 회전값을 목표 회전값으로 만듬

                rotation = Vector3.zero;
                rotation.x = pivotAngle;
                targetRotation = Quaternion.Euler(rotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
            // 록온을 했을 경우
            else {
                // 카메라가 록온한 대상을 정면으로 바라보며 수평회전하도록 한다.
                Vector3 dir = player.currentTarget.transform.position - transform.position;
                dir.Normalize();
                dir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(dir); // 대상을 바라보게끔 Quaternion 설정
                transform.rotation = targetRotation;

                // transform.rotation은 y축 회전값의 변화만 생김
                // 카메라 피벗의 회전값을 위 회전값과 똑같이 맞춰줌
                // 카메라 피벗의 회전값을 설정해주지 않으면, 록온 직전의 카메라 수직 회전값에 따라 록온 시점이 변함
                cameraPivotTransform.rotation = targetRotation;

                //dir = currentLockOnTarget.position - cameraPivotTransform.position;
                //dir.Normalize();
                //targetRotation = Quaternion.LookRotation(dir);
                //Vector3 eulerAngle = targetRotation.eulerAngles;
                //eulerAngle.y = 0;
                //cameraPivotTransform.localEulerAngles = eulerAngle;
            }
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
            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), obstacleLayer)) {
                // 플레이어의 좌표로부터 카메라의 방향으로 ray를 발사하여 카메라를 제외한 무언가와 충돌했을 경우 충돌한 좌표까지의 거리
                float dist = Vector3.Distance(cameraPivotTransform.position, hit.point);
                //Debug.Log(hit.transform.gameObject.name);
                // 카메라가 이동해야할 z 좌표를 설정한다.
                // 카메라는 pivot보다 항상 뒤에(z 좌표가 음수)있어야 하므로 음수 값이 되어야 한다.
                targetPosition = -(dist - cameraCollisionOffset);
            }

            // 물체와 플레이어 사이의 거리가 너무 가깝다면 최소치로 맞춰줌
            if (Mathf.Abs(targetPosition) < minimumCollisionOffset) {
                targetPosition = -minimumCollisionOffset;
            }

            //Debug.Log("카메라 이동거리" + targetPosition);
            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }

        public void HandleLockOn() {

            float shortestDistance = Mathf.Infinity;
            // 자기 자신을 원점으로 왼쪽은 -무한대 오른쪽은 +무한대
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, maximumLockOnDistance);

            // 감지한 Collider들로부터 CharacterManager 스크립트를 가져온다.
            for (int i = 0; i < colliders.Length; i++) {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();

                // 해당 스크립트가 존재한다면
                if (character != null) {
                    Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);

                    // 록온을 했을시의 시야각. 화면 밖에있는 대상은 록온이 안되도록한다.
                    float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);

                    RaycastHit hit;
                    // 자기 자신에게는 록온이 안되도록 한다.
                    if (character.transform.root != targetTransform.transform.root && viewableAngle > -50 && viewableAngle < 50 && distanceFromTarget <= maximumLockOnDistance) {
                        Vector3 startPosition = targetTransform.GetComponentInChildren<LockOnTransform>().transform.position;
                        Vector3 endPosition = character.GetComponentInChildren<LockOnTransform>().transform.position;
                        Vector3 direction = endPosition - startPosition;

                        if (Physics.Linecast(startPosition, endPosition, out hit, obstacleLayer)) return;
                        availableTargets.Add(character);
                    }
                }
            }

            for (int i = 0; i < availableTargets.Count; i++) {
                float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[i].transform.position);
                if (distanceFromTarget < shortestDistance) {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[i];
                }
                if (player.playerNetworkManager.isLockedOn.Value) { // 록온 상태
                    // InverseTransformPoint : 객체의 월드좌표를 로컬좌표로 변환
                    // 주변에 록온이 가능한 타겟들의 월드 좌표를 자기 자신의 로컬좌표계로 편입시킨다.
                    Vector3 relativeEnemyPosition = inputHandler.transform.InverseTransformPoint(availableTargets[i].transform.position);
                    var distanceFromLeftTarget = relativeEnemyPosition.x; // 왼쪽 타겟의 좌표
                    var distanceFromRightTarget = relativeEnemyPosition.x; // 오른쪽 타겟의 좌표

                    // 만약 x 좌표가 음수라면 자신을 기준으로 왼쪽에 있는 것
                    // 왼쪽에 있는 타겟들은 x 좌표의 값이 - 이므로 멀수록 값이 작아지고 가까울수록 값이 커짐
                    // 해당 좌표의 타겟이 현재 록온된 타겟이 아니라면
                    if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget && availableTargets[i] != player.currentTarget) {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        // 현재 록온된 오브젝트와 가장 가까운 거리를 가진 왼쪽 오브젝트를 저장
                        leftLockTarget = availableTargets[i];
                    }
                    // x 좌표가 양수라면 자신을 기준으로 오른쪽에 있는 것
                    // 오른쪽에 있는 타겟들은 x 좌표의 값이 + 이므로 멀수록 값이 커지고 가까울수록 값이 작아짐
                    // 해당 좌표의 타겟이 현재 록온된 타겟이 아니라면
                    else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget && availableTargets[i] != player.currentTarget) {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        // 현재 록온된 오브젝트와 가장 가까운 거리를 가진 오른쪽 오브젝트를 저장
                        rightLockTarget = availableTargets[i];
                    }
                }
            }
        }

        public void ClearLockOnTargets() {
            availableTargets.Clear();
            nearestLockOnTarget = null;
            player.currentTarget = null;
            player.playerNetworkManager.currentTargetID.Value = 0;
        }

        // 록온 여부에따라 카메라의 높낮이를 바꾼다.
        public void SetCameraHeight() {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);
            if (player.currentTarget != null) {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
            } else {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
        }

        public void ZoomForInteraction(float delta) {
            if (player.isInConversation) {
                //Debug.Log("줌 인");
                cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, cameraTransform.localPosition.z + 5, delta);
                cameraTransform.localPosition = cameraTransformPosition;
            } else {
                //Debug.Log("줌 아웃");
                cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, defaultPosition, delta);
                cameraTransform.localPosition = cameraTransformPosition;
            }
        }
    }
}

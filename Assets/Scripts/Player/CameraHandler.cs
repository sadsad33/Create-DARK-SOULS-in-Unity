using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class CameraHandler : MonoBehaviour {
        // ī�޶� ������ Ÿ���� ��ġ
        public Transform targetTransform;
        // ī�޶��� ��ġ
        public Transform cameraTransform;
        // ī�޶��� ȸ�� �߽� ��ġ
        public Transform cameraPivotTransform;
        public LayerMask ignoreLayer, environmentLayer;

        private Transform myTransform; // CameraHolder�� Transform (= Player�� Transform)
        private Vector3 cameraTransformPosition;
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
        public float lockedPivotPosition = 2.25f;
        public float unlockedPivotPosition = 1.65f;

        public Transform currentLockOnTarget;
        List<CharacterManager> availableTargets = new List<CharacterManager>();
        public Transform nearestLockOnTarget;
        public float maximumLockOnDistance = 30f;
        public Transform leftLockTarget, rightLockTarget;

        InputHandler inputHandler;
        PlayerManager playerManager;
        private void Awake() {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayer = ~(1 << 8 | 1 << 9 | 1 << 10);
            targetTransform = FindObjectOfType<PlayerManager>().transform;
            inputHandler = FindObjectOfType<InputHandler>();
            playerManager = FindObjectOfType<PlayerManager>();
        }

        private void Start() {
            environmentLayer = LayerMask.NameToLayer("Environment");
        }

        // ī�޶� ����� ���󰡵��� �ϴ� �Լ�
        public void FollowTarget(float delta) {

            // ��ǥ �������� �ε巴�� �̵��Ѵ�
            //Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);

            Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
            myTransform.position = targetPosition;

            HandleCameraCollision(delta);

            //Debug.Log("�÷��̾� Position : " + targetTransform.position);
            //Debug.Log("ī�޶� PivotPosition : " + cameraPivotTransform.position);
        }

        /*
         * ī�޶� ȸ��(���� ��ǥ���ε�?)
         * @delta - FixeddeltaTime
         * @mousXInput - ���콺 �¿� ȸ����
         * @mouseYInput - ���콺 ���� ȸ����
         */
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput) {
            // �Ͽ��� ���� �ʾ��� ���
            if (!inputHandler.lockOnFlag && currentLockOnTarget == null) {
                // �ð��� ���� ������ ȸ���� ��ȭ���� ����
                lookAngle += (mouseXInput * lookSpeed) / delta; // �¿� �ޱ�
                pivotAngle -= (mouseYInput * pivotSpeed) / delta; // ���� �ޱ�
                pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

                Vector3 rotation = Vector3.zero; // Euler Angle ���� ������ ���� ����
                rotation.y = lookAngle; // Euler Angle�� y ���� ���콺 �¿� ȸ������ �������ش�.
                Quaternion targetRotation = Quaternion.Euler(rotation); // ��ǥ ȸ����(Quaternion)
                myTransform.rotation = targetRotation; // ī�޶� ȸ������ ��ǥ ȸ�������� ����

                rotation = Vector3.zero;
                rotation.x = pivotAngle;
                targetRotation = Quaternion.Euler(rotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
            // �Ͽ��� ���� ���
            else {
                // ī�޶� �Ͽ��� ����� �������� �ٶ󺸸� ����ȸ���ϵ��� �Ѵ�.
                Vector3 dir = currentLockOnTarget.position - transform.position;
                dir.Normalize();
                dir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(dir); // ����� �ٶ󺸰Բ� Quaternion ����
                transform.rotation = targetRotation;

                // transform.rotation�� y�� ȸ������ ��ȭ�� ����
                // ī�޶� �ǹ��� ȸ������ �� ȸ������ �Ȱ��� ������
                // ī�޶� �ǹ��� ȸ������ ���������� ������, �Ͽ� ������ ī�޶� ���� ȸ������ ���� �Ͽ� ������ ����
                cameraPivotTransform.rotation = targetRotation;

                //dir = currentLockOnTarget.position - cameraPivotTransform.position;
                //dir.Normalize();
                //targetRotation = Quaternion.LookRotation(dir);
                //Vector3 eulerAngle = targetRotation.eulerAngles;
                //eulerAngle.y = 0;
                //cameraPivotTransform.localEulerAngles = eulerAngle;
            }
        }

        // ī�޶� �ٸ� ��ü�� �浹�� ��� �ذ��ϴ� �Լ�
        private void HandleCameraCollision(float delta) {
            targetPosition = defaultPosition;
            RaycastHit hit;
            // ī�޶�� �÷��̾� ������ ȸ���Ѵ�.
            // cameraPivotTransform �� ī�޶� ȸ���� �߽�����, ��ǥ�� �÷��̾��� Transform.position �� ����.
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position; // �÷��̾��� ��ǥ�κ��� ī�޶������ ����
            direction.Normalize();

            //Debug.DrawRay(cameraPivotTransform.position, direction, Color.magenta);

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayer)) {
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

            //Debug.Log("ī�޶� �̵��Ÿ�" + targetPosition);
            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }

        public void HandleLockOn() {

            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

            // ������ Collider��κ��� CharacterManager ��ũ��Ʈ�� �����´�.
            for (int i = 0; i < colliders.Length; i++) {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();

                // �ش� ��ũ��Ʈ�� �����Ѵٸ�
                if (character != null) {
                    Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);

                    // �Ͽ��� �������� �þ߰�. ȭ�� �ۿ��ִ� ����� �Ͽ��� �ȵǵ����Ѵ�.
                    float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);

                    RaycastHit hit;
                    // �ڱ� �ڽſ��Դ� �Ͽ��� �ȵǵ��� �Ѵ�.
                    if (character.transform.root != targetTransform.transform.root && viewableAngle > -50 && viewableAngle < 50 && distanceFromTarget <= maximumLockOnDistance) {
                        // �� ���� ���̿� ���̸� ���.
                        if (Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position, out hit)) {
                            Debug.DrawLine(playerManager.lockOnTransform.position, character.lockOnTransform.position);
                            if (hit.transform.gameObject.layer == environmentLayer) {
                                // ���� �÷��̾� ���̿� ��ֹ��� ������� �Ͽ��� �ȵǵ��� ��
                            } else {
                                availableTargets.Add(character);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < availableTargets.Count; i++) {
                float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[i].transform.position);
                if (distanceFromTarget < shortestDistance) {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[i].lockOnTransform;
                }
                if (inputHandler.lockOnFlag) { // �Ͽ� ����
                    // ���� �÷��̾ �Ͽ��� ���¿��� ���ֺ��� �ִٴ� ��Ȳ�� �����ѵ�
                    // InverseTransformPoint : ��ü�� ������ǥ�� ������ǥ�� ��ȯ
                    // ���� �Ͽ� �Ǿ��ִ� ������Ʈ�� ������ǥ�� ������ �ٸ� �Ͽ� ������ ������Ʈ���� �����ǥ�� ���Ѵ�.
                    Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(availableTargets[i].transform.position);
                    // ���� �ϿµǾ��ִ� ������Ʈ�� �ٸ� ������Ʈ ������ x�� �Ÿ�
                    var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - availableTargets[i].transform.position.x;
                    var distanceFromRightTarget = currentLockOnTarget.transform.position.x + availableTargets[i].transform.position.x;

                    // ��� ��ǥ�� x ��ǥ�� ������ �Ͽµ� ������Ʈ�� �������� ������ : �÷��̾� �������� ����
                    if (relativeEnemyPosition.x > 0.00 && distanceFromLeftTarget < shortestDistanceOfLeftTarget) {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        // ���� �Ͽµ� ������Ʈ�� ���� ����� �Ÿ��� ���� ���� ������Ʈ�� ����
                        leftLockTarget = availableTargets[i].lockOnTransform;
                    }

                    // ��� ��ǥ�� x ��ǥ�� ������� �Ͽµ� ������Ʈ�� �������� ���ʿ� : �÷��̾ �������� ������
                    if (relativeEnemyPosition.x < 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget) {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        // ���� �Ͽµ� ������Ʈ�� ���� ����� �Ÿ��� ���� ������ ������Ʈ�� ����
                        rightLockTarget = availableTargets[i].lockOnTransform;
                    }
                }
            }
        }
        public void ClearLockOnTargets() {
            availableTargets.Clear();
            nearestLockOnTarget = null;
            currentLockOnTarget = null;
        }

        // �Ͽ� ���ο����� ī�޶��� �����̸� �ٲ۴�.
        public void SetCameraHeight() {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);
            if (currentLockOnTarget != null) {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
            } else {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
        }
    }
}
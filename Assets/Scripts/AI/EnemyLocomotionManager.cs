using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyLocomotionManager : MonoBehaviour {
        EnemyManager enemyManager;
        public CharacterStats currentTarget;
        public LayerMask detectionLayer;
        private void Awake() {
            enemyManager = GetComponent<EnemyManager>();
        }
        // �ֺ� ������Ʈ�� ����
        public void HandleDetection() {
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);
            for (int i = 0; i < colliders.Length; i++) {
                // ������ �ֺ� collider�κ��� CharacterStats�� �����´�.
                CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();
                
                // �ش� ������Ʈ�� CharacterStats�� �����Ѵٸ�
                if (characterStats != null) {
                    Vector3 targetDirection = characterStats.transform.position - transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                    // ����� ��ǥ�� ������ ������ �ּ� �þ߰��� �ִ� �þ߰� ���� ������ �ִٸ�
                    if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle) {
                        currentTarget = characterStats; // Ÿ���� �����Ѵ�.
                        
                    }
                }
            }
        }
    }
}
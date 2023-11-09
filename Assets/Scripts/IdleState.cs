using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class IdleState : State {
        public LayerMask detectionLayer;
        public PursueTargetState pursueTargetState;
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {

            #region Handle Enemy Target Detection
            // �ֺ� ������Ʈ�� ����
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
                        enemyManager.currentTarget = characterStats; // Ÿ���� �����Ѵ�.
                    }
                }
            }
            #endregion

            #region Handle Switching to Next State
            if (enemyManager.currentTarget != null) {
                return pursueTargetState;
            } else {
                return this;
            }
            #endregion
        }
    }
}
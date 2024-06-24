using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class IdleState : State {
        public LayerMask detectionLayer;
        public PursueTargetState pursueTargetState;
        public DeadState deadState;
        public override State Tick(EnemyManager enemyManager, EnemyStatsManager enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            if (enemyStats.isDead) return deadState;
            // ��ǥ Ž��
            // ��ǥ Ž���� �����ϸ� Pursue Target State�� ��
            // ��ǥ Ž���� �����ϸ� Idle State ����

            #region ��ǥ�� Ž��
            // �ֺ� ������Ʈ�� ����
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);
            for (int i = 0; i < colliders.Length; i++) {
                // ������ �ֺ� collider�κ��� CharacterStats�� �����´�.
                CharacterStatsManager characterStats = colliders[i].transform.GetComponent<CharacterStatsManager>();

                // �ش� ������Ʈ�� CharacterStats�� �����Ѵٸ�
                if (characterStats != null && characterStats.teamIDNumber != enemyStats.teamIDNumber) {
                    Vector3 targetDirection = characterStats.transform.position - enemyManager.transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);

                    // ����� ��ǥ�� ������ ������ �ּ� �þ߰��� �ִ� �þ߰� ���� ������ �ִٸ�
                    if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle) {
                        enemyManager.currentTarget = characterStats; // Ÿ���� �����Ѵ�.
                    }
                }
            }
            #endregion

            #region ���� ���� ����
            if (enemyManager.currentTarget != null) {
                return pursueTargetState;
            } else {
                return this;
            }
            #endregion
        }
    }
}
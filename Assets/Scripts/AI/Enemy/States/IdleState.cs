using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class IdleState : State {
        public LayerMask detectionLayer;
        public PursueTargetState pursueTargetState;
        public DeadState deadState;
        public override State Tick(AICharacterManager enemyManager, EnemyStatsManager enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            if (enemyStats.isDead) return deadState;
            // 목표 탐색
            // 목표 탐색에 성공하면 Pursue Target State가 됨
            // 목표 탐색을 실패하면 Idle State 유지

            #region 목표물 탐색
            // 주변 오브젝트들 감지
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);
            for (int i = 0; i < colliders.Length; i++) {
                // 감지한 주변 collider로부터 CharacterStats을 가져온다.
                CharacterManager character = colliders[i].transform.GetComponent<CharacterManager>();

                // 해당 오브젝트에 CharacterStats이 존재한다면
                if (character != null && character.characterStatsManager.teamIDNumber != enemyStats.teamIDNumber) {
                    Vector3 targetDirection = character.transform.position - enemyManager.transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);

                    // 정면과 목표물 사이의 각도가 최소 시야각과 최대 시야각 내의 범위에 있다면
                    if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle) {
                        enemyManager.currentTarget = character; // 타겟을 설정한다.
                    }
                }
            }
            #endregion

            #region 다음 상태 전이
            if (enemyManager.currentTarget != null) {
                return pursueTargetState;
            } else {
                return this;
            }
            #endregion
        }
    }
}
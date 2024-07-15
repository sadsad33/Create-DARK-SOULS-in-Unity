using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CombatStanceState : State {
        public AttackState attackState;
        public EnemyAttackActions[] enemyAttacks;
        public PursueTargetState pursueTargetState;
        public DeadState deadState;

        protected bool randomDestinationSet = false; // animator value , 수직 이동, 수평 이동 값을 조절하기 위한 bool 변수
        protected float verticalMovementValue = 0;
        protected float horizontalMovementValue = 0;

        public override State Tick(EnemyManager enemyManager, EnemyStatsManager enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            if (enemyStats.isDead) return deadState;
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position); // 타겟과의 거리
            enemyManager.anim.SetFloat("Vertical", verticalMovementValue, 0.2f, Time.deltaTime); // 앞,뒤 이동
            enemyManager.anim.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime); // 좌,우 이동
            attackState.hasPerformedAttack = false; // 전투 태세 -> 플레이어가 적정 사거리 내에 있을 경우 공격 선택 -> 공격

            if (enemyManager.isInteracting) { // 행동중이라면 
                enemyManager.anim.SetFloat("Vertical", 0);
                enemyManager.anim.SetFloat("Horizontal", 0);
                return this;
            }

            HandleRotateTowardsTarget(enemyManager);

            if (distanceFromTarget > enemyManager.maximumAggroRadius) return pursueTargetState;

            if (!randomDestinationSet) {
                randomDestinationSet = true;
                DecideCirclingAction(enemyAnimatorManager);
            }

            // 현재 상태를 떠나기 전에 공격을 정한다.
            // 현재 상태에서 공격 상태로 넘어가기 전에 해야할 것은 공격을 선택하는 것
            if (enemyManager.currentRecoveryTime <= 0 && attackState.currentAttack != null) { // 현재 공격이 선택된 상태라면 공격 상태로 전이
                randomDestinationSet = false;
                //Debug.Log(attackState.hasPerformedAttack);
                //Debug.Log(attackState.currentAttack);
                return attackState;
            } else { // 아니라면 공격 선택
                GetNewAttack(enemyManager);
            }

            return this;
        }

        // 목표 방향으로 회전
        private void HandleRotateTowardsTarget(EnemyManager enemyManager) { // 회전 제어
            //Vector3 targetVelocity = enemyManager.enemyRigidbody.velocity;
            //enemyManager.navMeshAgent.enabled = true;
            //enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            //enemyManager.enemyRigidbody.velocity = targetVelocity;
            //enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
            enemyManager.navMeshAgent.enabled = true;
            enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            Quaternion tr = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(enemyManager.transform.rotation, tr, enemyManager.rotationSpeed * Time.deltaTime);
            enemyManager.transform.rotation = targetRotation;
        }

        // 원을 그리며 이동할때 어떤식으로 움직일지 결정
        protected void DecideCirclingAction(EnemyAnimatorManager enemyAnimatorManager) {
            // 원을 그리며 앞으로만 이동
            // 원을 그리며 달림
            // 원을 그리며 걷기 등...
            WalkAroundTarget(enemyAnimatorManager);
        }

        // 타겟을 향해 걸어간다.
        protected void WalkAroundTarget(EnemyAnimatorManager enemyAnimatorManager) {
            // 앞으로 이동하는 모션만 사용하기 위함
            // 뒷걸음질을 구현하고 싶다면 verticalMovementValue 변수값 범위에 음수를 포함시키면 됨
            verticalMovementValue = Random.Range(-1f, 1f);
            if (verticalMovementValue < 0f) verticalMovementValue = 0f;
            else verticalMovementValue = 0.5f;

            horizontalMovementValue = Random.Range(-1, 1);
            if (horizontalMovementValue <= 1 && horizontalMovementValue >= 0) {
                horizontalMovementValue = 0.5f;
            } else if (horizontalMovementValue >= -1 && horizontalMovementValue < 0) {
                horizontalMovementValue = -0.5f;
            }
        }

        // 공격 선택
        // 거리, 각도 판단
        // 각 공격들의 점수를 설정할때, 어떤 상황에라도 가능한 공격일수록 높은 점수로 설정하는 것이 좋을듯
        protected virtual void GetNewAttack(EnemyManager enemyManager) {
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);

            int maxScore = 0;

            // Enemy가 수행할수 있는 모든 공격을 순회
            for (int i = 0; i < enemyAttacks.Length; i++) {
                EnemyAttackActions enemyAttackAction = enemyAttacks[i];
                
                // 공격에따라 사거리, 각도, 점수가 다르다고 가정
                // 특정 공격이 가능한 상황이라면 점수를 누적
                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                    distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) { // 현재 목표가 공격 사거리 내에 있고
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) { // 공격 가능한 시야각내에 있다면
                        maxScore += enemyAttackAction.attackScore;
                    }
                }
            }

            // 위에서 구한 누적 점수와 0 사이 임의의 값을 추출
            int randomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;
            for (int i = 0; i < enemyAttacks.Length; i++) {
                EnemyAttackActions enemyAttackAction = enemyAttacks[i];

                // 특정 공격이 가능한 상황이라면 점수를 누적
                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                    distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) {
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) {
                        if (attackState.currentAttack != null) return; // 이미 공격중이라면
                        temporaryScore += enemyAttackAction.attackScore;

                        // 현재 검사하는 공격의 점수를 합산했을때, randomValue 보다 크다면 해당 공격을 선택
                        if (temporaryScore > randomValue) {
                            attackState.currentAttack = enemyAttackAction;
                        }
                    }
                }
            }
        }
    }
}

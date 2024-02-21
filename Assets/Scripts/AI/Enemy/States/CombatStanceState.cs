using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CombatStanceState : State {
        public AttackState attackState;
        public EnemyAttackActions[] enemyAttacks;
        public PursueTargetState pursueTargetState;

        protected bool randomDestinationSet = false; // animator value , 수직 이동, 수평 이동 값을 조절하기 위한 bool 변수
        protected float verticalMovementValue = 0;
        protected float horizontalMovementValue = 0;

        public override State Tick(EnemyManager enemyManager, EnemyStatsManager enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position); // 타겟과의 거리
            enemyAnimatorManager.anim.SetFloat("Vertical", verticalMovementValue, 0.2f, Time.deltaTime); // 앞,뒤 이동
            enemyAnimatorManager.anim.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime); // 좌,우 이동
            attackState.hasPerformedAttack = false; // 전투 태세 -> 플레이어가 적정 사거리 내에 있을 경우 공격 선택 -> 공격

            if (enemyManager.isInteracting) {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0);
                enemyAnimatorManager.anim.SetFloat("Horizontal", 0);
                return this;
            }

            if (distanceFromTarget > enemyManager.maximumAggroRadius) return pursueTargetState;

            if (!randomDestinationSet) {
                randomDestinationSet = true;
                DecideCirclingAction(enemyAnimatorManager);
            }

            HandleRotateTowardsTarget(enemyManager);

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

        protected void HandleRotateTowardsTarget(EnemyManager enemyManager) {
            // 특정 행동을 하고있다면 단순히 대상을 바라보도록 회전
            if (enemyManager.isPerformingAction) {
                //Debug.Log("일반 회전");
                Vector3 direction = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero) // 타겟이 없을경우 정면을 바라봄
                    direction = enemyManager.transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
            } else {
                // NavMeshAgent를 이용한 회전
                Vector3 targetVelocity = enemyManager.enemyRigidbody.velocity;
                enemyManager.navMeshAgent.enabled = true;
                enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
                enemyManager.enemyRigidbody.velocity = targetVelocity;
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
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
            // 뒷걸음질을 구현하고 싶다면 범위에 음수를 포함시키면 됨
            //verticalMovementValue = Random.Range(0, 1);

            //if (verticalMovementValue <= 1 && verticalMovementValue > 0) {
            //    verticalMovementValue = 0.5f;
            //} else if (verticalMovementValue >= -1 && verticalMovementValue < 0) {
            //    verticalMovementValue = -0.5f;
            //}

            verticalMovementValue = 0.5f;

            horizontalMovementValue = Random.Range(-1, 1);
            if (horizontalMovementValue <= 1 && horizontalMovementValue >= 0) {
                horizontalMovementValue = 0.5f;
            } else if (horizontalMovementValue >= -1 && horizontalMovementValue < 0) {
                horizontalMovementValue = -0.5f;
            }
        }

        // 공격 선택
        // 거리, 각도 판단
        protected virtual void GetNewAttack(EnemyManager enemyManager) {
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);
            
            int maxScore = 0;
            
            for (int i = 0; i < enemyAttacks.Length; i++) {
                EnemyAttackActions enemyAttackAction = enemyAttacks[i];

                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                    distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) { // 현재 목표가 공격 사거리 내에 있고
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) { // 공격 가능한 시야각내에 있다면
                        maxScore += enemyAttackAction.attackScore;
                    }
                }
            }

            int randomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;
            for (int i = 0; i < enemyAttacks.Length; i++) {
                EnemyAttackActions enemyAttackAction = enemyAttacks[i];

                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                    distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) {
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) {
                        if (attackState.currentAttack != null) return; // 이미 공격중이라면
                        temporaryScore += enemyAttackAction.attackScore;

                        if (temporaryScore > randomValue) {
                            attackState.currentAttack = enemyAttackAction;
                            //Debug.Log(attackState.hasPerformedAttack);
                            //Debug.Log(attackState.currentAttack);
                        }
                    }
                }
            }
        }
    }
}

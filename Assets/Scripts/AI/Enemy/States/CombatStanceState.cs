using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CombatStanceState : State {
        public AttackState attackState;
        public EnemyAttackActions[] enemyAttacks;
        public PursueTargetState pursueTargetState;
        public DeadState deadState;

        protected bool randomDestinationSet = false; // animator value , ���� �̵�, ���� �̵� ���� �����ϱ� ���� bool ����
        protected float verticalMovementValue = 0;
        protected float horizontalMovementValue = 0;

        public override State Tick(EnemyManager enemyManager, EnemyStatsManager enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            if (enemyStats.isDead) return deadState;
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position); // Ÿ�ٰ��� �Ÿ�
            enemyManager.anim.SetFloat("Vertical", verticalMovementValue, 0.2f, Time.deltaTime); // ��,�� �̵�
            enemyManager.anim.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime); // ��,�� �̵�
            attackState.hasPerformedAttack = false; // ���� �¼� -> �÷��̾ ���� ��Ÿ� ���� ���� ��� ���� ���� -> ����

            if (enemyManager.isInteracting) { // �ൿ���̶�� 
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

            // ���� ���¸� ������ ���� ������ ���Ѵ�.
            // ���� ���¿��� ���� ���·� �Ѿ�� ���� �ؾ��� ���� ������ �����ϴ� ��
            if (enemyManager.currentRecoveryTime <= 0 && attackState.currentAttack != null) { // ���� ������ ���õ� ���¶�� ���� ���·� ����
                randomDestinationSet = false;
                //Debug.Log(attackState.hasPerformedAttack);
                //Debug.Log(attackState.currentAttack);
                return attackState;
            } else { // �ƴ϶�� ���� ����
                GetNewAttack(enemyManager);
            }

            return this;
        }

        // ��ǥ �������� ȸ��
        private void HandleRotateTowardsTarget(EnemyManager enemyManager) { // ȸ�� ����
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

        // ���� �׸��� �̵��Ҷ� ������� �������� ����
        protected void DecideCirclingAction(EnemyAnimatorManager enemyAnimatorManager) {
            // ���� �׸��� �����θ� �̵�
            // ���� �׸��� �޸�
            // ���� �׸��� �ȱ� ��...
            WalkAroundTarget(enemyAnimatorManager);
        }

        // Ÿ���� ���� �ɾ��.
        protected void WalkAroundTarget(EnemyAnimatorManager enemyAnimatorManager) {
            // ������ �̵��ϴ� ��Ǹ� ����ϱ� ����
            // �ް������� �����ϰ� �ʹٸ� verticalMovementValue ������ ������ ������ ���Խ�Ű�� ��
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

        // ���� ����
        // �Ÿ�, ���� �Ǵ�
        // �� ���ݵ��� ������ �����Ҷ�, � ��Ȳ���� ������ �����ϼ��� ���� ������ �����ϴ� ���� ������
        protected virtual void GetNewAttack(EnemyManager enemyManager) {
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);

            int maxScore = 0;

            // Enemy�� �����Ҽ� �ִ� ��� ������ ��ȸ
            for (int i = 0; i < enemyAttacks.Length; i++) {
                EnemyAttackActions enemyAttackAction = enemyAttacks[i];
                
                // ���ݿ����� ��Ÿ�, ����, ������ �ٸ��ٰ� ����
                // Ư�� ������ ������ ��Ȳ�̶�� ������ ����
                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                    distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) { // ���� ��ǥ�� ���� ��Ÿ� ���� �ְ�
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) { // ���� ������ �þ߰����� �ִٸ�
                        maxScore += enemyAttackAction.attackScore;
                    }
                }
            }

            // ������ ���� ���� ������ 0 ���� ������ ���� ����
            int randomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;
            for (int i = 0; i < enemyAttacks.Length; i++) {
                EnemyAttackActions enemyAttackAction = enemyAttacks[i];

                // Ư�� ������ ������ ��Ȳ�̶�� ������ ����
                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                    distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) {
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) {
                        if (attackState.currentAttack != null) return; // �̹� �������̶��
                        temporaryScore += enemyAttackAction.attackScore;

                        // ���� �˻��ϴ� ������ ������ �ջ�������, randomValue ���� ũ�ٸ� �ش� ������ ����
                        if (temporaryScore > randomValue) {
                            attackState.currentAttack = enemyAttackAction;
                        }
                    }
                }
            }
        }
    }
}

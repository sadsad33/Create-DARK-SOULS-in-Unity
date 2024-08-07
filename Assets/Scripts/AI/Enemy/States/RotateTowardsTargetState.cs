using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class RotateTowardsTargetState : State {
        public CombatStanceState combatStanceState;
        public PursueTargetState pursueTargetState;
        public override State Tick(AICharacterManager enemyManager, EnemyStatsManager enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            enemyManager.animator.SetFloat("Vertical", 0);
            enemyManager.animator.SetFloat("Horizontal", 0);

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;

            //// enemy�� �������� enemy���� �÷��̾� ���� ������ ������ ����Ѵ�. 
            //// 0 ~ 180 , -180 ~ 0 ������ ���� ��ȯ�Ѵ�.
            //float viewableAngle = Vector3.SignedAngle(targetDirection, enemyManager.transform.forward, Vector3.up);

            // ȸ�� ���¿� �����ߴµ� ���� �ൿ�� �� ������ �ʾ��� ��� �帧�� ���⿡ ��Ƶα� ���� 
            if (enemyManager.isInteracting)
                return this;

            ////Debug.Log(viewableAngle);
            //if (viewableAngle >= 100 && viewableAngle <= 180 && !enemyManager.isInteracting) {
            //    enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Behind_Left", true);
            //} else if (viewableAngle <= -100 && viewableAngle >= -180 && !enemyManager.isInteracting) { 
            //    enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Behind_Right", true);
            //} else if (viewableAngle <= -45 && viewableAngle >= -100 && !enemyManager.isInteracting) {
            //    enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Right", true);
            //} else if (viewableAngle >= 45 && viewableAngle <= 100 && !enemyManager.isInteracting) {
            //    enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Left", true);
            //}

            //Debug.DrawRay(enemyManager.transform.position, enemyManager.transform.forward, Color.red, Mathf.Infinity);
            //Debug.DrawRay(enemyManager.transform.position, targetDirection, Color.blue, Mathf.Infinity);

            float angle = Vector3.SignedAngle(enemyManager.transform.forward, targetDirection, Vector3.up);
            //Debug.Log(angle);
            if (angle >= 30 && angle <= 120 && !enemyManager.isInteracting) {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Right", true);
                //Debug.Log("Turn_Right");
            } else if (angle > 120 && angle <= 180 && !enemyManager.isInteracting) {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Behind_Right", true);
                //Debug.Log("Turn_Behind_Right");
            } else if (angle >= -120 && angle <= -30 && !enemyManager.isInteracting) {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Left", true);
                //Debug.Log("Turn_Left");
            } else if (angle > -180 && angle < -120 && !enemyManager.isInteracting) {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Behind_Left", true);
                //Debug.Log("Turn_Behind_Left");
            }
            return combatStanceState;
        }
    }
}

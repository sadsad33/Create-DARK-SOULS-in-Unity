using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class RotateTowardsTargetState : State {
        CombatStanceState combatStanceState;

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            enemyAnimatorManager.anim.SetFloat("Vertical", 0);
            enemyAnimatorManager.anim.SetFloat("Horizontal", 0);

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;

            // enemy의 정면방향과 enemy에서 플레이어 방향 사이의 각도를 계산한다. 
            // 0 ~ 180 , -180 ~ 0 사이의 값을 반환한다.
            float viewableAngle = Vector3.SignedAngle(targetDirection, enemyManager.transform.forward, Vector3.up);

            if (viewableAngle >= 100 && viewableAngle <= 180 && !enemyManager.isInteracting) {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Behind", true);
                return this;
            } else if (viewableAngle < -100 && viewableAngle >= -180 && !enemyManager.isInteracting) {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Behind", true);
                return this;
            } else if (viewableAngle <= -45 && viewableAngle >= -100 && !enemyManager.isInteracting) {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Right", true);
                return this;
            } else if (viewableAngle > 45 && viewableAngle < 100 && !enemyManager.isInteracting) {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Left", true);
                return this;
            }

            return this;
        }
    }
}

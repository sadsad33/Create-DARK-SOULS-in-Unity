using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class RotateTowardsTargetState : State {
        public CombatStanceState combatStanceState;

        public override State Tick(EnemyManager enemyManager, EnemyStatsManager enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            enemyAnimatorManager.anim.SetFloat("Vertical", 0);
            enemyAnimatorManager.anim.SetFloat("Horizontal", 0);

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;

            // enemy�� �������� enemy���� �÷��̾� ���� ������ ������ ����Ѵ�. 
            // 0 ~ 180 , -180 ~ 0 ������ ���� ��ȯ�Ѵ�.
            float viewableAngle = Vector3.SignedAngle(targetDirection, enemyManager.transform.forward, Vector3.up);
            // ȸ�� ���¿� �����ߴµ� ���� ������ �� ������ �ʾ��� ��� �帧�� ���⿡ ��Ƶα� ���� 
            if (enemyManager.isInteracting) 
                return this;
            
            //Debug.Log(viewableAngle);
            if (viewableAngle >= 100 && viewableAngle <= 180 && !enemyManager.isInteracting) {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Behind_Left", true);
                return combatStanceState;
            } else if (viewableAngle <= -100 && viewableAngle >= -180 && !enemyManager.isInteracting) { 
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Behind_Right", true);
                return combatStanceState;
            } else if (viewableAngle <= -45 && viewableAngle >= -100 && !enemyManager.isInteracting) {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Right", true);
                return combatStanceState;
            } else if (viewableAngle >= 30 && viewableAngle <= 100 && !enemyManager.isInteracting) {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Left", true);
                return combatStanceState;
            }

            return combatStanceState;
        }
    }
}

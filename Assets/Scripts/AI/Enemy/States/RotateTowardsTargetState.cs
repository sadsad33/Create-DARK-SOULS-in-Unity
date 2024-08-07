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

            //// enemy의 정면방향과 enemy에서 플레이어 방향 사이의 각도를 계산한다. 
            //// 0 ~ 180 , -180 ~ 0 사이의 값을 반환한다.
            //float viewableAngle = Vector3.SignedAngle(targetDirection, enemyManager.transform.forward, Vector3.up);

            // 회전 상태에 돌입했는데 아직 행동이 다 끝나지 않았을 경우 흐름을 여기에 잡아두기 위함 
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

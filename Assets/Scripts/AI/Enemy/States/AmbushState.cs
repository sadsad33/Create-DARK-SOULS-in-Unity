using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class AmbushState : State {
        public bool isSleeping;
        public float detectionRadius = 2;
        public string sleepAnimation;
        public string wakeAnimation;
        public LayerMask detectionLayer;

        public PursueTargetState pursueTargetState;
        public override State Tick(AICharacterManager enemyManager, AICharacterStatsManager enemyStats, AICharacterAnimatorManager enemyAnimatorManager) {
            if (isSleeping && !enemyManager.isInteracting) {
                enemyAnimatorManager.PlayTargetAnimation(sleepAnimation, true);
            }
            #region Handle Target Detection

            Collider[] colliders = Physics.OverlapSphere(enemyManager.transform.position, detectionRadius, detectionLayer);

            for (int i = 0; i < colliders.Length; i++) {
                CharacterManager character = colliders[i].transform.GetComponent<CharacterManager>();
                if (character != null) {
                    Vector3 targetDirection = character.transform.position - enemyManager.transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);
                    if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle) {
                        enemyManager.currentTarget = character;
                        isSleeping = false;
                        enemyAnimatorManager.PlayTargetAnimation(wakeAnimation, true);
                    }
                }
            }
            #endregion

            #region Handle State Change
            if (enemyManager.currentTarget != null)
                return pursueTargetState;
            return this;
            #endregion
        }
    }
}

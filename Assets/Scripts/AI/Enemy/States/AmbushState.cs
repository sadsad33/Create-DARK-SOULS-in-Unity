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
        public override State Tick(AICharacterManager aiManager) {
            if (isSleeping && !aiManager.isInteracting) {
                aiManager.aiAnimatorManager.PlayTargetAnimation(sleepAnimation, true);
            }
            #region Handle Target Detection

            Collider[] colliders = Physics.OverlapSphere(aiManager.transform.position, detectionRadius, detectionLayer);

            for (int i = 0; i < colliders.Length; i++) {
                CharacterManager character = colliders[i].transform.GetComponent<CharacterManager>();
                if (character != null) {
                    Vector3 targetDirection = character.transform.position - aiManager.transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, aiManager.transform.forward);
                    if (viewableAngle > aiManager.minimumDetectionAngle && viewableAngle < aiManager.maximumDetectionAngle) {
                        aiManager.currentTarget = character;
                        isSleeping = false;
                        aiManager.aiAnimatorManager.PlayTargetAnimation(wakeAnimation, true);
                    }
                }
            }
            #endregion

            #region Handle State Change
            if (aiManager.currentTarget != null)
                return pursueTargetState;
            return this;
            #endregion
        }
    }
}

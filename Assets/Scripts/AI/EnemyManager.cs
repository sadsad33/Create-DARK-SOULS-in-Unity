using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyManager : CharacterManager {
        EnemyLocomotionManager enemyLocomotionManager;
        bool isPerformingAction;
        [Header("AI Settings")]
        public float detectionRadius = 20;
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;

        private void Awake() {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
        }

        private void Update() {
            HandleCurrentAction();  
        }

        private void HandleCurrentAction() {
            if (enemyLocomotionManager.currentTarget == null) {
                enemyLocomotionManager.HandleDetection();
            }
        }
    }
}

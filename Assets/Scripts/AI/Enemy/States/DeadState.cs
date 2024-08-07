using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class DeadState : State {
        public override State Tick(AICharacterManager enemyManager, EnemyStatsManager enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            //enemyManager.enemyRigidbody.isKinematic = false;
            //enemyManager.enemyRigidbody.useGravity = false;
            //enemyStats.enemyLocomotionManager.characterCollider.enabled = false;
            enemyManager.navMeshAgent.enabled = false;
            return this;
        }
    }
}

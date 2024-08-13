using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class DeadState : State {
        public override State Tick(AICharacterManager aiManager) {
            //enemyManager.enemyRigidbody.isKinematic = false;
            //enemyManager.enemyRigidbody.useGravity = false;
            //enemyStats.enemyLocomotionManager.characterCollider.enabled = false;
            aiManager.navMeshAgent.enabled = false;
            return this;
        }
    }
}

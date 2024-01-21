using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace sg {
    public class EnemyLocomotionManager : MonoBehaviour {
        EnemyManager enemyManager;
        EnemyAnimatorManager enemyAnimatorManager;
        public LayerMask detectionLayer;
        public CapsuleCollider characterCollider;
        public CapsuleCollider characterColliderBlocker;
        private void Awake() {
            enemyManager = GetComponent<EnemyManager>();
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
        }

        private void Start() {
            Physics.IgnoreCollision(characterCollider, characterColliderBlocker, true);
        }
    }
}
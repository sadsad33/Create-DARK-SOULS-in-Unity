using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace sg {
    public class EnemyLocomotionManager : MonoBehaviour {
        public LayerMask detectionLayer;
        public CapsuleCollider characterCollider;
        public CapsuleCollider characterColliderBlocker;
        private void Awake() {
        }

        private void Start() {
            Physics.IgnoreCollision(characterCollider, characterColliderBlocker, true);
        }
    }
}
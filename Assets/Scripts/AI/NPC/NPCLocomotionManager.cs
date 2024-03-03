using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCLocomotionManager : MonoBehaviour {
        public CapsuleCollider characterColliderBlocker;
        public CapsuleCollider npcCharacterCollider;

        private void Start() {
            Physics.IgnoreCollision(npcCharacterCollider, characterColliderBlocker, true);
        }
    }
}
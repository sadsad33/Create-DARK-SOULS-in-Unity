using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCLocomotionManager : CharacterLocomotionManager {
        public CapsuleCollider characterColliderBlocker;
        public CapsuleCollider npcCharacterCollider;

        protected override void Start() {
            base.Start();
            Physics.IgnoreCollision(npcCharacterCollider, characterColliderBlocker, true);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SoulsLike {
    public class AICharacterLocomotionManager : CharacterLocomotionManager {
        #region 리지드 바디를 사용할 경우
        //public CapsuleCollider characterCollider;
        //public CapsuleCollider characterColliderBlocker;
        //protected override void Start() {
        //    base.Start();
        //    Physics.IgnoreCollision(characterCollider, characterColliderBlocker, true);
        //}
        #endregion

        protected override void Awake() {
            base.Awake();
        }

        protected override void Start() {
            base.Start();
        }

        protected override void Update() {
            base.Update();
        }
    }
}
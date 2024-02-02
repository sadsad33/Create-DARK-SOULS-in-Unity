using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class CharacterManager : MonoBehaviour {
        [Header("Lock On Transform")]
        public Transform lockOnTransform;

        [Header("Comabat Colliders")]
        public CriticalDamageCollider backStabCollider;
        public CriticalDamageCollider riposteCollider;

        [Header("Interaction")]
        public bool isInteracting;
        
        [Header("Combat Flags")]
        public bool canBeRiposted;
        public bool canBeParried;
        public bool isParrying;
        public bool isBlocking;
        public bool isInvulnerable;
        public bool canDoCombo;
        public bool isUsingRightHand;
        public bool isUsingLeftHand;

        [Header("Movement Flags")]
        public bool isRotatingWithRootMotion;
        public bool canRotate;
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;
        public bool isTwoHandingWeapon;

        [Header("Spells")]
        public bool isFiringSpell;

        // 데미지는 애니메이션 이벤트로 가해질 것
        public float pendingCriticalDamage;
    }
}

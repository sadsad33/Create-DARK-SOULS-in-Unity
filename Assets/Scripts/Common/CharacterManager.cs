using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
namespace SoulsLike {
    public class CharacterManager : NetworkBehaviour {
        CharacterAnimatorManager characterAnimatorManager;
        public CharacterWeaponSlotManager characterWeaponSlotManager;
        [Header("Lock On Transform")]
        public Transform lockOnTransform;

        [Header("Comabat Colliders")]
        public CriticalDamageCollider backStabCollider;
        public CriticalDamageCollider riposteCollider;

        [Header("Interaction")]
        public bool canTalk;
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
        public bool isGrabbed;

        [Header("Movement Flags")]
        public bool isRotatingWithRootMotion;
        public bool canRotate;
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;
        public bool isTwoHandingWeapon;
        public bool isClimbing;
        public bool isMoving;

        [Header("Spells")]
        public bool isFiringSpell;

        // �������� �ִϸ��̼� �̺�Ʈ�� ������ ��
        public float pendingCriticalDamage;

        protected virtual void Awake() {
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterWeaponSlotManager = GetComponent<CharacterWeaponSlotManager>();
        }

        protected virtual void FixedUpdate() {
            characterAnimatorManager.CheckHandIKWeight(characterWeaponSlotManager.rightHandIKTarget, characterWeaponSlotManager.leftHandIKTarget, isTwoHandingWeapon);
        }
    }
}

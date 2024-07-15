using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
namespace SoulsLike {
    public class CharacterManager : NetworkBehaviour {
        public Animator anim;
        CharacterAnimatorManager characterAnimatorManager;
        public CharacterWeaponSlotManager characterWeaponSlotManager;
        public CharacterNetworkManager characterNetworkManager;
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
            anim = GetComponent<Animator>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterWeaponSlotManager = GetComponent<CharacterWeaponSlotManager>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
        }

        protected virtual void Update() {
            // Ŭ���̾�Ʈ�� �� ������Ʈ�� �����̶��
            if (IsOwner) {
                // ��Ʈ��ũ�� Ŭ���̾�Ʈ�� ������Ʈ ��ǥ�� ȸ������ ����
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            } else { // Ŭ���̾�Ʈ�� �� ������Ʈ�� ������ �ƴ϶�� ��ǥ�� ȸ������ �޾ƿ� ����
                transform.position = Vector3.SmoothDamp(transform.position, 
                    characterNetworkManager.networkPosition.Value, 
                    ref characterNetworkManager.networkVelocity, 
                    characterNetworkManager.networkPositionSmoothTime);

                transform.rotation = Quaternion.Slerp(transform.rotation, characterNetworkManager.networkRotation.Value, characterNetworkManager.networkRotationSmoothTime);
            }
        }
        protected virtual void FixedUpdate() {
            characterAnimatorManager.CheckHandIKWeight(characterWeaponSlotManager.rightHandIKTarget, characterWeaponSlotManager.leftHandIKTarget, isTwoHandingWeapon);
        }
    }
}

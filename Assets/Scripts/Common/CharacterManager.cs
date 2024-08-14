using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
namespace SoulsLike {
    public class CharacterManager : NetworkBehaviour {
        public CharacterController characterController;
        public Animator animator;
        public CharacterAnimatorManager characterAnimatorManager;
        public CharacterWeaponSlotManager characterWeaponSlotManager;
        public CharacterStatsManager characterStatsManager;
        public CharacterInventoryManager characterInventoryManager;
        public CharacterEffectsManager characterEffectsManager;
        public CharacterNetworkManager characterNetworkManager;
        public CharacterSoundEffectsManager characterSoundEffectsManager;

        [Header("Lock On Transform")]
        public Transform lockOnTransform;

        [Header("Comabat Colliders")]
        public CriticalDamageCollider backStabCollider;
        public CriticalDamageCollider riposteCollider;

        [Header("Interaction")]
        //public bool canTalk;
        public bool isInteracting;
        
        [Header("Combat Flags")]
        public bool canBeRiposted;
        public bool canBeParried;
        public bool isParrying;
        public bool isInvulnerable;
        public bool canDoCombo;
        public bool isGrabbed;

        [Header("Target")]
        public CharacterManager currentTarget;

        [Header("Movement Flags")]
        public bool isRotatingWithRootMotion;
        public bool canRotate;
        public bool isGrounded;
        public bool isClimbing;
        public bool isMoving;
        public bool isJumping = false;

        [Header("Spells")]
        public bool isFiringSpell;

        // 데미지는 애니메이션 이벤트로 가해질 것
        public float pendingCriticalDamage;
        public Collider[] characterColliders;
        protected virtual void Awake() {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterWeaponSlotManager = GetComponent<CharacterWeaponSlotManager>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterInventoryManager = GetComponent<CharacterInventoryManager>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterSoundEffectsManager = GetComponent<CharacterSoundEffectsManager>();
        }

        protected virtual void Start() {
            
        }

        protected virtual void Update() {
            characterEffectsManager.ProcessAllTimedEffects();
            
            // 클라이언트가 이 오브젝트의 주인이라면
            if (IsOwner) {
                // 네트워크에 클라이언트의 오브젝트 좌표와 회전값을 전달
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            } else { // 클라이언트가 이 오브젝트의 주인이 아니라면 좌표와 회전값을 받아와 복사
                transform.position = Vector3.SmoothDamp(transform.position, 
                    characterNetworkManager.networkPosition.Value, 
                    ref characterNetworkManager.networkVelocity, 
                    characterNetworkManager.networkPositionSmoothTime);

                transform.rotation = Quaternion.Slerp(transform.rotation, characterNetworkManager.networkRotation.Value, characterNetworkManager.networkRotationSmoothTime);
            }
        }
        protected virtual void FixedUpdate() {
            characterAnimatorManager.CheckHandIKWeight(characterWeaponSlotManager.rightHandIKTarget, characterWeaponSlotManager.leftHandIKTarget, characterNetworkManager.isTwoHandingWeapon.Value);
        }

        public virtual void UpdateWhichHandCharacterIsUsing(bool usingRightHand) {
            if (IsOwner) {
                if (usingRightHand) {
                    characterNetworkManager.isUsingRightHand.Value = true;
                    characterNetworkManager.isUsingLeftHand.Value = false;
                } else {
                    characterNetworkManager.isUsingRightHand.Value = false;
                    characterNetworkManager.isUsingLeftHand.Value = true;
                }
            }
        }
    }
}

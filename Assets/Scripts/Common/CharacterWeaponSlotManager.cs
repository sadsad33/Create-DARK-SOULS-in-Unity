using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CharacterWeaponSlotManager : MonoBehaviour {
        protected CharacterManager character;
        protected CharacterStatsManager characterStatsManager;
        public CharacterEffectsManager characterEffectsManager;
        protected CharacterAnimatorManager characterAnimatorManager;
        protected CharacterInventoryManager characterInventoryManager;

        [Header("Unarmed Weapon")]
        public WeaponItem unarmedWeapon;

        [Header("Weapon Slots")]
        public WeaponHolderSlot leftHandSlot;
        public WeaponHolderSlot rightHandSlot;
        public WeaponHolderSlot backSlot;

        [Header("Damage Collider")]
        public DamageCollider leftHandDamageCollider;
        public DamageCollider rightHandDamageCollider;

        [Header("Attacking Weapon")]
        public WeaponItem attackingWeapon;

        [Header("Hand IK Targets")]
        public HandIKTarget rightHandIKTarget;
        public HandIKTarget leftHandIKTarget;

        protected virtual void Awake() {
            character = GetComponent<CharacterManager>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterInventoryManager = GetComponent<CharacterInventoryManager>();
            LoadWeaponHolderSlots();
        }

        // 손에 무기가 아닌 무언가를 들고 하는 동작이 끝난후 무기를 다시 로드하기 위한 함수
        public virtual void LoadBothWeaponsOnSlots() {
            LoadWeaponOnSlot(characterInventoryManager.rightWeapon, false);
            LoadWeaponOnSlot(characterInventoryManager.leftWeapon, true);
        }

        // 대상에게 피해를 주기 위해 무기의 DamageCollider를 활성화
        public virtual void OpenDamageCollider() {
            if (character.isUsingRightHand) {
                characterEffectsManager.PlayWeaponFX(false);
                rightHandDamageCollider.EnableDamageCollider();
            } else if (character.isUsingLeftHand) {
                characterEffectsManager.PlayWeaponFX(true);
                leftHandDamageCollider.EnableDamageCollider();
            }
        }

        // 공격 모션중(혹은 끝나고) 무기의 DamageCollider 비활성화
        public virtual void CloseDamageCollider() {
            if (rightHandDamageCollider != null) {
                characterEffectsManager.StopWeaponFX(false);
                rightHandDamageCollider.DisableDamageCollider();
            }
            if (leftHandDamageCollider != null) {
                characterEffectsManager.StopWeaponFX(true);
                leftHandDamageCollider.DisableDamageCollider();
            }
        }

        // 각 손에 Componenet로 추가되어있는 WeaponHolderSlot 스크립트를 참조
        // 만약 왼손의 스크립트라면 왼손에 , 오른손의 스크립트라면 오른손에 할당
        // 할당된 각 손의 WeaponHolderSlot 스크립트는 현재 무기를 로드하기 위해 사용될 것
        protected virtual void LoadWeaponHolderSlots() {
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
                if (weaponSlot.isLeftHandSlot) {
                    leftHandSlot = weaponSlot;
                } else if (weaponSlot.isRightHandSlot) {
                    rightHandSlot = weaponSlot;
                } else if (weaponSlot.isBackSlot) {
                    backSlot = weaponSlot;
                }
            }
        }

        public virtual void LoadSpellOnSlot(SpellItem spellItem) {
        }

        public virtual void LoadConsumableOnSlot(ConsumableItem consumableItem) {
        }

        // 현재 각 손의 WeaponHolderSlot 스크립트를 참조하여 현재 들려있는 무기를 로드
        public virtual void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) {
            if (weaponItem != null) {
                if (isLeft) {
                    leftHandSlot.currentWeapon = weaponItem; // 양잡상태에서 돌아올때를 위해 현재 왼쪽 무기를 기억한다.
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    if (character.IsOwner)
                        character.characterNetworkManager.currentLeftWeaponID.Value = weaponItem.itemID;
                } else {
                    if (character.isTwoHandingWeapon) {
                        // 양잡시 왼쪽손의 무기를 등으로 옮기고, 왼손에 있는 무기는 제거한다.
                        backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
                        leftHandSlot.UnloadWeaponAndDestroy();
                        character.animator.CrossFade(weaponItem.th_idle, 0.2f);
                    } else {
                        character.animator.CrossFade("BothArmsEmpty", 0.2f);
                        backSlot.UnloadWeaponAndDestroy();
                        character.animator.CrossFade(weaponItem.Right_Hand_Idle, 0.2f);
                    }

                    // 양잡을 하던 안하던 오른쪽은 변함없음
                    rightHandSlot.currentWeapon = weaponItem; // 양잡상태에서 돌아올때를 위해 현재 오른쪽 무기를 기억한다.
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                    LoadTwoHandIKTargets(character.isTwoHandingWeapon);
                    if (character.IsOwner)
                        character.characterNetworkManager.currentRightWeaponID.Value = weaponItem.itemID;
                }
            } else {
                weaponItem = unarmedWeapon;
                if (isLeft) {
                    character.animator.CrossFade("LeftArmEmpty", 0.2f);
                    characterInventoryManager.leftWeapon = unarmedWeapon;
                    leftHandSlot.currentWeapon = weaponItem;
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    if (character.IsOwner)
                        character.characterNetworkManager.currentLeftWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;
                } else {
                    character.animator.CrossFade("RightArmEmpty", 0.2f);
                    characterInventoryManager.rightWeapon = unarmedWeapon;
                    rightHandSlot.currentWeapon = weaponItem;
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                    if (character.IsOwner)
                        character.characterNetworkManager.currentRightWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;
                }
            }
        }

        protected virtual void LoadLeftWeaponDamageCollider() {
            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            leftHandDamageCollider.physicalDamage = characterInventoryManager.leftWeapon.physicalDamage;
            leftHandDamageCollider.fireDamage = characterInventoryManager.leftWeapon.fireDamage;
            leftHandDamageCollider.teamIDNumber = characterStatsManager.teamIDNumber;

            leftHandDamageCollider.characterCausingDamage = character;
            // 왼쪽 무기의 DamageCollider에 현재 왼쪽 무기의 강인도 감쇄율을 전달
            leftHandDamageCollider.poiseBreak = characterInventoryManager.leftWeapon.poiseBreak;
            // 현재 왼쪽손에 들려있는 무기 모델의 자식에 있는 WeaponFX 스크립트 파일을 불러옴
            characterEffectsManager.leftWeaponManager = leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponManager>();
        }

        protected virtual void LoadRightWeaponDamageCollider() {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            rightHandDamageCollider.physicalDamage = characterInventoryManager.rightWeapon.physicalDamage;
            rightHandDamageCollider.fireDamage = characterInventoryManager.rightWeapon.fireDamage;
            rightHandDamageCollider.teamIDNumber = characterStatsManager.teamIDNumber;

            rightHandDamageCollider.characterCausingDamage = character;
            // 오른쪽 무기의 DamageCollider에 현재 오른쪽 무기의 강인도 감쇄율을 전달
            rightHandDamageCollider.poiseBreak = characterInventoryManager.rightWeapon.poiseBreak;
            // 현재 오른쪽손에 들려있는 무기 모델의 자식에 있는 WeaponFX 스크립트 파일을 불러옴
            characterEffectsManager.rightWeaponManager = rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponManager>();
        }

        public virtual void GrantWeaponAttackingPoiseBonus() { // (특)대형무기 공격 도중 강인도 보너스 합산
            // 적의 경우 무기를 교체하는 상황이 흔하지 않으므로 공격시 강인도 보너스를 스탯으로 가짐
            characterStatsManager.totalPoiseDefense += attackingWeapon.offensivePoiseBonus;
        }

        public virtual void ResetWeaponAttackingPoiseBonus() { // 공격이 끝나면 강인도 보너스 초기화
            characterStatsManager.totalPoiseDefense = characterStatsManager.armorPoiseBonus;
        }

        public virtual void LoadTwoHandIKTargets(bool isTwoHandingWeapon) {
            // 오른쪽 슬롯에 있는 무기를 양손잡기 할 것이므로 오른손 슬롯만 참조
            HandIKTarget[] handIKTargets = rightHandSlot.currentWeaponModel.GetComponentsInChildren<HandIKTarget>();
            foreach (HandIKTarget handIKTarget in handIKTargets) {
                if (handIKTarget.isLeft) leftHandIKTarget = handIKTarget;
                else rightHandIKTarget = handIKTarget;
            }
            characterAnimatorManager.SetHandIKForWeapon(rightHandIKTarget, leftHandIKTarget, isTwoHandingWeapon);
        }
    }
}
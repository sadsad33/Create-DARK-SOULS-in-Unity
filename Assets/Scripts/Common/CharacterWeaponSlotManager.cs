using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CharacterWeaponSlotManager : MonoBehaviour {
        protected CharacterManager characterManager;
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
            characterManager = GetComponent<CharacterManager>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterInventoryManager = GetComponent<CharacterInventoryManager>();
            LoadWeaponHolderSlots();
        }

        // �տ� ���Ⱑ �ƴ� ���𰡸� ��� �ϴ� ������ ������ ���⸦ �ٽ� �ε��ϱ� ���� �Լ�
        public virtual void LoadBothWeaponsOnSlots() {
            LoadWeaponOnSlot(characterInventoryManager.rightWeapon, false);
            LoadWeaponOnSlot(characterInventoryManager.leftWeapon, true);
        }

        // ��󿡰� ���ظ� �ֱ� ���� ������ DamageCollider�� Ȱ��ȭ
        public virtual void OpenDamageCollider() {
            if (characterManager.isUsingRightHand) {
                characterEffectsManager.PlayWeaponFX(false);
                rightHandDamageCollider.EnableDamageCollider();
            } else if (characterManager.isUsingLeftHand) {
                characterEffectsManager.PlayWeaponFX(true);
                leftHandDamageCollider.EnableDamageCollider();
            }
        }

        // ���� �����(Ȥ�� ������) ������ DamageCollider ��Ȱ��ȭ
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

        // �� �տ� Componenet�� �߰��Ǿ��ִ� WeaponHolderSlot ��ũ��Ʈ�� ����
        // ���� �޼��� ��ũ��Ʈ��� �޼տ� , �������� ��ũ��Ʈ��� �����տ� �Ҵ�
        // �Ҵ�� �� ���� WeaponHolderSlot ��ũ��Ʈ�� ���� ���⸦ �ε��ϱ� ���� ���� ��
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

        // ���� �� ���� WeaponHolderSlot ��ũ��Ʈ�� �����Ͽ� ���� ����ִ� ���⸦ �ε�
        public virtual void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) {
            if (weaponItem != null) {
                if (isLeft) {
                    leftHandSlot.currentWeapon = weaponItem; // ������¿��� ���ƿö��� ���� ���� ���� ���⸦ ����Ѵ�.
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                } else {
                    if (characterManager.isTwoHandingWeapon) {
                        // ����� ���ʼ��� ���⸦ ������ �ű��, �޼տ� �ִ� ����� �����Ѵ�.
                        backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
                        leftHandSlot.UnloadWeaponAndDestroy();
                        characterAnimatorManager.anim.CrossFade(weaponItem.th_idle, 0.2f);
                    } else {
                        characterAnimatorManager.anim.CrossFade("BothArmsEmpty", 0.2f);
                        backSlot.UnloadWeaponAndDestroy();
                        characterAnimatorManager.anim.CrossFade(weaponItem.Right_Hand_Idle, 0.2f);
                    }

                    // ������ �ϴ� ���ϴ� �������� ���Ծ���
                    rightHandSlot.currentWeapon = weaponItem; // ������¿��� ���ƿö��� ���� ���� ������ ���⸦ ����Ѵ�.
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                    LoadTwoHandIKTargets(characterManager.isTwoHandingWeapon);
                }
            } else {
                weaponItem = unarmedWeapon;
                if (isLeft) {
                    characterAnimatorManager.anim.CrossFade("LeftArmEmpty", 0.2f);
                    characterInventoryManager.leftWeapon = unarmedWeapon;
                    leftHandSlot.currentWeapon = weaponItem;
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                } else {
                    characterAnimatorManager.anim.CrossFade("RightArmEmpty", 0.2f);
                    characterInventoryManager.rightWeapon = unarmedWeapon;
                    rightHandSlot.currentWeapon = weaponItem;
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                }
            }
        }

        protected virtual void LoadLeftWeaponDamageCollider() {
            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            leftHandDamageCollider.physicalDamage = characterInventoryManager.leftWeapon.physicalDamage;
            leftHandDamageCollider.fireDamage = characterInventoryManager.leftWeapon.fireDamage;
            leftHandDamageCollider.teamIDNumber = characterStatsManager.teamIDNumber;

            leftHandDamageCollider.characterManager = characterManager;
            // ���� ������ DamageCollider�� ���� ���� ������ ���ε� �������� ����
            leftHandDamageCollider.poiseBreak = characterInventoryManager.leftWeapon.poiseBreak;
            // ���� ���ʼտ� ����ִ� ���� ���� �ڽĿ� �ִ� WeaponFX ��ũ��Ʈ ������ �ҷ���
            characterEffectsManager.leftWeaponFX = leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
        }

        protected virtual void LoadRightWeaponDamageCollider() {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            rightHandDamageCollider.physicalDamage = characterInventoryManager.rightWeapon.physicalDamage;
            rightHandDamageCollider.fireDamage = characterInventoryManager.rightWeapon.fireDamage;
            rightHandDamageCollider.teamIDNumber = characterStatsManager.teamIDNumber;

            rightHandDamageCollider.characterManager = characterManager;
            // ������ ������ DamageCollider�� ���� ������ ������ ���ε� �������� ����
            rightHandDamageCollider.poiseBreak = characterInventoryManager.rightWeapon.poiseBreak;
            // ���� �����ʼտ� ����ִ� ���� ���� �ڽĿ� �ִ� WeaponFX ��ũ��Ʈ ������ �ҷ���
            characterEffectsManager.rightWeaponFX = rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
        }

        public virtual void GrantWeaponAttackingPoiseBonus() { // (Ư)�������� ���� ���� ���ε� ���ʽ� �ջ�
            // ���� ��� ���⸦ ��ü�ϴ� ��Ȳ�� ������ �����Ƿ� ���ݽ� ���ε� ���ʽ��� �������� ����
            characterStatsManager.totalPoiseDefense += attackingWeapon.offensivePoiseBonus;
        }

        public virtual void ResetWeaponAttackingPoiseBonus() { // ������ ������ ���ε� ���ʽ� �ʱ�ȭ
            characterStatsManager.totalPoiseDefense = characterStatsManager.armorPoiseBonus;
        }

        public virtual void LoadTwoHandIKTargets(bool isTwoHandingWeapon) {
            // ������ ���Կ� �ִ� ���⸦ ������ �� ���̹Ƿ� ������ ���Ը� ����
            HandIKTarget[] handIKTargets = rightHandSlot.currentWeaponModel.GetComponentsInChildren<HandIKTarget>();
            foreach (HandIKTarget handIKTarget in handIKTargets) {
                if (handIKTarget.isLeft) leftHandIKTarget = handIKTarget;
                else rightHandIKTarget = handIKTarget;
            }
            characterAnimatorManager.SetHandIKForWeapon(rightHandIKTarget, leftHandIKTarget, isTwoHandingWeapon);
        }
    }
}
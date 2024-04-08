using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerWeaponSlotManager : CharacterWeaponSlotManager {
        PlayerInventoryManager playerInventoryManager;

        Animator animator;
        QuickSlots quickSlots;
        PlayerStatsManager playerStatsManager;
        InputHandler inputHandler;
        PlayerManager playerManager;
        PlayerEffectsManager playerEffectsManager;
        CameraHandler cameraHandler;
        protected override void Awake() {
            base.Awake();
            playerManager = GetComponent<PlayerManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            inputHandler = GetComponent<InputHandler>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();
            animator = GetComponent<Animator>();
            quickSlots = FindObjectOfType<QuickSlots>();
            cameraHandler = FindObjectOfType<CameraHandler>();
        }

        public override void LoadSpellOnSlot(SpellItem spellItem) {
            quickSlots.UpdateCurrentSpellIcon(spellItem);
        }

        public override void LoadConsumableOnSlot(ConsumableItem consumableItem) {
            quickSlots.UpdateCurrentConsumableIcon(consumableItem);
        }

        public override void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) {
            if (weaponItem != null) {
                if (isLeft) {
                    leftHandSlot.currentWeapon = weaponItem; // ������¿��� ���ƿö��� ���� ���� ���� ���⸦ ����Ѵ�.
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    quickSlots.UpdateWeaponQuickSlotsUI(true, weaponItem);
                    animator.CrossFade(weaponItem.Left_Hand_Idle, 0.2f);
                } else {
                    if (inputHandler.twoHandFlag) {
                        // ����� ���ʼ��� ���⸦ ������ �ű��, �޼տ� �ִ� ����� �����Ѵ�.
                        backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
                        leftHandSlot.UnloadWeaponAndDestroy();
                        animator.CrossFade(weaponItem.th_idle, 0.2f);
                    } else {
                        animator.CrossFade("BothArmsEmpty", 0.2f);
                        backSlot.UnloadWeaponAndDestroy();
                        animator.CrossFade(weaponItem.Right_Hand_Idle, 0.2f);
                    }

                    // ������ �ϴ� ���ϴ� �������� ���Ծ���
                    rightHandSlot.currentWeapon = weaponItem; // ������¿��� ���ƿö��� ���� ���� ������ ���⸦ ����Ѵ�.
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                    quickSlots.UpdateWeaponQuickSlotsUI(false, weaponItem);
                }
            } else {
                weaponItem = unarmedWeapon;
                if (isLeft) {
                    animator.CrossFade("LeftArmEmpty", 0.2f);
                    playerInventoryManager.leftWeapon = unarmedWeapon;
                    leftHandSlot.currentWeapon = weaponItem;
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    quickSlots.UpdateWeaponQuickSlotsUI(true, weaponItem);
                } else {
                    animator.CrossFade("RightArmEmpty", 0.2f);
                    playerInventoryManager.rightWeapon = unarmedWeapon;
                    rightHandSlot.currentWeapon = weaponItem;
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                    quickSlots.UpdateWeaponQuickSlotsUI(false, weaponItem);
                }
            }
        }

        public void SuccessfullyThrowFireBomb() {
            Destroy(playerEffectsManager.instantiatedFXModel);
            BombConsumableItem fireBombItem = playerInventoryManager.currentConsumable as BombConsumableItem;
            // Instantiate�޼��忡 �� ī�޶��� ȸ������ �Ѱ��ִ� ������ � ��쿡�� �÷��̾��� ���� ���⿡�� ȭ������ �����Ǿ�� �ϱ� ����
            GameObject activeBombModel = Instantiate(fireBombItem.liveBombModel, rightHandSlot.transform.position, cameraHandler.cameraPivotTransform.rotation);
            // ȭ������ ȸ������ ī�޶��� ȸ������ ���� �÷��̾ �ٶ󺸴� �������� ���ư��� �Ѵ�
            activeBombModel.transform.rotation = Quaternion.Euler(cameraHandler.cameraPivotTransform.eulerAngles.x, playerManager.lockOnTransform.eulerAngles.y, 0);
            
            // ȭ���� �ӵ� ����
            BombDamageCollider damageCollider = activeBombModel.GetComponentInChildren<BombDamageCollider>();
            damageCollider.contactDamage = fireBombItem.baseDamage;
            damageCollider.fireExplosionDamage = fireBombItem.explosiveDamage;
            damageCollider.bombRigidbody.AddForce(activeBombModel.transform.forward * fireBombItem.forwardVelocity);
            damageCollider.bombRigidbody.AddForce(activeBombModel.transform.up * fireBombItem.upwardVelocity);
            damageCollider.teamIDNumber = playerStatsManager.teamIDNumber; // �Ǿƽĺ��� ���� ��ID ����
            // LoadWeaponSlot(playerInventory.rightWeapon, false);
            
        }

        #region Handle Weapon's Stamina Drainage
        public void DrainStaminaLightAttack() {
            playerStatsManager.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier));
        }

        public void DrainStaminaHeavyAttack() {
            playerStatsManager.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier));
        }
        #endregion
    }
}

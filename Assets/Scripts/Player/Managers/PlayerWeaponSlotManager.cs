using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerWeaponSlotManager : CharacterWeaponSlotManager {
        PlayerInventoryManager playerInventoryManager;

        [Header("Attacking Weapon")]
        public WeaponItem attackingWeapon;

        Animator animator;
        QuickSlots quickSlots;
        PlayerStatsManager playerStatsManager;
        InputHandler inputHandler;
        PlayerManager playerManager;
        PlayerEffectsManager playerEffectsManager;
        CameraHandler cameraHandler;
        private void Awake() {
            playerManager = GetComponent<PlayerManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            inputHandler = GetComponent<InputHandler>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();
            animator = GetComponent<Animator>();
            quickSlots = FindObjectOfType<QuickSlots>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            LoadWeaponHolderSlots();
        }

        private void LoadWeaponHolderSlots() {
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>(); // �÷��̾��� �޼հ� �����տ� �ִ� WeaponHolderSlot�� ��� �����´�.
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
                if (weaponSlot.isLeftHandSlot)
                    leftHandSlot = weaponSlot; // ���� �����̶�� ���ʿ�
                else if (weaponSlot.isRightHandSlot)
                    rightHandSlot = weaponSlot; // ������ �����̶�� �����ʿ�
                else if (weaponSlot.isBackSlot) {
                    backSlot = weaponSlot;
                }
            }
        }

        // �տ� ���Ⱑ �ƴ� ���𰡸� ��� �ϴ� ������ ������ ���⸦ �ٽ� �ε��ϱ� ���� �Լ�
        // �ִϸ��̼� �̺�Ʈ�� �߰��ϴ°� ���� ���� ������
        public void LoadBothWeaponsOnSlots() {
            LoadWeaponOnSlot(playerInventoryManager.rightWeapon, false);
            LoadWeaponOnSlot(playerInventoryManager.leftWeapon, true);
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) {
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
                        animator.CrossFade("Both Arms Empty", 0.2f);
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
                    animator.CrossFade("Left Arm Empty", 0.2f);
                    playerInventoryManager.leftWeapon = unarmedWeapon;
                    leftHandSlot.currentWeapon = weaponItem;
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    quickSlots.UpdateWeaponQuickSlotsUI(true, weaponItem);
                } else {
                    animator.CrossFade("Right Arm Empty", 0.2f);
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

        #region Handle Weapon's Damage Collider
        // �ִϸ��̼� ���� event�� ������ �Լ����� ����� ��
        private void LoadLeftWeaponDamageCollider() {
            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            leftHandDamageCollider.physicalDamage = playerInventoryManager.leftWeapon.physicalDamage;
            leftHandDamageCollider.fireDamage = playerInventoryManager.leftWeapon.fireDamage;
            leftHandDamageCollider.teamIDNumber = playerStatsManager.teamIDNumber;
            
            // ���� ������ DamageCollider�� ���� ���� ������ ���ε� �������� ����
            leftHandDamageCollider.poiseBreak = playerInventoryManager.leftWeapon.poiseBreak;
            // ���� ���ʼտ� ����ִ� ���� ���� �ڽĿ� �ִ� WeaponFX ��ũ��Ʈ ������ �ҷ���
            playerEffectsManager.leftWeaponFX = leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
        }

        private void LoadRightWeaponDamageCollider() {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            rightHandDamageCollider.physicalDamage = playerInventoryManager.rightWeapon.physicalDamage;
            rightHandDamageCollider.fireDamage = playerInventoryManager.rightWeapon.fireDamage;
            rightHandDamageCollider.teamIDNumber = playerStatsManager.teamIDNumber;

            // ������ ������ DamageCollider�� ���� ������ ������ ���ε� �������� ����
            rightHandDamageCollider.poiseBreak = playerInventoryManager.rightWeapon.poiseBreak;
            // ���� �����ʼտ� ����ִ� ���� ���� �ڽĿ� �ִ� WeaponFX ��ũ��Ʈ ������ �ҷ���
            playerEffectsManager.rightWeaponFX = rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
        }

        public void OpenDamageCollider() {
            if (playerManager.isUsingRightHand) {
                playerEffectsManager.PlayWeaponFX(false);
                rightHandDamageCollider.EnableDamageCollider();
            } else if (playerManager.isUsingLeftHand) {
                playerEffectsManager.PlayWeaponFX(true);
                leftHandDamageCollider.EnableDamageCollider();
            }
        }

        public void CloseDamageCollider() {
            if (rightHandDamageCollider != null) {
                playerEffectsManager.StopWeaponFX(false);
                rightHandDamageCollider.DisableDamageCollider();
            }
            if (leftHandDamageCollider != null) {
                playerEffectsManager.StopWeaponFX(true);
                leftHandDamageCollider.DisableDamageCollider();
            }
        }

        #endregion

        #region Handle Weapon's Stamina Drainage
        public void DrainStaminaLightAttack() {
            playerStatsManager.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier));
        }

        public void DrainStaminaHeavyAttack() {
            playerStatsManager.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier));
        }
        #endregion

        #region Handle Weapon's Poise Bonus
        public void GrantWeaponAttackingPoiseBonus() { // (Ư)�������� ���� ���� ���ε� ���ʽ� �ջ�
            playerStatsManager.totalPoiseDefense = playerStatsManager.totalPoiseDefense + attackingWeapon.offensivePoiseBonus;
        }

        public void ResetWeaponAttackingPoiseBonus() { // ������ ������ ���ε� ���ʽ� �ʱ�ȭ
            playerStatsManager.totalPoiseDefense = playerStatsManager.armorPoiseBonus;
        }
        #endregion
    }
}

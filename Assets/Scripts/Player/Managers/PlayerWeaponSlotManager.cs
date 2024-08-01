using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerWeaponSlotManager : CharacterWeaponSlotManager {
        PlayerInventoryManager playerInventoryManager;

        Animator animator;
        PlayerStatsManager playerStatsManager;
        InputHandler inputHandler;
        PlayerManager player;
        PlayerEffectsManager playerEffectsManager;
        CameraHandler cameraHandler;
        protected override void Awake() {
            base.Awake();
            player = GetComponent<PlayerManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            inputHandler = GetComponent<InputHandler>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();
            animator = GetComponent<Animator>();
            cameraHandler = FindObjectOfType<CameraHandler>();
        }

        public override void LoadSpellOnSlot(SpellItem spellItem) {
            UIManager.instance.quickSlotsUI.UpdateCurrentSpellIcon(spellItem);
        }

        public override void LoadConsumableOnSlot(ConsumableItem consumableItem) {
            UIManager.instance.quickSlotsUI.UpdateCurrentConsumableIcon(consumableItem);
        }

        public override void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) {
            if (weaponItem != null) {
                if (isLeft) {
                    leftHandSlot.currentWeapon = weaponItem; // 양잡상태에서 돌아올때를 위해 현재 왼쪽 무기를 기억한다.
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    //animator.CrossFade(weaponItem.Left_Hand_Idle, 0.2f);
                    player.playerAnimatorManager.PlayTargetAnimation(weaponItem.Left_Hand_Idle, false, true);
                    if (character.IsOwner) {
                        character.characterNetworkManager.currentLeftWeaponID.Value = weaponItem.itemID;
                        UIManager.instance.quickSlotsUI.UpdateWeaponQuickSlotsUI(true, weaponItem);
                    }
                } else {
                    if (inputHandler.twoHandFlag) {
                        // 양잡시 왼쪽손의 무기를 등으로 옮기고, 왼손에 있는 무기는 제거한다.
                        backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
                        leftHandSlot.UnloadWeaponAndDestroy();
                        player.playerAnimatorManager.PlayTargetAnimation(weaponItem.th_idle, false, true);
                    } else {
                        //animator.CrossFade("BothArmsEmpty", 0.2f);
                        player.playerAnimatorManager.PlayTargetAnimation("BothArmsEmpty", false, true);
                        backSlot.UnloadWeaponAndDestroy();
                        animator.CrossFade(weaponItem.Right_Hand_Idle, 0.2f);
                    }

                    // 양잡을 하던 안하던 오른쪽은 변함없음
                    rightHandSlot.currentWeapon = weaponItem; // 양잡상태에서 돌아올때를 위해 현재 오른쪽 무기를 기억한다.
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                    if (character.IsOwner) {
                        character.characterNetworkManager.currentRightWeaponID.Value = weaponItem.itemID;
                        UIManager.instance.quickSlotsUI.UpdateWeaponQuickSlotsUI(false, weaponItem);
                    }
                }
            } else {
                weaponItem = unarmedWeapon;
                if (isLeft) {
                    //animator.CrossFade("LeftArmEmpty", 0.2f);
                    player.playerAnimatorManager.PlayTargetAnimation("LeftArmEmpty", false, true);
                    playerInventoryManager.leftWeapon = unarmedWeapon;
                    leftHandSlot.currentWeapon = weaponItem;
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    if (character.IsOwner) {
                        character.characterNetworkManager.currentLeftWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;
                        UIManager.instance.quickSlotsUI.UpdateWeaponQuickSlotsUI(true, weaponItem);
                    }
                } else {
                    //animator.CrossFade("RightArmEmpty", 0.2f);
                    player.playerAnimatorManager.PlayTargetAnimation("RightArmEmpty", false, true);
                    playerInventoryManager.rightWeapon = unarmedWeapon;
                    rightHandSlot.currentWeapon = weaponItem;
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                    if (character.IsOwner) {
                        character.characterNetworkManager.currentRightWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;
                        UIManager.instance.quickSlotsUI.UpdateWeaponQuickSlotsUI(false, weaponItem);
                    }
                }
            }
        }

        public void SuccessfullyThrowFireBomb() {
            Destroy(playerEffectsManager.instantiatedFXModel);
            BombConsumableItem fireBombItem = playerInventoryManager.currentConsumable as BombConsumableItem;
            // Instantiate메서드에 에 카메라의 회전값을 넘겨주는 이유는 어떤 경우에라도 플레이어의 정면 방향에서 화염병이 스폰되어야 하기 때문
            GameObject activeBombModel = Instantiate(fireBombItem.liveBombModel, rightHandSlot.transform.position, cameraHandler.cameraPivotTransform.rotation);
            // 화염병의 회전값은 카메라의 회전값과 맞춰 플레이어가 바라보는 방향으로 날아가게 한다
            activeBombModel.transform.rotation = Quaternion.Euler(cameraHandler.cameraPivotTransform.eulerAngles.x, player.lockOnTransform.eulerAngles.y, 0);

            // 화염병 속도 설정
            BombDamageCollider damageCollider = activeBombModel.GetComponentInChildren<BombDamageCollider>();
            damageCollider.characterThrowsThis = player;
            damageCollider.contactDamage = fireBombItem.baseDamage;
            damageCollider.fireExplosionDamage = fireBombItem.explosiveDamage;
            damageCollider.bombRigidbody.AddForce(activeBombModel.transform.forward * fireBombItem.forwardVelocity);
            damageCollider.bombRigidbody.AddForce(activeBombModel.transform.up * fireBombItem.upwardVelocity);
            damageCollider.teamIDNumber = playerStatsManager.teamIDNumber; // 피아식별을 위한 팀ID 설정
                                                                           // LoadWeaponSlot(playerInventory.rightWeapon, false);
        }

        #region Handle Weapon's Stamina Drainage
        public void DrainStaminaLightAttack() {
            if (player.IsOwner)
                playerStatsManager.DeductStamina(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier));
        }

        public void DrainStaminaHeavyAttack() {
            if (player.IsOwner)
                playerStatsManager.DeductStamina(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier));
        }
        #endregion
    }
}

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
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>(); // 플레이어의 왼손과 오른손에 있는 WeaponHolderSlot을 모두 가져온다.
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
                if (weaponSlot.isLeftHandSlot)
                    leftHandSlot = weaponSlot; // 왼쪽 슬롯이라면 왼쪽에
                else if (weaponSlot.isRightHandSlot)
                    rightHandSlot = weaponSlot; // 오른쪽 슬롯이라면 오른쪽에
                else if (weaponSlot.isBackSlot) {
                    backSlot = weaponSlot;
                }
            }
        }

        // 손에 무기가 아닌 무언가를 들고 하는 동작이 끝난후 무기를 다시 로드하기 위한 함수
        // 애니메이션 이벤트로 추가하는게 좀더 보기 좋은듯
        public void LoadBothWeaponsOnSlots() {
            LoadWeaponOnSlot(playerInventoryManager.rightWeapon, false);
            LoadWeaponOnSlot(playerInventoryManager.leftWeapon, true);
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) {
            if (weaponItem != null) {
                if (isLeft) {
                    leftHandSlot.currentWeapon = weaponItem; // 양잡상태에서 돌아올때를 위해 현재 왼쪽 무기를 기억한다.
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    quickSlots.UpdateWeaponQuickSlotsUI(true, weaponItem);
                    animator.CrossFade(weaponItem.Left_Hand_Idle, 0.2f);
                } else {
                    if (inputHandler.twoHandFlag) {
                        // 양잡시 왼쪽손의 무기를 등으로 옮기고, 왼손에 있는 무기는 제거한다.
                        backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
                        leftHandSlot.UnloadWeaponAndDestroy();
                        animator.CrossFade(weaponItem.th_idle, 0.2f);
                    } else {
                        animator.CrossFade("Both Arms Empty", 0.2f);
                        backSlot.UnloadWeaponAndDestroy();
                        animator.CrossFade(weaponItem.Right_Hand_Idle, 0.2f);
                    }

                    // 양잡을 하던 안하던 오른쪽은 변함없음
                    rightHandSlot.currentWeapon = weaponItem; // 양잡상태에서 돌아올때를 위해 현재 오른쪽 무기를 기억한다.
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
            // Instantiate메서드에 에 카메라의 회전값을 넘겨주는 이유는 어떤 경우에라도 플레이어의 정면 방향에서 화염병이 스폰되어야 하기 때문
            GameObject activeBombModel = Instantiate(fireBombItem.liveBombModel, rightHandSlot.transform.position, cameraHandler.cameraPivotTransform.rotation);
            // 화염병의 회전값은 카메라의 회전값과 맞춰 플레이어가 바라보는 방향으로 날아가게 한다
            activeBombModel.transform.rotation = Quaternion.Euler(cameraHandler.cameraPivotTransform.eulerAngles.x, playerManager.lockOnTransform.eulerAngles.y, 0);
            
            // 화염병 속도 설정
            BombDamageCollider damageCollider = activeBombModel.GetComponentInChildren<BombDamageCollider>();
            damageCollider.contactDamage = fireBombItem.baseDamage;
            damageCollider.fireExplosionDamage = fireBombItem.explosiveDamage;
            damageCollider.bombRigidbody.AddForce(activeBombModel.transform.forward * fireBombItem.forwardVelocity);
            damageCollider.bombRigidbody.AddForce(activeBombModel.transform.up * fireBombItem.upwardVelocity);
            damageCollider.teamIDNumber = playerStatsManager.teamIDNumber; // 피아식별을 위한 팀ID 설정
            // LoadWeaponSlot(playerInventory.rightWeapon, false);
            
        }

        #region Handle Weapon's Damage Collider
        // 애니메이션 내의 event로 다음의 함수들을 사용할 것
        private void LoadLeftWeaponDamageCollider() {
            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            leftHandDamageCollider.physicalDamage = playerInventoryManager.leftWeapon.physicalDamage;
            leftHandDamageCollider.fireDamage = playerInventoryManager.leftWeapon.fireDamage;
            leftHandDamageCollider.teamIDNumber = playerStatsManager.teamIDNumber;
            
            // 왼쪽 무기의 DamageCollider에 현재 왼쪽 무기의 강인도 감쇄율을 전달
            leftHandDamageCollider.poiseBreak = playerInventoryManager.leftWeapon.poiseBreak;
            // 현재 왼쪽손에 들려있는 무기 모델의 자식에 있는 WeaponFX 스크립트 파일을 불러옴
            playerEffectsManager.leftWeaponFX = leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
        }

        private void LoadRightWeaponDamageCollider() {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            rightHandDamageCollider.physicalDamage = playerInventoryManager.rightWeapon.physicalDamage;
            rightHandDamageCollider.fireDamage = playerInventoryManager.rightWeapon.fireDamage;
            rightHandDamageCollider.teamIDNumber = playerStatsManager.teamIDNumber;

            // 오른쪽 무기의 DamageCollider에 현재 오른쪽 무기의 강인도 감쇄율을 전달
            rightHandDamageCollider.poiseBreak = playerInventoryManager.rightWeapon.poiseBreak;
            // 현재 오른쪽손에 들려있는 무기 모델의 자식에 있는 WeaponFX 스크립트 파일을 불러옴
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
        public void GrantWeaponAttackingPoiseBonus() { // (특)대형무기 공격 도중 강인도 보너스 합산
            playerStatsManager.totalPoiseDefense = playerStatsManager.totalPoiseDefense + attackingWeapon.offensivePoiseBonus;
        }

        public void ResetWeaponAttackingPoiseBonus() { // 공격이 끝나면 강인도 보너스 초기화
            playerStatsManager.totalPoiseDefense = playerStatsManager.armorPoiseBonus;
        }
        #endregion
    }
}

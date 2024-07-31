using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerStatsManager : CharacterStatsManager {
        public int level;
        public string characterName;
        PlayerManager player;
        PlayerAnimatorManager playerAnimatorManager;

        public float staminaRegenerationAmount = 20;
        public float staminaRegenerationTimer = 0; // Stamina 회복에 필요한 딜레이

        //public int requriedSoulsToLevelUp;
        float sprintingTimer = 0;
        protected override void Awake() {
            base.Awake();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            player = GetComponent<PlayerManager>();
        }
        protected override void Start() {
            base.Start();
        }

        public void SetStatBarsHUD() {
            UIManager.instance.player = player;

            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            UIManager.instance.healthBar.SetMaxHealth(currentHealth);

            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            UIManager.instance.staminaBar.SetMaxStamina(currentStamina);

            maxFocus = SetMaxFocusFromFocusLevel();
            currentFocus = maxFocus;
            UIManager.instance.focusBar.SetMaxFocus(currentFocus);
            GetTotalDefense();
        }

        public void GetTotalDefense() {
            totalPhysicalDamageDefenseRate = 1 - (1 - physicalDamageAbsorptionHead / 100) * (1 - physicalDamageAbsorptionBody / 100) * (1 - physicalDamageAbsorptionLegs / 100) * (1 - physicalDamageAbsorptionHands / 100);
            //Debug.Log(totalPhysicalDamageAbsorption);
            totalFireDamageDefenseRate = 1 - (1 - fireDamageAbsorptionHead / 100) * (1 - fireDamageAbsorptionBody / 100) * (1 - fireDamageAbsorptionLegs / 100) * (1 - fireDamageAbsorptionHands / 100);
            //Debug.Log(totalFireDamageAbsorption);
        }

        public override void HandlePoiseResetTimer() {
            if (poiseResetTimer > 0) {
                poiseResetTimer -= Time.deltaTime;
            } else if (poiseResetTimer <= 0 && !player.isInteracting) {
                totalPoiseDefense = armorPoiseBonus;
            }
        }

        public override void TakeDamageNoAnimation(float damage, float fireDamage) {
            base.TakeDamageNoAnimation(damage, fireDamage);
            if (player.IsOwner)
                UIManager.instance.healthBar.SetCurrentHealth(currentHealth);
        }

        public override void TakePoisonDamage(float damage) {
            base.TakePoisonDamage(damage);

            if (player.IsOwner)
                UIManager.instance.healthBar.SetCurrentHealth(currentHealth);

            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
                playerAnimatorManager.PlayTargetAnimation("PoisonedDeath", true);
            }
        }

        public override void DeductStamina(float staminaToDeduct) {
            base.DeductStamina(staminaToDeduct);

            if (player.IsOwner)
                UIManager.instance.staminaBar.SetCurrentStamina(currentStamina);

            // TODO
            // stamina가 0이하로 떨어졌을 경우
        }

        public void DeductSprintingStamina(float staminaToDeduct) {
            if (player.IsOwner) {
                if (player.playerNetworkManager.isSprinting.Value) {
                    sprintingTimer += Time.deltaTime;

                    if (sprintingTimer > 0.1f) {
                        sprintingTimer = 0;
                        currentStamina -= staminaToDeduct;
                        UIManager.instance.staminaBar.SetCurrentStamina(currentStamina);
                    }
                } else {
                    sprintingTimer = 0;
                }
            }
        }

        public void RegenerateStamina() {
            if (player.IsOwner) {
                if (player.isInteracting || player.playerNetworkManager.isSprinting.Value) {
                    staminaRegenerationTimer = 0;
                    return;
                }

                staminaRegenerationTimer += Time.deltaTime;
                if (currentStamina < maxStamina && staminaRegenerationTimer > 1f) {

                    // 가드를 올리고 있을때는 좀더 느린속도로 스태미너가 회복되어야 함

                    currentStamina += staminaRegenerationAmount * Time.deltaTime;
                    UIManager.instance.staminaBar.SetCurrentStamina(currentStamina);
                }
            }
        }

        public override void HealPlayer(float healAmount) {
            base.HealPlayer(healAmount);
            if (player.IsOwner)
                UIManager.instance.healthBar.SetCurrentHealth(currentHealth);
        }

        public void DeductFocus(float focusPoints) {
            if (player.IsOwner) {
                currentFocus -= focusPoints;
                if (currentFocus < 0) {
                    currentFocus = 0;
                }
                UIManager.instance.focusBar.SetCurrentFocus(currentFocus);
            }
        }

        public void AddSouls(int souls) {

            soulCount += souls;
        }

    }
}
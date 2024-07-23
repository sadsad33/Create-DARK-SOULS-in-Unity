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
        public float staminaRegenerationTimer = 0;

        //public int requriedSoulsToLevelUp;
        float sprintingTimer = 0;
        protected override void Awake() {
            base.Awake();
            //requriedSoulsToLevelUp = level * 65 * (level / 10 > 0 ? level / 10 : 1);
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
            UIManager.instance.healthBar.SetCurrentHealth(currentHealth);
        }

        public override void TakePoisonDamage(float damage) {
            base.TakePoisonDamage(damage);
            UIManager.instance.healthBar.SetCurrentHealth(currentHealth);
            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
                playerAnimatorManager.PlayTargetAnimation("PoisonedDeath", true);
            }
        }

        public override void TakeDamage(float damage, float fireDamage, string damageAnimation, CharacterManager enemyCharacterDamaingMe) {
            if (player.isInvulnerable) return;

            base.TakeDamage(damage, fireDamage, damageAnimation, enemyCharacterDamaingMe);

            UIManager.instance.healthBar.SetCurrentHealth(currentHealth);
            playerAnimatorManager.PlayTargetAnimation(damageAnimation, true);

            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
                playerAnimatorManager.PlayTargetAnimation("Dead", true);
                // Dead 애니메이션은 Transition으로 Empty에 연결해놓지 않았다.
                // Empty 에서 isInteracting 항목을 초기화 함으로써 다음 애니메이션으로 넘어갈수 있게끔 해주는데 플레이어가 죽는다면 그럴필요 없음
            }
        }

        public override void DeductStamina(float staminaToDeduct) {
            base.DeductStamina(staminaToDeduct);
            UIManager.instance.staminaBar.SetCurrentStamina(currentStamina);

            // TODO
            // stamina가 0이하로 떨어졌을 경우
        }

        public void DeductSprintingStamina(float staminaToDeduct) {

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

        public void RegenerateStamina() {
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

        public override void HealPlayer(float healAmount) {
            base.HealPlayer(healAmount);
            UIManager.instance.healthBar.SetCurrentHealth(currentHealth);
        }

        public void DeductFocus(float focusPoints) {
            currentFocus -= focusPoints;
            if (currentFocus < 0) {
                currentFocus = 0;
            }
            UIManager.instance.focusBar.SetCurrentFocus(currentFocus);
        }

        public void AddSouls(int souls) {
            soulCount += souls;
        }

    }
}
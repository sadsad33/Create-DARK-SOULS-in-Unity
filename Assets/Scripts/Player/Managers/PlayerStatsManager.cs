using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerStatsManager : CharacterStatsManager {
        public int level;
        PlayerManager playerManager;
        PlayerAnimatorManager playerAnimatorManager;

        public float staminaRegenerationAmount = 20;
        public float staminaRegenerationTimer = 0;

        //public int requriedSoulsToLevelUp;
        
        protected override void Awake() {
            base.Awake();
            //requriedSoulsToLevelUp = level * 65 * (level / 10 > 0 ? level / 10 : 1);
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerManager = GetComponent<PlayerManager>();
        }
        protected override void Start() {
            base.Start();
        }

        public void SetStatBarsHUD() {
            UIManager.instance.player = playerManager;
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
        private void GetTotalDefense() {
            totalPhysicalDamageAbsorption = 1 - (1 - physicalDamageAbsorptionHead / 100) * (1 - physicalDamageAbsorptionBody / 100) * (1 - physicalDamageAbsorptionLegs / 100) * (1 - physicalDamageAbsorptionHands / 100);
            //Debug.Log(totalPhysicalDamageAbsorption);
            totalFireDamageAbsorption = 1 - (1 - fireDamageAbsorptionHead / 100) * (1 - fireDamageAbsorptionBody / 100) * (1 - fireDamageAbsorptionLegs / 100) * (1 - fireDamageAbsorptionHands / 100);
            //Debug.Log(totalFireDamageAbsorption);
        }

        public override void HandlePoiseResetTimer() {
            if (poiseResetTimer > 0) {
                poiseResetTimer -= Time.deltaTime;
            } else if (poiseResetTimer <= 0 && !playerManager.isInteracting) {
                totalPoiseDefense = armorPoiseBonus;
            }
        }
        private float SetMaxHealthFromHealthLevel() {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        private float SetMaxFocusFromFocusLevel() {
            maxFocus = focusLevel * 10;
            return maxFocus;
        }

        private float SetMaxStaminaFromStaminaLevel() {
            maxStamina = staminaLevel * 10;
            return maxStamina;
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

        public override void TakeDamage(float damage, float fireDamage, string damageAnimation) {
            if (playerManager.isInvulnerable) return;

            base.TakeDamage(damage, fireDamage, damageAnimation);

            UIManager.instance.healthBar.SetCurrentHealth(currentHealth);
            playerAnimatorManager.PlayTargetAnimation(damageAnimation, true);

            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
                playerAnimatorManager.PlayTargetAnimation("Dead", true);
                // Dead �ִϸ��̼��� Transition���� Empty�� �����س��� �ʾҴ�.
                // Empty ���� isInteracting �׸��� �ʱ�ȭ �����ν� ���� �ִϸ��̼����� �Ѿ�� �ְԲ� ���ִµ� �÷��̾ �״´ٸ� �׷��ʿ� ����
            }
        }

        public void TakeStaminaDamage(float damage) {
            currentStamina -= damage;
            UIManager.instance.staminaBar.SetCurrentStamina(currentStamina);

            // TODO
            // stamina�� 0���Ϸ� �������� ���
        }

        public void RegenerateStamina() {
            if (playerManager.isInteracting) {
                staminaRegenerationTimer = 0;
                return;
            }
            staminaRegenerationTimer += Time.deltaTime;
            if (currentStamina < maxStamina && staminaRegenerationTimer > 1f) {
                currentStamina += staminaRegenerationAmount * Time.deltaTime;
                UIManager.instance.staminaBar.SetCurrentStamina(currentStamina);
            }
        }

        public void HealPlayer(float healAmount) {
            currentHealth += healAmount;
            if (currentHealth > maxHealth) {
                currentHealth = maxHealth;
            }
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
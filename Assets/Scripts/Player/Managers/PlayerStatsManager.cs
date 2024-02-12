using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerStatsManager : CharacterStatsManager {
        public HealthBar healthBar;
        public StaminaBar staminaBar;
        public FocusBar focusBar;
        PlayerManager playerManager;
        PlayerAnimatorManager playerAnimatorManager;

        public float staminaRegenerationAmount = 20;
        public float staminaRegenerationTimer = 0;
        protected override void Awake() {
            base.Awake();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerManager = GetComponent<PlayerManager>();
        }
        private void Start() {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(currentHealth);

            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            staminaBar.SetMaxStamina(currentStamina);

            maxFocus = SetMaxFocusFromFocusLevel();
            currentFocus = maxFocus;
            focusBar.SetMaxFocus(currentFocus);
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
            healthBar.SetCurrentHealth(currentHealth);
        }

        public override void TakePoisonDamage(float damage) {
            base.TakePoisonDamage(damage);
            healthBar.SetCurrentHealth(currentHealth);
            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
                playerAnimatorManager.PlayTargetAnimation("PoisonedDeath", true);
            }
        }

        public override void TakeDamage(float damage, float fireDamage, string damageAnimation) {
            if (playerManager.isInvulnerable) return;

            base.TakeDamage(damage, fireDamage, damageAnimation);
           
            healthBar.SetCurrentHealth(currentHealth);
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
            staminaBar.SetCurrentStamina(currentStamina);

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
                staminaBar.SetCurrentStamina(currentStamina);
            }
        }

        public void HealPlayer(float healAmount) {
            currentHealth += healAmount;
            if (currentHealth > maxHealth) {
                currentHealth = maxHealth;
            }
            healthBar.SetCurrentHealth(currentHealth);
        }

        public void DeductFocus(float focusPoints) {
            currentFocus -= focusPoints;
            if (currentFocus < 0) {
                currentFocus = 0;
            }
            focusBar.SetCurrentFocus(currentFocus);
        }

        public void AddSouls(int souls) {
            soulCount += souls;
        }
    }
}
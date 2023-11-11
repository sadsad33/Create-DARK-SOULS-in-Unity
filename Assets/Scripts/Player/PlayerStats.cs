using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerStats : CharacterStats {
        HealthBar healthBar;
        StaminaBar staminaBar;
        FocusBar focusBar;
        PlayerManager playerManager;
        AnimatorHandler animatorHandler;

        public float staminaRegenerationAmount = 20;
        public float staminaRegenerationTimer = 0;
        private void Awake() {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            healthBar = FindObjectOfType<HealthBar>();
            staminaBar = FindObjectOfType<StaminaBar>();
            focusBar = FindObjectOfType<FocusBar>();
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

        public void TakeDamage(float damage) {
            if (playerManager.isInvulnerable) return;
            if (isDead) return;
            currentHealth -= damage;
            healthBar.SetCurrentHealth(currentHealth);
            animatorHandler.PlayTargetAnimation("Damage", true);

            if (currentHealth <= 0) {
                currentHealth = 0;
                animatorHandler.PlayTargetAnimation("Dead", true);
                isDead = true;
                // Dead �ִϸ��̼��� Transition���� Empty�� �����س��� �ʾҴ�.
                // Empty ���� isInteracting �׸��� �ʱ�ȭ �����ν� ���� �ִϸ��̼����� �Ѿ�� �ְԲ� ���ִµ� �÷��̾ �״´ٸ� �׷��ʿ� ����
            }
        }

        public void TakeStaminaDamage(int damage) {
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
    }
}
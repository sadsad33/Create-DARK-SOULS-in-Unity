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
                // Dead 애니메이션은 Transition으로 Empty에 연결해놓지 않았다.
                // Empty 에서 isInteracting 항목을 초기화 함으로써 다음 애니메이션으로 넘어갈수 있게끔 해주는데 플레이어가 죽는다면 그럴필요 없음
            }
        }

        public void TakeStaminaDamage(int damage) {
            currentStamina -= damage;
            staminaBar.SetCurrentStamina(currentStamina);

            // TODO
            // stamina가 0이하로 떨어졌을 경우
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
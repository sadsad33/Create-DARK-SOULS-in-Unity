using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerStats : CharacterStats {
        public HealthBar healthBar;
        public StaminaBar staminaBar;

        AnimatorHandler animatorHandler;
        private void Awake() {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            healthBar = FindObjectOfType<HealthBar>();
            staminaBar = FindObjectOfType<StaminaBar>();
        }
        private void Start() {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(currentHealth);

            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            staminaBar.SetMaxStamina(currentStamina);
        }
        
        private int SetMaxHealthFromHealthLevel() {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        private int SetMaxStaminaFromStaminaLevel() {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }

        public void TakeDamage(int damage) {
            currentHealth -= damage;
            healthBar.SetCurrentHealth(currentHealth);
            animatorHandler.PlayTargetAnimation("Damage", true);

            if (currentHealth <= 0) {
                currentHealth = 0;
                animatorHandler.PlayTargetAnimation("Dead", true);
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
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerStats : MonoBehaviour {
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;

        public HealthBar healthBar;
        AnimatorHandler animatorHandler;
        private void Awake() {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }
        private void Start() {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }
        
        private int SetMaxHealthFromHealthLevel() {
            maxHealth = healthLevel * 10;
            return maxHealth;
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
    }
}
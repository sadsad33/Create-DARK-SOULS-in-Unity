using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyStats : CharacterStats {
        public int soulsAwardedOnDeath;
        EnemyAnimatorManager enemyAnimatorManager;
        private void Awake() {
            soulsAwardedOnDeath = 50;
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        }

        private void Start() {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
        }

        private float SetMaxHealthFromHealthLevel() {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamageNoAnimation(float damage) {
            currentHealth -= damage;
            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
            }
        }

        public void TakeDamage(float damage) {
            if (isDead) return;
            currentHealth -= damage;
            enemyAnimatorManager.PlayTargetAnimation("Damage", true);

            if (currentHealth <= 0) {
                HandleDeath();
            }
        }

        private void HandleDeath() {
            currentHealth = 0;
            enemyAnimatorManager.PlayTargetAnimation("Dead",true);
            isDead = true;
        }
    }
}

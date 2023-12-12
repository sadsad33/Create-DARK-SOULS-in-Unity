using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyStats : CharacterStats {
        public int soulsAwardedOnDeath;
        EnemyAnimatorManager enemyAnimatorManager;
        public UIEnemyHealthBar enemyHealthBar;
        private void Awake() {
            soulsAwardedOnDeath = 50;
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        }

        private void Start() {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            enemyHealthBar.SetMaxHealth(maxHealth);
        }

        private float SetMaxHealthFromHealthLevel() {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamageNoAnimation(float damage) {
            currentHealth -= damage;
            enemyHealthBar.SetHealth(currentHealth);
            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
            }
        }

        public override void TakeDamage(float damage, string damageAnimation = "Damage") {
            if (isDead) return;
            currentHealth -= damage;
            enemyHealthBar.SetHealth(currentHealth);
            enemyAnimatorManager.PlayTargetAnimation(damageAnimation, true);

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

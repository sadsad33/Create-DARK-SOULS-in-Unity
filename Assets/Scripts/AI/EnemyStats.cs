using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyStats : CharacterStats {
        public int soulsAwardedOnDeath;
        EnemyAnimatorManager enemyAnimatorManager;
        BossManager bossManager;
        public UIEnemyHealthBar enemyHealthBar;

        public bool isBoss;
        private void Awake() {
            soulsAwardedOnDeath = 50;
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            bossManager = GetComponent<BossManager>();
            // ���������� ��� Start �޼��忡�� ������ �������� ������ ������ �����������
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
        }

        private void Start() {
            if (!isBoss)
                enemyHealthBar.SetMaxHealth(maxHealth);
        }

        private float SetMaxHealthFromHealthLevel() {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        // �����̳� ����� �ִϸ��̼��� �����ؾ� �ϴ� ��� ���
        public void TakeDamageNoAnimation(float damage) {
            currentHealth -= damage;
            if (!isBoss)
                enemyHealthBar.SetHealth(currentHealth);
            else if (isBoss && bossManager != null)
                bossManager.UpdateBossHealthBar(currentHealth);

            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
            }
        }

        public override void TakeDamage(float damage, string damageAnimation = "Damage") {
            base.TakeDamage(damage, damageAnimation = "Damage");

            if (!isBoss)
                enemyHealthBar.SetHealth(currentHealth);
            else if (isBoss && bossManager != null)
                bossManager.UpdateBossHealthBar(currentHealth);
            enemyAnimatorManager.PlayTargetAnimation(damageAnimation, true);

            if (currentHealth <= 0) {
                HandleDeath();
            }
        }

        private void HandleDeath() {
            currentHealth = 0;
            enemyAnimatorManager.PlayTargetAnimation("Dead", true);
            isDead = true;
        }
    }
}

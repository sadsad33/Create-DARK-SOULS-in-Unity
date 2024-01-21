using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyStatsManager : CharacterStatsManager {
        EnemyAnimatorManager enemyAnimatorManager;
        BossManager bossManager;
        public UIEnemyHealthBar enemyHealthBar;

        public bool isBoss;
        private void Awake() {
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
            bossManager = GetComponent<BossManager>();
            // ���������� ��� Start �޼��忡�� ������ �������� ������ ������ �����������
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
        }

        private void Start() {
            if (!isBoss) enemyHealthBar.SetMaxHealth(maxHealth);
        }

        private float SetMaxHealthFromHealthLevel() {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        // �����̳� ����� �ִϸ��̼��� �����ؾ� �ϴ� ��� ���
        public override void TakeDamageNoAnimation(float damage) {
            base.TakeDamageNoAnimation(damage);
            if (!isBoss)
                enemyHealthBar.SetHealth(currentHealth);
            else if (isBoss && bossManager != null)
                bossManager.UpdateBossHealthBar(currentHealth, maxHealth);
        }

        public override void TakePoisonDamage(float damage) {
            base.TakePoisonDamage(damage);
            
            if (!isBoss)
                enemyHealthBar.SetHealth(currentHealth);
            else if (isBoss && bossManager != null)
                bossManager.UpdateBossHealthBar(currentHealth, maxHealth);

            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
                enemyAnimatorManager.PlayTargetAnimation("PoisonedDeath", true);
            }
        }

        public override void TakeDamage(float damage, string damageAnimation = "Damage") {

            base.TakeDamage(damage, damageAnimation = "Damage");

            if (!isBoss)
                enemyHealthBar.SetHealth(currentHealth);
            else if (isBoss && bossManager != null)
                bossManager.UpdateBossHealthBar(currentHealth, maxHealth);

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

        public void BreakGuard() {
            enemyAnimatorManager.PlayTargetAnimation("Break Guard", true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyStatsManager : CharacterStatsManager {
        EnemyAnimatorManager enemyAnimatorManager;
        BossManager bossManager;
        public UIEnemyHealthBar enemyHealthBar;

        protected override void Awake() {
            base.Awake();
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
            bossManager = GetComponent<BossManager>();
            // 보스몬스터의 경우 Start 메서드에서 스탯을 가져오기 때문에 그전에 세팅해줘야함
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

        // 뒤잡이나 앞잡등 애니메이션을 강제해야 하는 경우 사용
        public override void TakeDamageNoAnimation(float damage, float fireDamage) {
            base.TakeDamageNoAnimation(damage, fireDamage);
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

        public override void TakeDamage(float damage, float fireDamage, string damageAnimation) {

            base.TakeDamage(damage, fireDamage, damageAnimation);

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

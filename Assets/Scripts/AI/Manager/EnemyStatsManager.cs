using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyStatsManager : CharacterStatsManager {
        EnemyAnimatorManager enemyAnimatorManager;
        BossManager bossManager;
        EnemyManager enemyManager;
        public UIEnemyHealthBar enemyHealthBar;

        protected override void Awake() {
            base.Awake();
            enemyManager = GetComponent<EnemyManager>();
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
            if (enemyManager.isInvulnerable) return;
            base.TakeDamageNoAnimation(damage, fireDamage);
            if (!isBoss)
                enemyHealthBar.SetHealth(currentHealth);
            else if (isBoss && bossManager != null)
                bossManager.UpdateBossHealthBar(currentHealth, maxHealth);
            if (isDead && !enemyManager.isGrabbed) {
                HandleDeath("Dead");
            }
        }

        public override void TakePoisonDamage(float damage) {
            base.TakePoisonDamage(damage);
            if (!isBoss)
                enemyHealthBar.SetHealth(currentHealth);
            else if (isBoss && bossManager != null)
                bossManager.UpdateBossHealthBar(currentHealth, maxHealth);
            
            if (isDead && !enemyManager.isGrabbed) {
                HandleDeath("PoisonedDeath");
            }
        }

        public override void TakeDamage(float damage, float fireDamage, string damageAnimation) {
            if (enemyManager.isInvulnerable) return;
            base.TakeDamage(damage, fireDamage, damageAnimation);
            if (!isBoss)
                enemyHealthBar.SetHealth(currentHealth);
            else if (isBoss && bossManager != null)
                bossManager.UpdateBossHealthBar(currentHealth, maxHealth);

            enemyAnimatorManager.PlayTargetAnimation(damageAnimation, true);
            if (isDead && !enemyManager.isGrabbed) {
                HandleDeath("Dead");
            }
        }

        private void HandleDeath(string deathAnimation) {
            //currentHealth = 0;
            enemyAnimatorManager.PlayTargetAnimation(deathAnimation, true);
            //isDead = true;
        }
    }
}

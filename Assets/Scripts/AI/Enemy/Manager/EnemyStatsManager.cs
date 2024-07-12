using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    // ���� ����(����, ���ε�, HP, FP, ���¹̳�, �޴� ���� ��)
    public class EnemyStatsManager : CharacterStatsManager {
        EnemyAnimatorManager enemyAnimatorManager;
        public EnemyLocomotionManager enemyLocomotionManager;
        BossManager bossManager;
        EnemyManager enemyManager;
        public UIEnemyHealthBar enemyHealthBar;
        public WorldEventManager worldEventManager;
        protected override void Awake() {
            base.Awake();
            enemyManager = GetComponent<EnemyManager>();
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            bossManager = GetComponent<BossManager>();
            // ���������� ��� Start �޼��忡�� ������ �������� ������ ������ �����������
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
        }

        protected override void Start() {
            base.Start();
            if (!isBoss) enemyHealthBar.SetMaxHealth(maxHealth);
        }

        private float SetMaxHealthFromHealthLevel() {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        // �����̳� ����� �ִϸ��̼��� �����ؾ� �ϴ� ��� ���
        public override void TakeDamageNoAnimation(float damage, float fireDamage) {
            if (enemyManager.isInvulnerable) return;
            base.TakeDamageNoAnimation(damage, fireDamage);
            if (!isBoss)
                enemyHealthBar.UpdateHealth(currentHealth);
            else if (isBoss && bossManager != null)
                bossManager.UpdateBossHealthBar(currentHealth, maxHealth);
            if (isDead && !enemyManager.isGrabbed) {
                HandleDeath("Dead");
            } else if (isDead && enemyManager.isGrabbed) {
                HandleDeathWithNoAnimation();
            }
        }

        public override void TakePoisonDamage(float damage) {
            base.TakePoisonDamage(damage);
            if (!isBoss)
                enemyHealthBar.UpdateHealth(currentHealth);
            else if (isBoss && bossManager != null)
                bossManager.UpdateBossHealthBar(currentHealth, maxHealth);
            
            if (isDead && !enemyManager.isGrabbed) {
                HandleDeath("PoisonedDeath");
            }
        }

        public override void TakeDamage(float damage, float fireDamage, string damageAnimation) {
            if (enemyManager.isInvulnerable) return;
            base.TakeDamage(damage, fireDamage, damageAnimation);
            Debug.Log(damage + fireDamage);
            if (!isBoss)
                enemyHealthBar.UpdateHealth(currentHealth);
            else if (isBoss && bossManager != null)
                bossManager.UpdateBossHealthBar(currentHealth, maxHealth);

            enemyAnimatorManager.PlayTargetAnimation(damageAnimation, true);
            if (isDead && !enemyManager.isGrabbed) {
                HandleDeath("Dead");
            }
        }

        private void HandleDeath(string deathAnimation) {
            //currentHealth = 0;
            Debug.Log("Dead");
            enemyAnimatorManager.PlayTargetAnimation(deathAnimation, true);
            //enemyManager.enemyRigidbody.isKinematic = false;
            //enemyManager.enemyRigidbody.useGravity = false;
            //enemyLocomotionManager.characterCollider.enabled = false;
            //enemyManager.navMeshAgent.enabled = false;

            if (isBoss) worldEventManager.BossHasBeenDefeated();
            ChangeLayerIncludingAllChilds(transform.gameObject);
            //Destroy(gameObject, 3.0f);
            //isDead = true;
        }

        private void HandleDeathWithNoAnimation() {
            Debug.Log("DeadWithNoAnimation");
            //enemyManager.enemyRigidbody.isKinematic = false;
            //enemyManager.enemyRigidbody.useGravity = false;
            //enemyLocomotionManager.characterCollider.enabled = false;
            //enemyManager.navMeshAgent.enabled = false;
            if (isBoss) worldEventManager.BossHasBeenDefeated();
            ChangeLayerIncludingAllChilds(transform.gameObject);
        }

        private void ChangeLayerIncludingAllChilds(GameObject obj) {
            foreach (Transform child in obj.transform) {
                ChangeLayerIncludingAllChilds(child.gameObject);
                child.gameObject.layer = 7;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    // 스탯 관리(방어력, 강인도, HP, FP, 스태미나, 받는 피해 등)
    public class AICharacterStatsManager : CharacterStatsManager {
        AICharacterAnimatorManager enemyAnimatorManager;
        public AICharacterLocomotionManager enemyLocomotionManager;
        //BossManager bossManager;
        AICharacterManager aiCharacter;

        public UIEnemyHealthBar enemyHealthBar;
        //public WorldEventManager worldEventManager;
        protected override void Awake() {
            base.Awake();
            aiCharacter = GetComponent<AICharacterManager>();
            enemyAnimatorManager = GetComponent<AICharacterAnimatorManager>();
            enemyLocomotionManager = GetComponent<AICharacterLocomotionManager>();
            //bossManager = GetComponent<BossManager>();
        }

        protected override void Start() {
            base.Start();
            //if (!aiCharacter.isBoss) {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            enemyHealthBar.SetMaxHealth(maxHealth);
            //}
        }

        public override float SetMaxHealthFromHealthLevel() {
            base.SetMaxHealthFromHealthLevel();
            return maxHealth;
        }

        // 뒤잡이나 앞잡등 애니메이션을 강제해야 하는 경우 사용
        public override void TakeDamageNoAnimation(float damage, float fireDamage) {
            if (aiCharacter.isInvulnerable) return;
            base.TakeDamageNoAnimation(damage, fireDamage);
            enemyHealthBar.UpdateHealth(currentHealth);
            //else if (isBoss && bossManager != null)
            //    bossManager.UpdateBossHealthBar(currentHealth, maxHealth);
            if (isDead && !aiCharacter.isGrabbed) {
                HandleDeath("Dead");
            } else if (isDead && aiCharacter.isGrabbed) {
                HandleDeathWithNoAnimation();
            }
        }

        public override void TakePoisonDamage(float damage) {
            base.TakePoisonDamage(damage);
            enemyHealthBar.UpdateHealth(currentHealth);
            //else if (isBoss && bossManager != null)
            //    bossManager.UpdateBossHealthBar(currentHealth, maxHealth);

            if (isDead && !aiCharacter.isGrabbed) {
                HandleDeath("PoisonedDeath");
            }
        }

        private void HandleDeath(string deathAnimation) {
            //currentHealth = 0;
            Debug.Log("Dead");
            enemyAnimatorManager.PlayTargetAnimation(deathAnimation, true);
            //enemyManager.navMeshAgent.enabled = false;

            //if (isBoss) worldEventManager.BossHasBeenDefeated();
            ChangeLayerIncludingAllChilds(transform.gameObject);
            //Destroy(gameObject, 3.0f);
            //isDead = true;
        }

        private void HandleDeathWithNoAnimation() {
            //Debug.Log("DeadWithNoAnimation");
            //enemyManager.navMeshAgent.enabled = false;
            //if (isBoss) worldEventManager.BossHasBeenDefeated();
            ChangeLayerIncludingAllChilds(transform.gameObject);
        }

        private void ChangeLayerIncludingAllChilds(GameObject obj) {
            obj.gameObject.layer = 7;
            foreach (Transform child in obj.transform) {
                ChangeLayerIncludingAllChilds(child.gameObject);
                child.gameObject.layer = 7;
            }
        }
    }
}

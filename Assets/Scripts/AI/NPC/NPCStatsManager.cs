using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCStatsManager : CharacterStatsManager {
        NPCAnimatorManager npcAnimatorManager;
        NPCManager npcManager;
        public UIEnemyHealthBar npcHealthBar;        

        protected override void Awake() {
            base.Awake();
            npcManager = GetComponent<NPCManager>();
            npcAnimatorManager = GetComponent<NPCAnimatorManager>();
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
        }

        protected override void Start() {
            base.Start();
            npcHealthBar.SetMaxHealth(maxHealth);
        }

        private float SetMaxHealthFromHealthLevel() {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public override void TakeDamageNoAnimation(float damage, float fireDamage) {
            if (npcManager.isInvulnerable) return;
            base.TakeDamageNoAnimation(damage, fireDamage);
            npcHealthBar.UpdateHealth(currentHealth);
            if (isDead && !npcManager.isGrabbed) HandleDeath("Dead");
        }

        public override void TakePoisonDamage(float damage) {
            base.TakePoisonDamage(damage);
            npcHealthBar.UpdateHealth(currentHealth);
            if (isDead && !npcManager.isGrabbed) HandleDeath("PoisonedDeath");
        }

        public override void TakeDamage(float physicalDamage, float fireDamage, string damageAnimation) {
            if (npcManager.isInvulnerable) return;
            base.TakeDamage(physicalDamage, fireDamage, damageAnimation);
            npcHealthBar.UpdateHealth(currentHealth);
            npcAnimatorManager.PlayTargetAnimation(damageAnimation, true);
            if (isDead && !npcManager.isGrabbed) HandleDeath("Dead");
        }

        private void HandleDeath(string deathAnimation) {
            npcAnimatorManager.PlayTargetAnimation(deathAnimation, true);
        }
    }
}

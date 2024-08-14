using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class BossStatsManager : AICharacterStatsManager {
        BossManager boss;
        public BossHealthBar bossHealthBar;
        public WorldEventManager worldEventManager;
        protected override void Awake() {
            base.Awake();
            boss = GetComponent<BossManager>();
        }

        protected override void Start() {
            base.Start();
            bossHealthBar = worldEventManager.bossHealthBar;
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            bossHealthBar.SetMaxHealth(maxHealth);
        }

        public override void TakeDamageNoAnimation(float damage, float fireDamage) {
            
        }
    }
}

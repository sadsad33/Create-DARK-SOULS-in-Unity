using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SoulsLike {
    public class BossStatsManager : AICharacterStatsManager {
        //public static Func<WorldEventManager> FetchWorldEventManagerDelegate;
        BossManager boss;
        //public BossHealthBar bossHealthBar;
        public WorldEventManager worldEventManager;
        
        protected override void Awake() {
            base.Awake();
            boss = GetComponent<BossManager>();
            worldEventManager = FindObjectOfType<WorldEventManager>();
            worldEventManager.boss = boss;
        }

        protected override void Start() {
            enemyHealthBar = worldEventManager.bossHealthBar;
            base.Start();
            //maxHealth = SetMaxHealthFromHealthLevel();
            //currentHealth = maxHealth;
            //bossHealthBar.SetMaxHealth(maxHealth);
        }

        public override void TakeDamageNoAnimation(float damage, float fireDamage) {
            base.TakeDamageNoAnimation(damage, fireDamage);
        }

        public override void TakePoisonDamage(float damage) {
            base.TakePoisonDamage(damage);
        }
    }
}

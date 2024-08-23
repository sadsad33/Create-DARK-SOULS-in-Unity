using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SoulsLike {
    public class BossStatsManager : AICharacterStatsManager {
        BossManager boss;
        
        protected override void Awake() {
            base.Awake();
            boss = GetComponent<BossManager>();
        }

        protected override void Start() {
            //base.Start();
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            //bossHealthBar.SetMaxHealth(maxHealth);
        }

        protected override void Update() {
            base.Update();
            boss.HandlePhaseShifting(currentHealth, maxHealth);
        }

        public override void TakeDamageNoAnimation(float damage, float fireDamage) {
            base.TakeDamageNoAnimation(damage, fireDamage);
        }

        public override void TakePoisonDamage(float damage) {
            base.TakePoisonDamage(damage);
        }
    }
}

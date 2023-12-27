using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class BossManager : MonoBehaviour {
        BossHealthBar bossHealthBar;
        public string bossName;
        EnemyStats enemyStats;
        private void Awake() {
            bossHealthBar = FindObjectOfType<BossHealthBar>();
            enemyStats = GetComponent<EnemyStats>();
        }

        private void Start() {
            bossHealthBar.SetBossName(bossName);
            bossHealthBar.SetBossMaxHealth(enemyStats.maxHealth);
        }

        public void UpdateBossHealthBar(float currentHealth) {
            bossHealthBar.SetBossCurrentHealth(currentHealth);
        }
        
        //페이즈 전환
        //공격 패턴 전환
    }
}

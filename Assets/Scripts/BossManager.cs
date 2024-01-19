using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class BossManager : MonoBehaviour {
        BossHealthBar bossHealthBar;
        public string bossName;
        EnemyStatsManager enemyStats;
        EnemyAnimatorManager enemyAnimatorManager;
        BossCombatStanceState bossCombatStanceState;

        [Header("Second Phase FX")]
        public GameObject particleFX;

        private void Awake() {
            bossHealthBar = FindObjectOfType<BossHealthBar>();
            enemyStats = GetComponent<EnemyStatsManager>();
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
            bossCombatStanceState = GetComponentInChildren<BossCombatStanceState>();
        }

        private void Start() {
            bossHealthBar.SetBossName(bossName);
            bossHealthBar.SetBossMaxHealth(enemyStats.maxHealth);
        }

        public void UpdateBossHealthBar(float currentHealth, float maxHealth) {
            bossHealthBar.SetBossCurrentHealth(currentHealth);
            if (currentHealth <= maxHealth / 2 && !bossCombatStanceState.hasPhaseShifted) {
                bossCombatStanceState.hasPhaseShifted = true;
                ShiftToSecondPhase();
            }
        }

        //페이즈 전환
        public void ShiftToSecondPhase() {
            enemyAnimatorManager.anim.SetBool("isInvulnerable", true);
            enemyAnimatorManager.anim.SetBool("isPhaseShifting", true);
            enemyAnimatorManager.PlayTargetAnimation("PhaseShift", true);
            //패턴 전환
            bossCombatStanceState.hasPhaseShifted = true;
        }
    }
}

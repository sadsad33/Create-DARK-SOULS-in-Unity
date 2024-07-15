using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class BossManager : MonoBehaviour {
        Animator animator;
        BossHealthBar bossHealthBar;
        public string bossName;
        EnemyStatsManager enemyStats;
        EnemyAnimatorManager enemyAnimatorManager;
        BossCombatStanceState bossCombatStanceState;

        [Header("Second Phase FX")]
        public GameObject particleFX;

        private void Awake() {
            animator = GetComponent<Animator>();
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
                if (enemyStats.isStuned) return;
                //Debug.Log("2페이즈 진입");
                bossCombatStanceState.hasPhaseShifted = true;
                ShiftToSecondPhase();
            }
        }

        //페이즈 전환
        public void ShiftToSecondPhase() {
            animator.SetBool("isInvulnerable", true);
            animator.SetBool("isPhaseShifting", true);
            enemyAnimatorManager.PlayTargetAnimation("PhaseShift", true);
            //패턴 전환
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class BossManager : AICharacterManager {
        //BossHealthBar bossHealthBar;
        public string bossName;
        BossCombatStanceState bossCombatStanceState;

        [Header("Second Phase FX")]
        public GameObject particleFX;

        protected override void Awake() {
            base.Awake();
            isBoss = true;
            animator = GetComponent<Animator>();
            aiStatsManager = GetComponent<BossStatsManager>();    
            bossCombatStanceState = GetComponentInChildren<BossCombatStanceState>();
        }

        protected override void Start() {
            base.Start();
        }

        public void UpdateBossHealthBar(float currentHealth, float maxHealth) {
            if (currentHealth <= maxHealth / 2 && !bossCombatStanceState.hasPhaseShifted) {
                if (aiStatsManager.isStuned) return;
                //Debug.Log("2페이즈 진입");
                bossCombatStanceState.hasPhaseShifted = true;
                ShiftToSecondPhase();
            }
        }

        //페이즈 전환
        public void ShiftToSecondPhase() {
            animator.SetBool("isInvulnerable", true);
            animator.SetBool("isPhaseShifting", true);
            aiAnimatorManager.PlayTargetAnimation("PhaseShift", true);
            //패턴 전환
        }
    }
}

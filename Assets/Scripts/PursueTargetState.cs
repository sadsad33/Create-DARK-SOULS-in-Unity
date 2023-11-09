using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PursueTargetState : State {
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            // 목표 추적
            // 공격 사거리내에 타겟이 들어오면 Combat Stance State가 됨
            // 타겟이 공격 사거리 밖으로 나가면 Pursue Target State를 유지한채 추적
            return this;
        }
    }
}
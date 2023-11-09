using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class AttackState : State {
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            // attack score 변수에 따라 수행가능한 공격들중 하나를 선택한다
            // 선택된 공격이 각도나 거리에 의해 불가능해 진다면 새로운 공격을 선택
            // 공격이 가능하다면 이동을 멈춘후 공격
            // 공격 딜레이 시간을 세팅한다.
            // Combat Stance State로 돌아감 
            return this;
        }
    }
}

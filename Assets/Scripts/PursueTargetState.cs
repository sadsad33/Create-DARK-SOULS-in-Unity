using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PursueTargetState : State {
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            // ��ǥ ����
            // ���� ��Ÿ����� Ÿ���� ������ Combat Stance State�� ��
            // Ÿ���� ���� ��Ÿ� ������ ������ Pursue Target State�� ������ä ����
            return this;
        }
    }
}
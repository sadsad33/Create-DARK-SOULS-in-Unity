using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class CombatStanceState : State {
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            // ���� ��Ÿ� Ȯ��
            // ���� ��� �������� �Ȱų� ���ۺ��� ����
            // ���� ��Ÿ� ���� ���� Attack State�� �ȴ�.
            // ������ ������ ���¶�� Combat Stance State�� ���ƿ��� Ÿ�� ������ ��ȸ
            // ���� Ÿ���� ���� ��Ÿ� ������ ������������ Pursue Target State�� ��.
            return this;
        }
    }
}

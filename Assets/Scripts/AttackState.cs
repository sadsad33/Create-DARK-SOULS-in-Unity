using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class AttackState : State {
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            // attack score ������ ���� ���డ���� ���ݵ��� �ϳ��� �����Ѵ�
            // ���õ� ������ ������ �Ÿ��� ���� �Ұ����� ���ٸ� ���ο� ������ ����
            // ������ �����ϴٸ� �̵��� ������ ����
            // ���� ������ �ð��� �����Ѵ�.
            // Combat Stance State�� ���ư� 
            return this;
        }
    }
}

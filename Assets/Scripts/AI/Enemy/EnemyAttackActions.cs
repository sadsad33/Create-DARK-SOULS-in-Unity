using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "AI/Enemy Actions/Attack Actions")]
    public class EnemyAttackActions : EnemyActions {
        
        public bool canCombo;
        public EnemyAttackActions comboAction;

        public int attackScore = 3;
        public float recoveryTime = 2;
        public float maximumAttackAngle = 35;
        public float minimumAttackAngle = -35;

        public float minimumDistanceNeededToAttack = 0;
        public float maximumDistanceNeededToAttack = 1.5f;
    }
}
